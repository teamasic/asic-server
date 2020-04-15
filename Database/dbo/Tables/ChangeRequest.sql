CREATE TABLE [dbo].[ChangeRequest] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [RecordId] INT            NOT NULL,
    [Comment]  NVARCHAR (255) NULL,
    [Status]   INT            NOT NULL,
    CONSTRAINT [PK__ChangeRequests] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ChangeRequests_Records] FOREIGN KEY ([RecordId]) REFERENCES [dbo].[Record] ([Id]),
    CONSTRAINT [UK_ChangeRequests_RecordId] UNIQUE NONCLUSTERED ([RecordId] ASC)
);

