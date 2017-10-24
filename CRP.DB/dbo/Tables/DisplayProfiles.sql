CREATE TABLE [dbo].[DisplayProfiles] (
    [id]           INT             IDENTITY (1, 1) NOT NULL,
    [Name]         VARCHAR (200)   NOT NULL,
    [UnitId]       INT             NULL,
    [SchoolId]     VARCHAR (2)     NULL,
    [Logo]         VARBINARY (MAX) NULL,
    [SchoolMaster] BIT             CONSTRAINT [DF_DisplayProfiles_CollegeMaster] DEFAULT ((0)) NOT NULL,
    [CustomCss]    VARCHAR (MAX)   NULL,
    CONSTRAINT [PK_Profiles] PRIMARY KEY CLUSTERED ([id] ASC)
);

