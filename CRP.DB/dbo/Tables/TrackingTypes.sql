CREATE TABLE [dbo].[TrackingTypes] (
    [TrackingTypeID] INT          IDENTITY (1, 1) NOT NULL,
    [TrackingType]   VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_TrackingType] PRIMARY KEY CLUSTERED ([TrackingTypeID] ASC)
);

