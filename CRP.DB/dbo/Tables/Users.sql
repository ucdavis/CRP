CREATE TABLE [dbo].[Users] (
    [UserID]     INT              IDENTITY (1, 1) NOT NULL,
    [LoginID]    VARCHAR (10)     NOT NULL,
    [Email]      NVARCHAR (50)    NOT NULL,
    [Phone]      VARCHAR (50)     NULL,
    [FirstName]  NVARCHAR (50)    NULL,
    [LastName]   NVARCHAR (50)    NOT NULL,
    [EmployeeID] VARCHAR (9)      NULL,
    [StudentID]  VARCHAR (9)      NULL,
    [UserImage]  NVARCHAR (50)    NULL,
    [SID]        NVARCHAR (50)    NULL,
    [Inactive]   BIT              CONSTRAINT [DF_Users_Inactive] DEFAULT ((0)) NOT NULL,
    [UserKey]    UNIQUEIDENTIFIER CONSTRAINT [DF_Users_UserKey] DEFAULT (newid()) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserID] ASC)
);

