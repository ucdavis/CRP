CREATE TABLE [dbo].[QuestionSets] (
    [id]              INT          IDENTITY (1, 1) NOT NULL,
    [Name]            VARCHAR (50) NOT NULL,
    [CollegeReusable] BIT          CONSTRAINT [DF_QuestionSets_CollegeReusable] DEFAULT ((0)) NOT NULL,
    [SystemReusable]  BIT          CONSTRAINT [DF_QuestionSets_SystemReusable] DEFAULT ((0)) NOT NULL,
    [UserReusable]    BIT          CONSTRAINT [DF_QuestionSets_UserReusable] DEFAULT ((0)) NOT NULL,
    [UserId]          INT          NULL,
    [SchoolId]        VARCHAR (2)  NULL,
    [IsActive]        BIT          CONSTRAINT [DF_QuestionSets_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_QuestionSets] PRIMARY KEY CLUSTERED ([id] ASC)
);

