CREATE TABLE [dbo].[User] (
    [Id]           BIGINT          IDENTITY (1, 1) NOT NULL,
    [Username]     VARCHAR (255)   NOT NULL,
    [PasswordHash] VARBINARY (MAX) NOT NULL,
    [PasswordSalt] VARBINARY (MAX) NOT NULL,
    [Fullname]     NVARCHAR (255)  NOT NULL,
    [PhoneNumber]  VARCHAR (255)   NULL,
    [Email]        VARCHAR (255)   NULL,
    [Address]      NVARCHAR (255)  NULL,
    [Birthdate]    DATETIME        NULL,
    CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [IX_Account] UNIQUE NONCLUSTERED ([Username] ASC)
);

