CREATE TABLE [dbo].[AttendeeGroups] (
    [AttendeeId] BIGINT NOT NULL,
    [GroupId]    INT    NOT NULL,
    [IsActive]   BIT    CONSTRAINT [DF_AttendeeGroups_isActive] DEFAULT ((1)) NULL,
    CONSTRAINT [sqlite_autoindex_AttendeeGroups_1] PRIMARY KEY CLUSTERED ([AttendeeId] ASC, [GroupId] ASC),
    CONSTRAINT [FK_AttendeeGroups_Groups] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Groups] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AttendeeGroups_User] FOREIGN KEY ([AttendeeId]) REFERENCES [dbo].[User] ([Id])
);



