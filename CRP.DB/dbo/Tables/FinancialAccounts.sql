CREATE TABLE [dbo].[FinancialAccounts] (
    [id]          INT           IDENTITY (1, 1) NOT NULL,
    [Account]     NVARCHAR (7)   NOT NULL,
    [Chart]       NVARCHAR (1)   NOT NULL,
    [Description] NVARCHAR (MAX) NULL,
    [Name]        NVARCHAR (128) NOT NULL,
    [Project]     NVARCHAR (9)   NULL,
    [SubAccount]  NVARCHAR (5)   NULL,
    [UnitId]      INT            NOT NULL,
    CONSTRAINT [PK_FinancialAccounts] PRIMARY KEY CLUSTERED ([id] ASC)
);

