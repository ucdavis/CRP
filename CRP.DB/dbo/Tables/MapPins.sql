CREATE TABLE [dbo].[MapPins] (
    [id]          INT           IDENTITY (1, 1) NOT NULL,
    [ItemId]      INT           NOT NULL,
    [IsPrimary]   BIT           NOT NULL,
    [Latitude]    VARCHAR (50)  NOT NULL,
    [Longitude]   VARCHAR (50)  NOT NULL,
    [Title]       VARCHAR (50)  NULL,
    [Description] VARCHAR (100) NULL,
    CONSTRAINT [PK_MapPins] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_MapPins_Items] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items] ([id])
);

