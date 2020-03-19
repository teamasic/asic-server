CREATE TABLE [dbo].[AttendeeDataSet] (
    [Id]         INT NOT NULL,
    [AttendeeId] INT NOT NULL,
    [DataSetId]  INT NOT NULL,
    CONSTRAINT [PK_AttendeeDataSet] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_AttendeeDataSet]
    ON [dbo].[AttendeeDataSet]([DataSetId] ASC, [AttendeeId] ASC);

