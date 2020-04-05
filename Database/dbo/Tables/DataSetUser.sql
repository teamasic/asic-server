CREATE TABLE [dbo].[DataSetUser] (
    [Id]        INT    IDENTITY (1, 1) NOT NULL,
    [DataSetId] INT    NULL,
    [UserId]    BIGINT NULL,
    CONSTRAINT [PK_DataSetUser] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DataSetUser_DataSet] FOREIGN KEY ([DataSetId]) REFERENCES [dbo].[DataSet] ([Id]),
    CONSTRAINT [FK_DataSetUser_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id])
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_DataSetUser]
    ON [dbo].[DataSetUser]([DataSetId] ASC, [UserId] ASC);

