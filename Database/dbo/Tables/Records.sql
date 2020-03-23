CREATE TABLE [dbo].[Records] (
    [Id]         BIGINT IDENTITY (1, 1) NOT NULL,
    [AttendeeId] BIGINT NULL,
    [SessionId]  INT    NULL,
    [Present]    INT    NOT NULL,
    CONSTRAINT [PK__Records__3214EC07F9AE9C1D] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Records_0_0] FOREIGN KEY ([SessionId]) REFERENCES [dbo].[Sessions] ([Id]),
    CONSTRAINT [FK_Records_User] FOREIGN KEY ([AttendeeId]) REFERENCES [dbo].[User] ([Id])
);

