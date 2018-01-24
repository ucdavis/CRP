CREATE TABLE [dbo].[ItemReports] (
    [id]             INT          IDENTITY (1, 1) NOT NULL,
    [Name]           VARCHAR (50) NOT NULL,
    [ItemId]         INT          NULL,
    [UserId]         INT          NOT NULL,
    [SystemReusable] BIT          CONSTRAINT [DF_ItemReports_SystemReusable] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ItemReports] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_ItemReports_Items] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items] ([id])
);

