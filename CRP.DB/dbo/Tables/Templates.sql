CREATE TABLE [dbo].[Templates] (
    [id]      INT           IDENTITY (1, 1) NOT NULL,
    [Text]    VARCHAR (MAX) NOT NULL,
    [ItemId]  INT           NULL,
    [Default] BIT           CONSTRAINT [DF_Templates_Default] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Templates] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_Templates_Items] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items] ([id])
);

