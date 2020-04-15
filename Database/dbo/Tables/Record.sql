CREATE TABLE [dbo].[Record] (
    [Id]              INT           IDENTITY (1, 1) NOT NULL,
    [AttendeeCode]    VARCHAR (50)  NOT NULL,
    [SessionName]     NVARCHAR (50) NULL,
    [StartTime]       DATETIME      CONSTRAINT [DF_Records_StartTime] DEFAULT (getdate()) NULL,
    [EndTime]         DATETIME      NULL,
    [Present]         BIT           CONSTRAINT [DF_Records_Present] DEFAULT ((0)) NOT NULL,
    [SessionId]       INT           NOT NULL,
    [AttendeeGroupId] INT           NOT NULL,
    CONSTRAINT [PK_Records] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Records_0_0] FOREIGN KEY ([SessionId]) REFERENCES [dbo].[Session] ([Id]),
    CONSTRAINT [FK_Records_AttendeeGroups] FOREIGN KEY ([AttendeeGroupId]) REFERENCES [dbo].[AttendeeGroup] ([Id])
);

