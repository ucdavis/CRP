CREATE TABLE [dbo].[ItemTypes] (
    [id]       INT          IDENTITY (1, 1) NOT NULL,
    [Name]     VARCHAR (50) NOT NULL,
    [IsActive] BIT          CONSTRAINT [DF_ItemTypes_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_ItemTypes] PRIMARY KEY CLUSTERED ([id] ASC)
);

