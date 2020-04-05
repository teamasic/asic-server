CREATE TABLE [dbo].[Rooms] (
    [Id]         INT           IDENTITY (1, 1) NOT NULL,
    [Name]       VARCHAR (50)  NOT NULL,
    [RtspString] VARCHAR (255) NOT NULL,
    CONSTRAINT [PK__Rooms__3214EC0715EC8008] PRIMARY KEY CLUSTERED ([Id] ASC)
);

