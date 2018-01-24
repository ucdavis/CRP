CREATE TABLE [dbo].[OpenIdUsers] (
    [id]            VARCHAR (255) NOT NULL,
    [Email]         VARCHAR (255) NULL,
    [FirstName]     VARCHAR (255) NULL,
    [LastName]      VARCHAR (255) NULL,
    [StreetAddress] VARCHAR (255) NULL,
    [Address2]      VARCHAR (255) NULL,
    [City]          VARCHAR (255) NULL,
    [State]         VARCHAR (50)  NULL,
    [Zip]           VARCHAR (10)  NULL,
    [PhoneNumber]   VARCHAR (20)  NULL,
    CONSTRAINT [PK_OpenIdUsers] PRIMARY KEY CLUSTERED ([id] ASC)
);

