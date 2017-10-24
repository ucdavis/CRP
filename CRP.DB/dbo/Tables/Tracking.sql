CREATE TABLE [dbo].[Tracking] (
    [TrackingID]         INT            IDENTITY (1, 1) NOT NULL,
    [TrackingTypeID]     INT            NOT NULL,
    [TrackingActionID]   INT            NOT NULL,
    [TrackingUserName]   VARCHAR (10)   NOT NULL,
    [TrackingActionDate] DATETIME       NOT NULL,
    [Comments]           NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Tracking] PRIMARY KEY CLUSTERED ([TrackingID] ASC),
    CONSTRAINT [FK_Tracking_TrackingActions] FOREIGN KEY ([TrackingActionID]) REFERENCES [dbo].[TrackingActions] ([TrackingActionID]),
    CONSTRAINT [FK_Tracking_TrackingTypes] FOREIGN KEY ([TrackingTypeID]) REFERENCES [dbo].[TrackingTypes] ([TrackingTypeID])
);

