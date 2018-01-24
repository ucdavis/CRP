CREATE TABLE [dbo].[Unit] (
    [UnitID]     INT           IDENTITY (1, 1) NOT NULL,
    [FullName]   NVARCHAR (50) NOT NULL,
    [ShortName]  NVARCHAR (50) NOT NULL,
    [PPS_Code]   CHAR (6)      NULL,
    [FIS_Code]   CHAR (4)      NOT NULL,
    [SchoolCode] VARCHAR (2)   NOT NULL,
    [ParentID]   INT           NULL,
    [Type]       VARCHAR (10)  NULL,
    CONSTRAINT [PK_Unit] PRIMARY KEY CLUSTERED ([UnitID] ASC),
    CONSTRAINT [FK_Unit_Schools] FOREIGN KEY ([SchoolCode]) REFERENCES [dbo].[Schools] ([SchoolCode]),
    CONSTRAINT [FK_Unit_Unit_Parent] FOREIGN KEY ([ParentID]) REFERENCES [dbo].[Unit] ([UnitID])
);

