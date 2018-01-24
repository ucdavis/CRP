CREATE TABLE [dbo].[Checks] (
    [id]            INT           IDENTITY (1, 1) NOT NULL,
    [Payee]         VARCHAR (200) NOT NULL,
    [CheckNumber]   INT           NOT NULL,
    [Amount]        MONEY         NOT NULL,
    [DateReceived]  DATE          NOT NULL,
    [TransactionId] INT           NOT NULL,
    [Notes]         VARCHAR (MAX) NULL,
    CONSTRAINT [PK_Checks] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_Checks_Transactions] FOREIGN KEY ([TransactionId]) REFERENCES [dbo].[Transactions] ([id])
);

