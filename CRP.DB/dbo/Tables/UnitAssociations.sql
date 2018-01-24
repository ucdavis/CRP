CREATE TABLE [dbo].[UnitAssociations] (
    [UnitAssociationID] INT IDENTITY (1, 1) NOT NULL,
    [UnitID]            INT NOT NULL,
    [UserID]            INT NOT NULL,
    [ApplicationID]     INT NOT NULL,
    [Inactive]          BIT NOT NULL,
    CONSTRAINT [PK_UnitAssociations] PRIMARY KEY CLUSTERED ([UnitAssociationID] ASC),
    CONSTRAINT [FK_UnitAssociations_Applications] FOREIGN KEY ([ApplicationID]) REFERENCES [dbo].[Applications] ([ApplicationID]),
    CONSTRAINT [FK_UnitAssociations_Unit] FOREIGN KEY ([UnitID]) REFERENCES [dbo].[Unit] ([UnitID]),
    CONSTRAINT [FK_UnitAssociations_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([UserID])
);

