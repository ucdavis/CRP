CREATE FULLTEXT INDEX ON [dbo].[Tags]
    ([Name] LANGUAGE 1033)
    KEY INDEX [PK_Tags]
    ON [Tags];


GO
CREATE FULLTEXT INDEX ON [dbo].[Items]
    ([Name] LANGUAGE 1033, [Description] LANGUAGE 1033)
    KEY INDEX [PK_Items]
    ON [Items];


GO
CREATE FULLTEXT INDEX ON [dbo].[ExtendedPropertyAnswers]
    ([Answer] LANGUAGE 1033)
    KEY INDEX [PK_ExtendedPropertyAnswers]
    ON [ExtendedPropertyAnswers];

