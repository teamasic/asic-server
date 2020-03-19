CREATE TABLE [dbo].[User] (
    [Id]           BIGINT          IDENTITY (1, 1) NOT NULL,
    [Username]     VARCHAR (255)   NOT NULL,
    [RollNumber]   VARCHAR (50)    NULL,
    [PasswordHash] VARBINARY (MAX) NULL,
    [PasswordSalt] VARBINARY (MAX) NULL,
    [Fullname]     NVARCHAR (255)  NOT NULL,
    [PhoneNumber]  VARCHAR (255)   NULL,
    [Email]        VARCHAR (255)   NULL,
    [Address]      NVARCHAR (255)  NULL,
    [Birthdate]    DATETIME        NULL,
    CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [IX_Account] UNIQUE NONCLUSTERED ([Username] ASC),
    CONSTRAINT [UK_RollNumber_Account] UNIQUE NONCLUSTERED ([RollNumber] ASC)
);



