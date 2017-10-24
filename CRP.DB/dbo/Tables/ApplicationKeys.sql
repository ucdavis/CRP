CREATE TABLE [dbo].[ApplicationKeys] (
    [id]          INT           IDENTITY (1, 1) NOT NULL,
    [Application] VARCHAR (100) NOT NULL,
    [Key]         VARCHAR (50)  NOT NULL,
    [IsActive]    BIT           CONSTRAINT [DF_ApplicationKeys_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_ApplicationKeys] PRIMARY KEY CLUSTERED ([id] ASC)
);

