CREATE TABLE [dbo].[ItemReportColumns] (
    [id]            INT           IDENTITY (1, 1) NOT NULL,
    [ItemReportId]  INT           NOT NULL,
    [Order]         INT           NOT NULL,
    [Format]        VARCHAR (50)  NULL,
    [Name]          VARCHAR (200) NOT NULL,
    [Quantity]      BIT           CONSTRAINT [DF_ItemReportColumns_Quantity] DEFAULT ((0)) NOT NULL,
    [Transaction]   BIT           CONSTRAINT [DF_ItemReportColumns_Transaction] DEFAULT ((0)) NOT NULL,
    [Property]      BIT           CONSTRAINT [DF_ItemReportColumns_Property] DEFAULT ((0)) NOT NULL,
    [QuestionSetId] INT           NULL,
    CONSTRAINT [PK_ItemReportColumns] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_ItemReportColumns_ItemReports] FOREIGN KEY ([ItemReportId]) REFERENCES [dbo].[ItemReports] ([id])
);

