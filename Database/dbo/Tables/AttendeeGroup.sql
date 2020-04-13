CREATE TABLE [dbo].[AttendeeGroup] (
    [Id]           INT          IDENTITY (1, 1) NOT NULL,
    [AttendeeCode] VARCHAR (50) NOT NULL,
    [GroupCode]    VARCHAR (50) NOT NULL,
    [IsActive]     BIT          CONSTRAINT [DF_AttendeeGroups_isActive] DEFAULT ((1)) NULL,
    CONSTRAINT [PK_AttendeeGroups] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AttendeeGroups_Groups] FOREIGN KEY ([GroupCode]) REFERENCES [dbo].[Group] ([Code]),
    CONSTRAINT [FK_AttendeeGroups_User] FOREIGN KEY ([AttendeeCode]) REFERENCES [dbo].[User] ([Code]),
    CONSTRAINT [UK_AttendeeGroups] UNIQUE NONCLUSTERED ([AttendeeCode] ASC, [GroupCode] ASC)
);

