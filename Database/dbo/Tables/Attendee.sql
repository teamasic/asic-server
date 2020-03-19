CREATE TABLE [dbo].[Attendee] (
    [Id]   INT          IDENTITY (1, 1) NOT NULL,
    [Code] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Attendee] PRIMARY KEY CLUSTERED ([Id] ASC)
);

