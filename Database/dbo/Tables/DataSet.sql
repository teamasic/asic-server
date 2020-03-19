CREATE TABLE [dbo].[DataSet] (
    [Id]   INT            IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (100) NOT NULL,
    CONSTRAINT [PK_DataSet] PRIMARY KEY CLUSTERED ([Id] ASC)
);

