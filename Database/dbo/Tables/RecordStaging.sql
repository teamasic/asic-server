CREATE TABLE [dbo].[RecordStaging] (
    [Id]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [AttendeeCode]     VARCHAR (50)   NULL,
    [AttendeeName]     NVARCHAR (255) NULL,
    [SessionName]      NVARCHAR (50)  NULL,
    [SessionStartTime] DATETIME       NULL,
    [SessionEndTime]   DATETIME       NULL,
    [RtspString]       VARCHAR (255)  NULL,
    [RoomName]         VARCHAR (50)   NULL,
    [GroupCode]        NVARCHAR (50)  NULL,
    [GroupName]        NVARCHAR (255) NULL,
    [GroupCreateTime]  DATETIME       NULL,
    [MaxSessionCount]  INT            NULL,
    [Present]          BIT            NULL,
    [IsEnrollInClass]  BIT            NULL,
    CONSTRAINT [PK_RecordStaging] PRIMARY KEY CLUSTERED ([Id] ASC)
);



