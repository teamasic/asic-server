CREATE TABLE [dbo].[Groups] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [Name]            NVARCHAR (255) NULL,
    [Code]            NVARCHAR (50)  NULL,
    [DateTimeCreated] DATETIME       NULL,
    [Deleted]         INT            NOT NULL,
    [MaxSessionCount] INT            NULL,
    CONSTRAINT [PK__Groups__3214EC07812DCBF3] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UK_Groups_Code] UNIQUE NONCLUSTERED ([Code] ASC)
);

