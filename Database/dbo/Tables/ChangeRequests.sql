CREATE TABLE [dbo].[ChangeRequests] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [RecordId] BIGINT         NULL,
    [Comment]  NVARCHAR (255) NULL,
    [Status]   INT            NOT NULL,
    CONSTRAINT [PK__ChangeRe__3214EC0796B58FEC] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ChangeRequests_0_0] FOREIGN KEY ([RecordId]) REFERENCES [dbo].[Records] ([Id])
);

