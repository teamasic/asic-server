CREATE PROCEDURE [dbo].[MergeRecordStagingInSyncData] 
	-- Add the parameters for the stored procedure here
	@stagingid IntID readonly
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @GroupsInStaging TABLE(GroupCode varchar(50), GroupName nvarchar(255), GroupCreateTime datetime, MaxSessionCount int)
	DECLARE @SessionsInStaging TABLE(SessionName nvarchar(50), SessionStartTime datetime, SessionEndTime datetime,
									RoomName varchar(50), RtspString varchar(255), GroupId int)
	DECLARE @RecordsInStaging TABLE(NewAttendeeId int, AttendeeCode varchar(50), 
										NewSessionId int, SessionName nvarchar(50), 
										SessionStartTime datetime, SessionEndTime datetime, 
										Present bit, IsEnrollInClass bit, GroupId int)
	DECLARE @AttendeeGroupsInStaging TABLE(AttendeeId int, AttendeeCode varchar(50), GroupCode varchar(50), GroupId int, IsActive bit)

	--merge group
	INSERT INTO @GroupsInStaging(GroupCode, GroupName, GroupCreateTime, MaxSessionCount) 
	(
		SELECT DISTINCT GroupCode, GroupName, GroupCreateTime, MaxSessionCount
		FROM [dbo].[RecordStaging]
		WHERE Id in (SELECT * FROM @stagingid)
	)

	MERGE [Groups] s
		USING  @GroupsInStaging t
			ON (s.Code = t.GroupCode)
		WHEN NOT MATCHED BY TARGET
			THEN INSERT (Code, [Name], DateTimeCreated, MaxSessionCount)
					VALUES(t.GroupCode, t.GroupName, t.GroupCreateTime, t.MaxSessionCount);
				
	--merge AttendeeGroups (Enrollment)
	INSERT INTO @AttendeeGroupsInStaging(AttendeeId, AttendeeCode, GroupCode, GroupId, IsActive) 
	(
		SELECT DISTINCT u.Id, AttendeeCode, rs.GroupCode, g.Id, rs.IsEnrollInClass
		FROM [dbo].[RecordStaging] rs
			INNER JOIN [User] u
				ON rs.AttendeeCode = u.RollNumber
			INNER JOIN [Groups] g
				ON rs.GroupCode = g.Code
		WHERE rs.Id in (SELECT * FROM @stagingid)
	)
	MERGE [AttendeeGroups] s
		USING  @AttendeeGroupsInStaging t
			ON (s.AttendeeId = t.AttendeeId and s.GroupId = t.GroupId)
		WHEN MATCHED
			THEN UPDATE SET 
				s.IsActive = t.IsActive
		WHEN NOT MATCHED BY TARGET
			THEN INSERT (AttendeeId, GroupId, IsActive)
					VALUES(t.AttendeeId, t.GroupId, t.IsActive);

	--merge session
	INSERT INTO @SessionsInStaging(SessionName, SessionStartTime, SessionEndTime, RoomName, RtspString, GroupId)
	(
		SELECT DISTINCT SessionName, SessionStartTime, SessionEndTime, RoomName, RtspString, g.Id as 'GroupId'
		FROM [dbo].[RecordStaging] rs 
				INNER JOIN [Groups] g
					ON rs.GroupCode = g.Code
		WHERE rs.Id in (SELECT * FROM @stagingid)
	)
	MERGE [Sessions] s
		USING  @SessionsInStaging t
			ON (s.[Name] = t.SessionName and s.StartTime = t.SessionStartTime 
										 and s.EndTime = t.SessionEndTime)
		WHEN NOT MATCHED BY TARGET
			THEN INSERT ([Name], StartTime, EndTime, RoomName, RtspString, GroupId)
					VALUES(t.SessionName, t.SessionStartTime, t.SessionEndTime, t.RoomName, t.RtspString, t.GroupId);
	
	--merge record
	INSERT INTO @RecordsInStaging(NewAttendeeId, AttendeeCode, NewSessionId, SessionName, SessionStartTime, SessionEndTime, 
									Present, IsEnrollInClass, GroupId)
	(
		SELECT a.Id as 'NewAttendeeId', rs.AttendeeCode,
				s.Id as 'NewSessionId', s.[Name] as 'SessionName', rs.SessionStartTime, rs.SessionEndTime, 
				rs.Present, rs.IsEnrollInClass, s.GroupId
		FROM RecordStaging rs
				INNER JOIN [User] a
					ON rs.Attendeecode = a.RollNumber
				INNER JOIN [Sessions] s
					ON (s.[Name] = rs.SessionName and rs.SessionStartTime = s.StartTime 
												 and rs.SessionEndTime = s.EndTime)
		WHERE rs.Id in (SELECT * FROM @stagingid)
	)
	MERGE [Records] s
		USING  @RecordsInStaging t
			ON (s.AttendeeId = t.NewAttendeeId and s.SessionId = t.NewSessionId)
		WHEN MATCHED
			THEN UPDATE SET 
				s.Present = t.Present
		WHEN NOT MATCHED BY TARGET
			THEN INSERT (AttendeeId, AttendeeCode, SessionId, SessionName, StartTime, EndTime, Present, GroupId)
					VALUES(t.NewAttendeeId, t.AttendeeCode, 
							t.NewSessionId, t.SessionName, t.SessionStartTime, t.SessionEndTime, 
							t.Present, t.GroupId);
END