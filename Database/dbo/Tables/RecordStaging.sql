CREATE TABLE [dbo].[RecordStaging] (
    [Id]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [AttendeeCode]     VARCHAR (50)   NULL,
    [SessionName]      NVARCHAR (50)  NULL,
    [SessionStartTime] DATETIME       NULL,
    [SessionEndTime]   DATETIME       NULL,
    [RoomId]           INT            NULL,
    [GroupCode]        NVARCHAR (50)  NULL,
    [GroupName]        NVARCHAR (255) NULL,
    [GroupCreateTime]  DATETIME       NULL,
    [TotalSession]     INT            NULL,
    [Present]          BIT            NULL,
    [IsEnrollInClass]  BIT            NULL,
    CONSTRAINT [PK_RecordStaging] PRIMARY KEY CLUSTERED ([Id] ASC)
);





