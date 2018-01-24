CREATE TABLE [dbo].[ExtendedProperties] (
    [id]             INT           IDENTITY (1, 1) NOT NULL,
    [ItemTypeId]     INT           NOT NULL,
    [Name]           VARCHAR (100) NOT NULL,
    [QuestionTypeId] INT           NOT NULL,
    [Order]          INT           NOT NULL,
    CONSTRAINT [PK_ExtendedProperties] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_ExtendedProperties_ItemTypes] FOREIGN KEY ([ItemTypeId]) REFERENCES [dbo].[ItemTypes] ([id]),
    CONSTRAINT [FK_ExtendedProperties_QuestionTypes] FOREIGN KEY ([QuestionTypeId]) REFERENCES [dbo].[QuestionTypes] ([id])
);

