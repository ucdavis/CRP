CREATE TABLE [dbo].[PageTracking] (
    [id]        INT           IDENTITY (1, 1) NOT NULL,
    [LoginId]   VARCHAR (50)  NULL,
    [Location]  VARCHAR (500) NULL,
    [IPAddress] VARCHAR (20)  NULL,
    [DateTime]  DATETIME      CONSTRAINT [DF_PageTracking_DateTime] DEFAULT (getdate()) NOT NULL
);

