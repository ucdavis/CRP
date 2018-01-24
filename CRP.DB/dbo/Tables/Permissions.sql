CREATE TABLE [dbo].[Permissions] (
    [PermissionID]  INT IDENTITY (1, 1) NOT NULL,
    [UserID]        INT NOT NULL,
    [ApplicationID] INT NULL,
    [RoleID]        INT NULL,
    [Inactive]      BIT CONSTRAINT [DF_Permissions_Inactive] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Permissions] PRIMARY KEY CLUSTERED ([PermissionID] ASC),
    CONSTRAINT [FK_Permissions_Applications] FOREIGN KEY ([ApplicationID]) REFERENCES [dbo].[Applications] ([ApplicationID]),
    CONSTRAINT [FK_Permissions_Roles] FOREIGN KEY ([RoleID]) REFERENCES [dbo].[Roles] ([RoleID]),
    CONSTRAINT [FK_Permissions_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([UserID])
);

