CREATE TABLE [dbo].[HelpTopics] (
    [id]                INT           IDENTITY (1, 1) NOT NULL,
    [Question]          VARCHAR (MAX) NOT NULL,
    [Answer]            VARCHAR (MAX) NULL,
    [AvailableToPublic] BIT           NOT NULL,
    [IsActive]          BIT           NOT NULL,
    [NumberOfReads]     INT           NULL,
    [IsVideo]           BIT           NULL,
    [VideoName]         VARCHAR (50)  NULL,
    CONSTRAINT [PK_HelpTopics] PRIMARY KEY CLUSTERED ([id] ASC)
);

