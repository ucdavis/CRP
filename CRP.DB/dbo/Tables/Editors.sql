CREATE TABLE [dbo].[Editors] (
    [id]     INT IDENTITY (1, 1) NOT NULL,
    [ItemId] INT NOT NULL,
    [UserId] INT NOT NULL,
    [Owner]  BIT CONSTRAINT [DF_Editors_Owner] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Editors] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_Editors_Items] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items] ([id])
);

