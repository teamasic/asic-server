CREATE TABLE [dbo].[Groups] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [Code]            VARCHAR (50)   NULL,
    [Name]            NVARCHAR (255) NULL,
    [DateTimeCreated] DATETIME       NULL,
    [Deleted]         BIT            CONSTRAINT [DF_Groups_Deleted] DEFAULT ((0)) NOT NULL,
    [MaxSessionCount] INT            NULL,
    CONSTRAINT [PK__Groups__3214EC07812DCBF3] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UK_Groups_Code] UNIQUE NONCLUSTERED ([Code] ASC)
);



