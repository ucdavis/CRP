CREATE TABLE [dbo].[ItemsXTags] (
    [ItemId] INT NOT NULL,
    [TagId]  INT NOT NULL,
    CONSTRAINT [PK_ItemsXTags] PRIMARY KEY CLUSTERED ([ItemId] ASC, [TagId] ASC),
    CONSTRAINT [FK_ItemsXTags_Items] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items] ([id]),
    CONSTRAINT [FK_ItemsXTags_Tags] FOREIGN KEY ([TagId]) REFERENCES [dbo].[Tags] ([id])
);

