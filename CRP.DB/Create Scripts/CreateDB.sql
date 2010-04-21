USE [CRP]
GO
/****** Object:  Table [dbo].[DisplayProfiles]    Script Date: 01/06/2010 07:31:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DisplayProfiles](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[UnitId] [int] NOT NULL,
	[Logo] [varbinary](max) NULL,
 CONSTRAINT [PK_Profiles] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Checks]    Script Date: 01/06/2010 07:31:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Checks](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Payee] [varchar](200) NOT NULL,
	[CheckNumber] [int] NOT NULL,
	[Amount] [money] NOT NULL,
	[DateReceived] [date] NOT NULL,
 CONSTRAINT [PK_Checks] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Tags]    Script Date: 01/06/2010 07:31:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Tags](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Tags] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[QuestionTypes]    Script Date: 01/06/2010 07:31:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[QuestionTypes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[HasOptions] [bit] NOT NULL,
	[ExtendedProperty] [bit] NULL,
 CONSTRAINT [PK_QuestionTypes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[QuestionSets]    Script Date: 01/06/2010 07:31:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[QuestionSets](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[CollegeReusable] [bit] NOT NULL,
	[SystemReusable] [bit] NOT NULL,
	[UserReusable] [bit] NOT NULL,
	[UserId] [int] NULL,
	[SchoolId] [varchar](2) NULL,
 CONSTRAINT [PK_QuestionSets] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ItemTypes]    Script Date: 01/06/2010 07:31:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ItemTypes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ItemTypes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ItemTypesXQuestionSets]    Script Date: 01/06/2010 07:31:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemTypesXQuestionSets](
	[ItemTypeId] [int] NOT NULL,
	[QuestionSetId] [int] NOT NULL,
	[Order] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vUsers]    Script Date: 01/06/2010 07:31:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vUsers]
AS
SELECT     UserID AS id, LoginID, Email, Phone, FirstName, LastName, EmployeeID, SID, UserKey
FROM         Catbert3.dbo.Users
WHERE     (Inactive = 0)
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Users (Catbert3.dbo)"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 280
               Right = 198
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 13
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vUsers'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vUsers'
GO
/****** Object:  View [dbo].[vUnits]    Script Date: 01/06/2010 07:31:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vUnits]
AS
SELECT     UnitID AS id, FullName, ShortName, PPS_Code, FIS_Code, SchoolCode
FROM         Catbert3.dbo.Unit
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Unit (Catbert3.dbo)"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 249
               Right = 198
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vUnits'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vUnits'
GO
/****** Object:  View [dbo].[vSchools]    Script Date: 01/06/2010 07:31:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vSchools]
AS
SELECT     SchoolCode AS id, ShortDescription, LongDescription, Abbreviation
FROM         Catbert3.dbo.Schools
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Schools (Catbert3.dbo)"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 125
               Right = 206
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vSchools'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vSchools'
GO
/****** Object:  Table [dbo].[Questions]    Script Date: 01/06/2010 07:31:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Questions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[ShortName] [varchar](50) NULL,
	[QuestionTypeId] [int] NOT NULL,
	[CollegeReusable] [bit] NOT NULL,
	[SystemReusable] [bit] NOT NULL,
	[UserReusable] [bit] NOT NULL,
	[UserId] [int] NULL,
	[SchoolId] [varchar](2) NULL,
 CONSTRAINT [PK_Questions] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ExtendedProperties]    Script Date: 01/06/2010 07:31:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ExtendedProperties](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ItemTypeId] [int] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[QuestionTypeId] [int] NOT NULL,
	[Order] [int] NOT NULL,
 CONSTRAINT [PK_ExtendedProperties] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Items]    Script Date: 01/06/2010 07:31:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Items](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[Description] [varchar](max) NULL,
	[CostPerItem] [money] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Expiration] [date] NULL,
	[Image] [varbinary](max) NULL,
	[Link] [varchar](200) NULL,
	[ItemTypeId] [int] NOT NULL,
	[UnitId] [int] NOT NULL,
 CONSTRAINT [PK_Items] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ItemQuestionSets]    Script Date: 01/06/2010 07:31:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemQuestionSets](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ItemId] [int] NOT NULL,
	[QuestionSetId] [int] NOT NULL,
	[TransactionLevel] [bit] NOT NULL,
	[QuantityLevel] [bit] NOT NULL,
	[Order] [int] NOT NULL,
	[Required] [bit] NOT NULL,
 CONSTRAINT [PK_ItemQuestionSets] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ExtendedPropertyAnswers]    Script Date: 01/06/2010 07:31:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ExtendedPropertyAnswers](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Answer] [varchar](max) NOT NULL,
	[ItemId] [int] NOT NULL,
	[ExtendedPropertyId] [int] NOT NULL,
 CONSTRAINT [PK_ExtendedPropertyAnswers] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Editors]    Script Date: 01/06/2010 07:31:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Editors](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ItemId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[Owner] [bit] NOT NULL,
 CONSTRAINT [PK_Editors] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Coupons]    Script Date: 01/06/2010 07:31:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Coupons](
	[id] [int] NOT NULL,
	[Code] [varchar](10) NOT NULL,
	[ItemId] [int] NOT NULL,
	[Unlimited] [bit] NOT NULL,
	[Expiration] [date] NULL,
	[Email] [varchar](100) NULL,
	[Used] [bit] NOT NULL,
	[DiscountAmount] [money] NOT NULL,
	[UserId] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Coupons] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[QuestionSetQuestions]    Script Date: 01/06/2010 07:31:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuestionSetQuestions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[QuestionId] [int] NOT NULL,
	[QuestionSetId] [int] NOT NULL,
	[Order] [int] NOT NULL,
 CONSTRAINT [PK_QuestionsXQuestionSets] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ItemsXTags]    Script Date: 01/06/2010 07:31:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemsXTags](
	[ItemId] [int] NOT NULL,
	[TagId] [int] NOT NULL,
 CONSTRAINT [PK_ItemsXTags] PRIMARY KEY CLUSTERED 
(
	[ItemId] ASC,
	[TagId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[QuestionOptions]    Script Date: 01/06/2010 07:31:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[QuestionOptions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[QuestionId] [int] NOT NULL,
 CONSTRAINT [PK_QuestionOptions] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Transactions]    Script Date: 01/06/2010 07:31:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Transactions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ItemId] [int] NOT NULL,
	[TransactionDate] [datetime] NOT NULL,
	[PaymentConfirmation] [varchar](100) NULL,
	[Credit] [bit] NOT NULL,
	[Check] [bit] NOT NULL,
	[Paid] [bit] NOT NULL,
	[Amount] [money] NOT NULL,
	[Donation] [bit] NOT NULL,
	[CouponId] [int] NULL,
 CONSTRAINT [PK_Transactions] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TransactionAnswers]    Script Date: 01/06/2010 07:31:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TransactionAnswers](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[TransactionId] [int] NOT NULL,
	[QuestionSetId] [int] NOT NULL,
	[QuestionId] [int] NOT NULL,
	[Answer] [varchar](max) NOT NULL,
 CONSTRAINT [PK_TransactionAnswers] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[QuantityAnswers]    Script Date: 01/06/2010 07:31:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[QuantityAnswers](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[TransactionId] [int] NOT NULL,
	[QuestionSetId] [int] NOT NULL,
	[QuestionId] [int] NOT NULL,
	[QuantityId] [uniqueidentifier] NOT NULL,
	[Answer] [varchar](max) NOT NULL,
 CONSTRAINT [PK_QuantityAnswers] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ChecksXTransactions]    Script Date: 01/06/2010 07:31:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChecksXTransactions](
	[CheckId] [int] NOT NULL,
	[TransactionId] [int] NOT NULL,
 CONSTRAINT [PK_ChecksXTransactions] PRIMARY KEY CLUSTERED 
(
	[CheckId] ASC,
	[TransactionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Default [DF_Coupons_Unlimited]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[Coupons] ADD  CONSTRAINT [DF_Coupons_Unlimited]  DEFAULT ((0)) FOR [Unlimited]
GO
/****** Object:  Default [DF_Coupons_Used]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[Coupons] ADD  CONSTRAINT [DF_Coupons_Used]  DEFAULT ((0)) FOR [Used]
GO
/****** Object:  Default [DF_Editors_Owner]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[Editors] ADD  CONSTRAINT [DF_Editors_Owner]  DEFAULT ((0)) FOR [Owner]
GO
/****** Object:  Default [DF_ItemQuestionSets_TransactionLevel]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[ItemQuestionSets] ADD  CONSTRAINT [DF_ItemQuestionSets_TransactionLevel]  DEFAULT ((0)) FOR [TransactionLevel]
GO
/****** Object:  Default [DF_ItemQuestionSets_QuantityLevel]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[ItemQuestionSets] ADD  CONSTRAINT [DF_ItemQuestionSets_QuantityLevel]  DEFAULT ((0)) FOR [QuantityLevel]
GO
/****** Object:  Default [DF_ItemQuestionSets_Required]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[ItemQuestionSets] ADD  CONSTRAINT [DF_ItemQuestionSets_Required]  DEFAULT ((0)) FOR [Required]
GO
/****** Object:  Default [DF_Items_Quantity]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[Items] ADD  CONSTRAINT [DF_Items_Quantity]  DEFAULT ((0)) FOR [Quantity]
GO
/****** Object:  Default [DF_ItemTypes_IsActive]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[ItemTypes] ADD  CONSTRAINT [DF_ItemTypes_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
/****** Object:  Default [DF_Questions_CollegeReusable]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[Questions] ADD  CONSTRAINT [DF_Questions_CollegeReusable]  DEFAULT ((0)) FOR [CollegeReusable]
GO
/****** Object:  Default [DF_Questions_SystemReusable]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[Questions] ADD  CONSTRAINT [DF_Questions_SystemReusable]  DEFAULT ((0)) FOR [SystemReusable]
GO
/****** Object:  Default [DF_Questions_UserReusable]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[Questions] ADD  CONSTRAINT [DF_Questions_UserReusable]  DEFAULT ((0)) FOR [UserReusable]
GO
/****** Object:  Default [DF_QuestionSets_CollegeReusable]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[QuestionSets] ADD  CONSTRAINT [DF_QuestionSets_CollegeReusable]  DEFAULT ((0)) FOR [CollegeReusable]
GO
/****** Object:  Default [DF_QuestionSets_SystemReusable]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[QuestionSets] ADD  CONSTRAINT [DF_QuestionSets_SystemReusable]  DEFAULT ((0)) FOR [SystemReusable]
GO
/****** Object:  Default [DF_QuestionSets_UserReusable]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[QuestionSets] ADD  CONSTRAINT [DF_QuestionSets_UserReusable]  DEFAULT ((0)) FOR [UserReusable]
GO
/****** Object:  Default [DF_QuestionTypes_HasOptions]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[QuestionTypes] ADD  CONSTRAINT [DF_QuestionTypes_HasOptions]  DEFAULT ((0)) FOR [HasOptions]
GO
/****** Object:  Default [DF_Transactions_Credit]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[Transactions] ADD  CONSTRAINT [DF_Transactions_Credit]  DEFAULT ((0)) FOR [Credit]
GO
/****** Object:  Default [DF_Transactions_Check]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[Transactions] ADD  CONSTRAINT [DF_Transactions_Check]  DEFAULT ((0)) FOR [Check]
GO
/****** Object:  Default [DF_Transactions_Paid]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[Transactions] ADD  CONSTRAINT [DF_Transactions_Paid]  DEFAULT ((0)) FOR [Paid]
GO
/****** Object:  Default [DF_Transactions_Donation]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[Transactions] ADD  CONSTRAINT [DF_Transactions_Donation]  DEFAULT ((0)) FOR [Donation]
GO
/****** Object:  ForeignKey [FK_ChecksXTransactions_Checks]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[ChecksXTransactions]  WITH CHECK ADD  CONSTRAINT [FK_ChecksXTransactions_Checks] FOREIGN KEY([CheckId])
REFERENCES [dbo].[Checks] ([id])
GO
ALTER TABLE [dbo].[ChecksXTransactions] CHECK CONSTRAINT [FK_ChecksXTransactions_Checks]
GO
/****** Object:  ForeignKey [FK_ChecksXTransactions_Transactions]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[ChecksXTransactions]  WITH CHECK ADD  CONSTRAINT [FK_ChecksXTransactions_Transactions] FOREIGN KEY([TransactionId])
REFERENCES [dbo].[Transactions] ([id])
GO
ALTER TABLE [dbo].[ChecksXTransactions] CHECK CONSTRAINT [FK_ChecksXTransactions_Transactions]
GO
/****** Object:  ForeignKey [FK_Coupons_Items]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[Coupons]  WITH CHECK ADD  CONSTRAINT [FK_Coupons_Items] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([id])
GO
ALTER TABLE [dbo].[Coupons] CHECK CONSTRAINT [FK_Coupons_Items]
GO
/****** Object:  ForeignKey [FK_Editors_Items]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[Editors]  WITH CHECK ADD  CONSTRAINT [FK_Editors_Items] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([id])
GO
ALTER TABLE [dbo].[Editors] CHECK CONSTRAINT [FK_Editors_Items]
GO
/****** Object:  ForeignKey [FK_ExtendedProperties_ItemTypes]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[ExtendedProperties]  WITH CHECK ADD  CONSTRAINT [FK_ExtendedProperties_ItemTypes] FOREIGN KEY([ItemTypeId])
REFERENCES [dbo].[ItemTypes] ([id])
GO
ALTER TABLE [dbo].[ExtendedProperties] CHECK CONSTRAINT [FK_ExtendedProperties_ItemTypes]
GO
/****** Object:  ForeignKey [FK_ExtendedProperties_QuestionTypes]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[ExtendedProperties]  WITH CHECK ADD  CONSTRAINT [FK_ExtendedProperties_QuestionTypes] FOREIGN KEY([QuestionTypeId])
REFERENCES [dbo].[QuestionTypes] ([id])
GO
ALTER TABLE [dbo].[ExtendedProperties] CHECK CONSTRAINT [FK_ExtendedProperties_QuestionTypes]
GO
/****** Object:  ForeignKey [FK_ExtendedPropertyAnswers_ExtendedProperties]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[ExtendedPropertyAnswers]  WITH CHECK ADD  CONSTRAINT [FK_ExtendedPropertyAnswers_ExtendedProperties] FOREIGN KEY([ExtendedPropertyId])
REFERENCES [dbo].[ExtendedProperties] ([id])
GO
ALTER TABLE [dbo].[ExtendedPropertyAnswers] CHECK CONSTRAINT [FK_ExtendedPropertyAnswers_ExtendedProperties]
GO
/****** Object:  ForeignKey [FK_ExtendedPropertyAnswers_Items]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[ExtendedPropertyAnswers]  WITH CHECK ADD  CONSTRAINT [FK_ExtendedPropertyAnswers_Items] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([id])
GO
ALTER TABLE [dbo].[ExtendedPropertyAnswers] CHECK CONSTRAINT [FK_ExtendedPropertyAnswers_Items]
GO
/****** Object:  ForeignKey [FK_ItemQuestionSets_Items]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[ItemQuestionSets]  WITH CHECK ADD  CONSTRAINT [FK_ItemQuestionSets_Items] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([id])
GO
ALTER TABLE [dbo].[ItemQuestionSets] CHECK CONSTRAINT [FK_ItemQuestionSets_Items]
GO
/****** Object:  ForeignKey [FK_ItemQuestionSets_QuestionSets]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[ItemQuestionSets]  WITH CHECK ADD  CONSTRAINT [FK_ItemQuestionSets_QuestionSets] FOREIGN KEY([QuestionSetId])
REFERENCES [dbo].[QuestionSets] ([id])
GO
ALTER TABLE [dbo].[ItemQuestionSets] CHECK CONSTRAINT [FK_ItemQuestionSets_QuestionSets]
GO
/****** Object:  ForeignKey [FK_Items_ItemTypes]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[Items]  WITH CHECK ADD  CONSTRAINT [FK_Items_ItemTypes] FOREIGN KEY([ItemTypeId])
REFERENCES [dbo].[ItemTypes] ([id])
GO
ALTER TABLE [dbo].[Items] CHECK CONSTRAINT [FK_Items_ItemTypes]
GO
/****** Object:  ForeignKey [FK_ItemsXTags_Items]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[ItemsXTags]  WITH CHECK ADD  CONSTRAINT [FK_ItemsXTags_Items] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([id])
GO
ALTER TABLE [dbo].[ItemsXTags] CHECK CONSTRAINT [FK_ItemsXTags_Items]
GO
/****** Object:  ForeignKey [FK_ItemsXTags_Tags]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[ItemsXTags]  WITH CHECK ADD  CONSTRAINT [FK_ItemsXTags_Tags] FOREIGN KEY([TagId])
REFERENCES [dbo].[Tags] ([id])
GO
ALTER TABLE [dbo].[ItemsXTags] CHECK CONSTRAINT [FK_ItemsXTags_Tags]
GO
/****** Object:  ForeignKey [FK_ItemTypesXQuestionSets_ItemTypes]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[ItemTypesXQuestionSets]  WITH CHECK ADD  CONSTRAINT [FK_ItemTypesXQuestionSets_ItemTypes] FOREIGN KEY([ItemTypeId])
REFERENCES [dbo].[ItemTypes] ([id])
GO
ALTER TABLE [dbo].[ItemTypesXQuestionSets] CHECK CONSTRAINT [FK_ItemTypesXQuestionSets_ItemTypes]
GO
/****** Object:  ForeignKey [FK_ItemTypesXQuestionSets_QuestionSets]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[ItemTypesXQuestionSets]  WITH CHECK ADD  CONSTRAINT [FK_ItemTypesXQuestionSets_QuestionSets] FOREIGN KEY([QuestionSetId])
REFERENCES [dbo].[QuestionSets] ([id])
GO
ALTER TABLE [dbo].[ItemTypesXQuestionSets] CHECK CONSTRAINT [FK_ItemTypesXQuestionSets_QuestionSets]
GO
/****** Object:  ForeignKey [FK_QuantityAnswers_Questions]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[QuantityAnswers]  WITH CHECK ADD  CONSTRAINT [FK_QuantityAnswers_Questions] FOREIGN KEY([QuestionId])
REFERENCES [dbo].[Questions] ([id])
GO
ALTER TABLE [dbo].[QuantityAnswers] CHECK CONSTRAINT [FK_QuantityAnswers_Questions]
GO
/****** Object:  ForeignKey [FK_QuantityAnswers_QuestionSets]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[QuantityAnswers]  WITH CHECK ADD  CONSTRAINT [FK_QuantityAnswers_QuestionSets] FOREIGN KEY([QuestionSetId])
REFERENCES [dbo].[QuestionSets] ([id])
GO
ALTER TABLE [dbo].[QuantityAnswers] CHECK CONSTRAINT [FK_QuantityAnswers_QuestionSets]
GO
/****** Object:  ForeignKey [FK_QuantityAnswers_Transactions]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[QuantityAnswers]  WITH CHECK ADD  CONSTRAINT [FK_QuantityAnswers_Transactions] FOREIGN KEY([TransactionId])
REFERENCES [dbo].[Transactions] ([id])
GO
ALTER TABLE [dbo].[QuantityAnswers] CHECK CONSTRAINT [FK_QuantityAnswers_Transactions]
GO
/****** Object:  ForeignKey [FK_QuestionOptions_Questions]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[QuestionOptions]  WITH CHECK ADD  CONSTRAINT [FK_QuestionOptions_Questions] FOREIGN KEY([QuestionId])
REFERENCES [dbo].[Questions] ([id])
GO
ALTER TABLE [dbo].[QuestionOptions] CHECK CONSTRAINT [FK_QuestionOptions_Questions]
GO
/****** Object:  ForeignKey [FK_Questions_QuestionTypes]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[Questions]  WITH CHECK ADD  CONSTRAINT [FK_Questions_QuestionTypes] FOREIGN KEY([QuestionTypeId])
REFERENCES [dbo].[QuestionTypes] ([id])
GO
ALTER TABLE [dbo].[Questions] CHECK CONSTRAINT [FK_Questions_QuestionTypes]
GO
/****** Object:  ForeignKey [FK_QuestionsXQuestionSets_Questions]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[QuestionSetQuestions]  WITH CHECK ADD  CONSTRAINT [FK_QuestionsXQuestionSets_Questions] FOREIGN KEY([QuestionId])
REFERENCES [dbo].[Questions] ([id])
GO
ALTER TABLE [dbo].[QuestionSetQuestions] CHECK CONSTRAINT [FK_QuestionsXQuestionSets_Questions]
GO
/****** Object:  ForeignKey [FK_QuestionsXQuestionSets_QuestionSets]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[QuestionSetQuestions]  WITH CHECK ADD  CONSTRAINT [FK_QuestionsXQuestionSets_QuestionSets] FOREIGN KEY([QuestionSetId])
REFERENCES [dbo].[QuestionSets] ([id])
GO
ALTER TABLE [dbo].[QuestionSetQuestions] CHECK CONSTRAINT [FK_QuestionsXQuestionSets_QuestionSets]
GO
/****** Object:  ForeignKey [FK_TransactionAnswers_Questions]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[TransactionAnswers]  WITH CHECK ADD  CONSTRAINT [FK_TransactionAnswers_Questions] FOREIGN KEY([QuestionId])
REFERENCES [dbo].[Questions] ([id])
GO
ALTER TABLE [dbo].[TransactionAnswers] CHECK CONSTRAINT [FK_TransactionAnswers_Questions]
GO
/****** Object:  ForeignKey [FK_TransactionAnswers_QuestionSets]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[TransactionAnswers]  WITH CHECK ADD  CONSTRAINT [FK_TransactionAnswers_QuestionSets] FOREIGN KEY([QuestionSetId])
REFERENCES [dbo].[QuestionSets] ([id])
GO
ALTER TABLE [dbo].[TransactionAnswers] CHECK CONSTRAINT [FK_TransactionAnswers_QuestionSets]
GO
/****** Object:  ForeignKey [FK_TransactionAnswers_Transactions]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[TransactionAnswers]  WITH CHECK ADD  CONSTRAINT [FK_TransactionAnswers_Transactions] FOREIGN KEY([TransactionId])
REFERENCES [dbo].[Transactions] ([id])
GO
ALTER TABLE [dbo].[TransactionAnswers] CHECK CONSTRAINT [FK_TransactionAnswers_Transactions]
GO
/****** Object:  ForeignKey [FK_Transactions_Coupons]    Script Date: 01/06/2010 07:31:38 ******/
ALTER TABLE [dbo].[Transactions]  WITH CHECK ADD  CONSTRAINT [FK_Transactions_Coupons] FOREIGN KEY([CouponId])
REFERENCES [dbo].[Coupons] ([id])
GO
ALTER TABLE [dbo].[Transactions] CHECK CONSTRAINT [FK_Transactions_Coupons]
GO
