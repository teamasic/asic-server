CREATE PROCEDURE [dbo].[MergeRecordStagingInSyncData] 
	-- Add the parameters for the stored procedure here
	@stagingid IntID readonly
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @GroupsInStaging TABLE(GroupCode varchar(50), GroupName nvarchar(255), GroupCreateTime datetime, TotalSession int)
	DECLARE @SessionsInStaging TABLE(SessionName nvarchar(50), SessionStartTime datetime, SessionEndTime datetime,
									RoomId int, GroupCode varchar(50), [Status] varchar(50))
	DECLARE @RecordsInStaging TABLE(AttendeeCode varchar(50), AttendeeGroupId int,
										NewSessionId int, SessionName nvarchar(50), 
										SessionStartTime datetime, SessionEndTime datetime, 
										Present bit)
	DECLARE @AttendeeGroupsInStaging TABLE(AttendeeCode varchar(50), GroupCode varchar(50), IsEnrollInClass bit)

	--merge group
	INSERT INTO @GroupsInStaging(GroupCode, GroupName, GroupCreateTime, TotalSession) 
	(
		SELECT DISTINCT GroupCode, GroupName, GroupCreateTime, TotalSession
		FROM [dbo].[RecordStaging]
		WHERE Id in (SELECT * FROM @stagingid)
	)

	MERGE [Group] s
		USING  @GroupsInStaging t
			ON (s.Code = t.GroupCode)
		WHEN NOT MATCHED BY TARGET
			THEN INSERT (Code, [Name], DateTimeCreated, TotalSession, Deleted)
					VALUES(t.GroupCode, t.GroupName, t.GroupCreateTime, t.TotalSession, 0);
				
	--merge AttendeeGroups (Enrollment)
	INSERT INTO @AttendeeGroupsInStaging(AttendeeCode, GroupCode, IsEnrollInClass) 
	(
		SELECT DISTINCT rs.AttendeeCode, rs.GroupCode, rs.IsEnrollInClass
		FROM [dbo].[RecordStaging] rs
		WHERE rs.Id in (SELECT * FROM @stagingid)
	)
	MERGE [AttendeeGroup] s
		USING  @AttendeeGroupsInStaging t
			ON (s.AttendeeCode = t.AttendeeCode and s.GroupCode = t.GroupCode)
		WHEN MATCHED
			THEN UPDATE SET 
				s.IsActive = t.IsEnrollInClass
		WHEN NOT MATCHED BY TARGET
			THEN INSERT (AttendeeCode, GroupCode, IsActive)
					VALUES(t.AttendeeCode, t.GroupCode, t.IsEnrollInClass);

	--merge session
	INSERT INTO @SessionsInStaging(SessionName, SessionStartTime, SessionEndTime, RoomId, GroupCode, [Status])
	(
		SELECT DISTINCT rs.SessionName, rs.SessionStartTime, rs.SessionEndTime, rs.RoomId, rs.GroupCode, 'FINISHED'
		FROM [dbo].[RecordStaging] rs 
		WHERE rs.Id in (SELECT * FROM @stagingid)
	)
	MERGE [Session] s
		USING  @SessionsInStaging t
			ON (s.GroupCode = t.GroupCode and s.StartTime = t.SessionStartTime)
		WHEN NOT MATCHED BY TARGET
			THEN INSERT ([Name], StartTime, EndTime, RoomId, GroupCode, [Status])
					VALUES(t.SessionName, t.SessionStartTime, t.SessionEndTime, t.RoomId, t.GroupCode, t.[Status]);
	
	--merge record
	INSERT INTO @RecordsInStaging(AttendeeCode, SessionName, SessionStartTime, SessionEndTime, 
									Present, NewSessionId, AttendeeGroupId)
	(
		SELECT rs.AttendeeCode, rs.SessionName, rs.SessionStartTime, rs.SessionEndTime,
				rs.Present, s.Id as 'NewSessionId', ag.Id as 'AttendeeGroupId'
		FROM RecordStaging rs
				INNER JOIN [AttendeeGroup] ag
					ON (rs.Attendeecode = ag.AttendeeCode and rs.GroupCode = ag.GroupCode)
				INNER JOIN [Session] s
					ON (s.GroupCode = rs.GroupCode and rs.SessionStartTime = s.StartTime)
		WHERE rs.Id in (SELECT * FROM @stagingid)
	)
	MERGE [Record] s
		USING  @RecordsInStaging t
			ON (s.AttendeeGroupId = t.AttendeeGroupId and s.SessionId = t.NewSessionId)
		WHEN MATCHED
			THEN UPDATE SET 
				s.Present = t.Present
		WHEN NOT MATCHED BY TARGET
			THEN INSERT (AttendeeCode, SessionId, SessionName, StartTime, EndTime, Present, AttendeeGroupId)
					VALUES(t.AttendeeCode, 
							t.NewSessionId, t.SessionName, t.SessionStartTime, t.SessionEndTime, 
							t.Present, t.AttendeeGroupId);
END