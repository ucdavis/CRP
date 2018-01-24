﻿CREATE TABLE [dbo].[Items] (
    [id]                       INT             IDENTITY (1, 1) NOT NULL,
    [Name]                     VARCHAR (200)   NOT NULL,
    [Summary]                  VARCHAR (MAX)   NULL,
    [Description]              VARCHAR (MAX)   NULL,
    [CostPerItem]              MONEY           NOT NULL,
    [Quantity]                 INT             CONSTRAINT [DF_Items_Quantity] DEFAULT ((0)) NOT NULL,
    [QuantityName]             VARCHAR (50)    NULL,
    [Expiration]               DATE            NULL,
    [Image]                    VARBINARY (MAX) NULL,
    [Link]                     VARCHAR (200)   NULL,
    [ItemTypeId]               INT             NOT NULL,
    [UnitId]                   INT             NOT NULL,
    [DateCreated]              DATETIME        CONSTRAINT [DF_Items_DateCreated] DEFAULT (getdate()) NOT NULL,
    [Available]                BIT             CONSTRAINT [DF_Items_Available] DEFAULT ((0)) NOT NULL,
    [Private]                  BIT             CONSTRAINT [DF_Items_Private] DEFAULT ((0)) NOT NULL,
    [RestrictedKey]            VARCHAR (10)    NULL,
    [MapLink]                  VARCHAR (MAX)   NULL,
    [LinkLink]                 VARCHAR (MAX)   NULL,
    [CheckPaymentInstructions] VARCHAR (MAX)   NULL,
    [AllowCheckPayment]        BIT             CONSTRAINT [DF_Items_AllowCheckPayment] DEFAULT ((1)) NOT NULL,
    [AllowCreditPayment]       BIT             CONSTRAINT [DF_Items_AllowCreditPayment] DEFAULT ((1)) NOT NULL,
    [HideDonation]             BIT             CONSTRAINT [DF_Items_HideDonation] DEFAULT ((0)) NOT NULL,
    [TouchnetFID]              CHAR (3)        NULL,
    [DonationLinkLegend]       VARCHAR (50)    NULL,
    [DonationLinkInformation]  VARCHAR (500)   NULL,
    [DonationLinkText]         VARCHAR (50)    NULL,
    [DonationLinkLink]         VARCHAR (200)   NULL,
    CONSTRAINT [PK_Items] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_Items_ItemTypes] FOREIGN KEY ([ItemTypeId]) REFERENCES [dbo].[ItemTypes] ([id])
);

