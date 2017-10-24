CREATE TABLE [dbo].[ApplicationRoles] (
    [ApplicationRoleID] INT IDENTITY (1, 1) NOT NULL,
    [ApplicationID]     INT NOT NULL,
    [RoleID]            INT NOT NULL,
    [Level]             INT NULL,
    CONSTRAINT [PK_ApplicationRoles] PRIMARY KEY CLUSTERED ([ApplicationRoleID] ASC),
    CONSTRAINT [FK_ApplicationRoles_Applications] FOREIGN KEY ([ApplicationID]) REFERENCES [dbo].[Applications] ([ApplicationID]),
    CONSTRAINT [FK_ApplicationRoles_Roles] FOREIGN KEY ([RoleID]) REFERENCES [dbo].[Roles] ([RoleID])
);

