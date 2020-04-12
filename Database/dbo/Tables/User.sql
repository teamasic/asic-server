CREATE TABLE [dbo].[User] (
    [Id]         BIGINT         IDENTITY (1, 1) NOT NULL,
    [Username]   VARCHAR (255)  NOT NULL,
    [RollNumber] VARCHAR (50)   NULL,
    [Fullname]   NVARCHAR (255) NOT NULL,
    [Email]      VARCHAR (255)  NULL,
    [Image]      VARCHAR (MAX)  NULL,
    [RoleId]     INT            NOT NULL,
    CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_User_Role] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([Id]),
    CONSTRAINT [IX_Account] UNIQUE NONCLUSTERED ([Username] ASC),
    CONSTRAINT [UK_RollNumber_Account] UNIQUE NONCLUSTERED ([RollNumber] ASC)
);







