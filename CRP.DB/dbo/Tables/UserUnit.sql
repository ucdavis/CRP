CREATE TABLE [dbo].[UserUnit] (
    [UserID] INT NOT NULL,
    [UnitID] INT NOT NULL,
    CONSTRAINT [PK_UserUnit] PRIMARY KEY CLUSTERED ([UserID] ASC, [UnitID] ASC),
    CONSTRAINT [FK_UserUnit_Unit] FOREIGN KEY ([UnitID]) REFERENCES [dbo].[Unit] ([UnitID]),
    CONSTRAINT [FK_UserUnit_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([UserID])
);

