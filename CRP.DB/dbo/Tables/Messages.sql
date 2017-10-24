CREATE TABLE [dbo].[Messages] (
    [ID]               INT           IDENTITY (1, 1) NOT NULL,
    [Message]          VARCHAR (MAX) NOT NULL,
    [ApplicationID]    INT           NULL,
    [BeginDisplayDate] DATE          NOT NULL,
    [EndDisplayDate]   DATE          NULL,
    [Critical]         BIT           NOT NULL,
    [IsActive]         BIT           CONSTRAINT [DF_Messages_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Messages] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Messages_Applications] FOREIGN KEY ([ApplicationID]) REFERENCES [dbo].[Applications] ([ApplicationID])
);

