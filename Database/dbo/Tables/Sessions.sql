CREATE TABLE [dbo].[Sessions] (
    [Id]         INT           IDENTITY (1, 1) NOT NULL,
    [GroupId]    INT           NULL,
    [Name]       NVARCHAR (50) NULL,
    [StartTime]  DATETIME      NULL,
    [EndTime]    DATETIME      NULL,
    [RoomName]   NVARCHAR (50) NOT NULL,
    [RtspString] VARCHAR (255) NOT NULL,
    CONSTRAINT [PK__Sessions__3214EC073FC29D16] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Sessions_0_0] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Groups] ([Id])
);

