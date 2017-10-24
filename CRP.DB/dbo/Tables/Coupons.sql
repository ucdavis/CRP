CREATE TABLE [dbo].[Coupons] (
    [id]             INT           IDENTITY (1, 1) NOT NULL,
    [Code]           VARCHAR (10)  NOT NULL,
    [ItemId]         INT           NOT NULL,
    [Unlimited]      BIT           CONSTRAINT [DF_Coupons_Unlimited] DEFAULT ((0)) NOT NULL,
    [Expiration]     DATE          NULL,
    [Email]          VARCHAR (100) NULL,
    [Used]           BIT           CONSTRAINT [DF_Coupons_Used] DEFAULT ((0)) NOT NULL,
    [DiscountAmount] MONEY         NOT NULL,
    [UserId]         VARCHAR (50)  NOT NULL,
    [IsActive]       BIT           CONSTRAINT [DF_Coupons_IsActive] DEFAULT ((1)) NOT NULL,
    [MaxQuantity]    INT           NULL,
    [MaxUsage]       INT           NOT NULL,
    CONSTRAINT [PK_Coupons] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_Coupons_Items] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items] ([id])
);

