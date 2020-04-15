CREATE TABLE [dbo].[Group] (
    [Code]            VARCHAR (50)   NOT NULL,
    [Name]            NVARCHAR (100) NULL,
    [DateTimeCreated] DATETIME       CONSTRAINT [DF_Groups_DateTimeCreated] DEFAULT (getdate()) NULL,
    [TotalSession]    INT            NULL,
    [Deleted]         BIT            CONSTRAINT [DF_Groups_Deleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Groups] PRIMARY KEY CLUSTERED ([Code] ASC)
);

