﻿CREATE TABLE [dbo].[PaymentLogs] (
    [id]                   INT           IDENTITY (1, 1) NOT NULL,
    [Name]                 VARCHAR (MAX) NULL,
    [Amount]               MONEY         NOT NULL,
    [DatePayment]          DATETIME      CONSTRAINT [DF_PaymentLogs_DatePayment] DEFAULT (getdate()) NOT NULL,
    [TransactionId]        INT           NOT NULL,
    [CheckNumber]          INT           NULL,
    [GatewayTransactionId] VARCHAR (MAX) NULL,
    [CardType]             VARCHAR (MAX) NULL,
    [Accepted]             BIT           CONSTRAINT [DF_PaymentLogs_Accepted] DEFAULT ((0)) NOT NULL,
    [Check]                BIT           CONSTRAINT [DF_PaymentLogs_Check] DEFAULT ((0)) NOT NULL,
    [Credit]               BIT           CONSTRAINT [DF_PaymentLogs_Credit] DEFAULT ((0)) NOT NULL,
    [Notes]                VARCHAR (MAX) NULL,
    [TnStatus]             NCHAR (1)     NULL,
    [TnPaymentDate]        VARCHAR (MAX) NULL,
    [TnSysTrackingId]      VARCHAR (MAX) NULL,
    [TnBillingAddress1]    VARCHAR (MAX) NULL,
    [TnBillingAddress2]    VARCHAR (MAX) NULL,
    [TnBillingCity]        VARCHAR (MAX) NULL,
    [TnBillingState]       VARCHAR (MAX) NULL,
    [TnBillingZip]         VARCHAR (MAX) NULL,
    [TnUpaySiteId]         VARCHAR (MAX) NULL,
    [TnErrorLink]          VARCHAR (MAX) NULL,
    [TnSubmit]             VARCHAR (MAX) NULL,
    [TnSuccessLink]        VARCHAR (MAX) NULL,
    [TnCancelLink]         VARCHAR (MAX) NULL,
    CONSTRAINT [PK_PaymentLogs] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_PaymentLogs_Transactions] FOREIGN KEY ([TransactionId]) REFERENCES [dbo].[Transactions] ([id])
);

