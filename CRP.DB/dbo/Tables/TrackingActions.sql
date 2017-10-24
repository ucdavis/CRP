CREATE TABLE [dbo].[TrackingActions] (
    [TrackingActionID] INT          IDENTITY (1, 1) NOT NULL,
    [TrackingAction]   VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_TrackingActions] PRIMARY KEY CLUSTERED ([TrackingActionID] ASC)
);

