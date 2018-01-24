CREATE TABLE [dbo].[AccessTokens] (
    [ID]            INT           IDENTITY (1, 1) NOT NULL,
    [Token]         CHAR (32)     NOT NULL,
    [ApplicationID] INT           NOT NULL,
    [ContactEmail]  VARCHAR (50)  NOT NULL,
    [Reason]        VARCHAR (MAX) NULL,
    [Active]        BIT           NOT NULL
);

