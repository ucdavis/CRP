CREATE TABLE [dbo].[Applications] (
    [ApplicationID]  INT            IDENTITY (1, 1) NOT NULL,
    [Name]           NVARCHAR (50)  NOT NULL,
    [Abbr]           NVARCHAR (50)  NULL,
    [Location]       NVARCHAR (256) NULL,
    [IconLocation]   NVARCHAR (256) NULL,
    [Inactive]       BIT            CONSTRAINT [DF_Applications_Inactive] DEFAULT ((0)) NOT NULL,
    [WebServiceHash] NVARCHAR (100) NULL,
    [Salt]           NVARCHAR (20)  NULL,
    CONSTRAINT [PK_Applications] PRIMARY KEY CLUSTERED ([ApplicationID] ASC)
);

