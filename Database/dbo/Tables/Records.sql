CREATE TABLE [dbo].[Records] (
    [Id]           BIGINT        IDENTITY (1, 1) NOT NULL,
    [AttendeeId]   BIGINT        NOT NULL,
    [GroupId]      INT           NULL,
    [SessionId]    INT           NOT NULL,
    [AttendeeCode] VARCHAR (50)  NOT NULL,
    [SessionName]  NVARCHAR (50) NULL,
    [StartTime]    DATETIME      CONSTRAINT [DF_Records_StartTime] DEFAULT (getdate()) NULL,
    [EndTime]      DATETIME      NULL,
    [Present]      BIT           CONSTRAINT [DF_Records_Present] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK__Records__3214EC07F9AE9C1D] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Records_0_0] FOREIGN KEY ([SessionId]) REFERENCES [dbo].[Sessions] ([Id]),
    CONSTRAINT [FK_Records_AttendeeGroups] FOREIGN KEY ([AttendeeId], [GroupId]) REFERENCES [dbo].[AttendeeGroups] ([AttendeeId], [GroupId]),
    CONSTRAINT [UK_Records] UNIQUE NONCLUSTERED ([AttendeeId] ASC, [GroupId] ASC, [SessionId] ASC)
);







