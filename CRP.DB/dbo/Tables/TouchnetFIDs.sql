CREATE TABLE [dbo].[TouchnetFIDs] (
    [id]          INT           IDENTITY (1, 1) NOT NULL,
    [FID]         CHAR (3)      NOT NULL,
    [Description] VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_TouchnetFIDs] PRIMARY KEY CLUSTERED ([id] ASC)
);

