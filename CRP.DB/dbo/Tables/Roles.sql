CREATE TABLE [dbo].[Roles] (
    [RoleID]   INT           IDENTITY (1, 1) NOT NULL,
    [Role]     NVARCHAR (50) NOT NULL,
    [Inactive] BIT           CONSTRAINT [DF_Roles_Inactive] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([RoleID] ASC)
);

