CREATE TABLE [dbo].[User] (
    [Code]     VARCHAR (50)  NOT NULL,
    [Email]    VARCHAR (50)  NOT NULL,
    [Fullname] NVARCHAR (50) NOT NULL,
    [Image]    VARCHAR (MAX) NULL,
    [RoleId]   INT           NOT NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Code] ASC),
    CONSTRAINT [FK_User_Role] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([Id]),
    CONSTRAINT [UK_User_Email] UNIQUE NONCLUSTERED ([Email] ASC)
);











