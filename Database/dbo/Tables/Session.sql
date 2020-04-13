CREATE TABLE [dbo].[Session] (
    [Id]        INT           IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (50) NULL,
    [StartTime] DATETIME      NULL,
    [EndTime]   DATETIME      NULL,
    [Status]    VARCHAR (50)  NULL,
    [GroupCode] VARCHAR (50)  NULL,
    [RoomId]    INT           NOT NULL,
    CONSTRAINT [PK__Sessions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Sessions_Groups] FOREIGN KEY ([GroupCode]) REFERENCES [dbo].[Group] ([Code]),
    CONSTRAINT [FK_Sessions_Rooms] FOREIGN KEY ([RoomId]) REFERENCES [dbo].[Room] ([Id])
);

