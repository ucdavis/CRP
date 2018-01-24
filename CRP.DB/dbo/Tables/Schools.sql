CREATE TABLE [dbo].[Schools] (
    [SchoolCode]       VARCHAR (2)  NOT NULL,
    [ShortDescription] VARCHAR (25) NOT NULL,
    [LongDescription]  VARCHAR (50) NOT NULL,
    [Abbreviation]     VARCHAR (12) NOT NULL,
    CONSTRAINT [PK_Schools] PRIMARY KEY CLUSTERED ([SchoolCode] ASC)
);

