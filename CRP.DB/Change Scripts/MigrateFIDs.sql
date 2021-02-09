INSERT INTO [CRP].[dbo].[FinancialAccounts]
	(UnitId, Chart, Account, SubAccount, Name)
SELECT DISTINCT 
	 i.[UnitId] as UnitId
	,LEFT(f.[Description], 1) as Chart
    ,SUBSTRING(f.[Description], 3, 7) as Account
    ,CASE LEN(f.[Description])
		WHEN 9 THEN ''
		ELSE REPLACE(RIGHT(f.[DESCRIPTION], 5), '-', '')
	 END as SubAccount
	,f.FID as 'Name'
  FROM [CRP].[dbo].[Items] i
  INNER JOIN [CRP].[dbo].[TouchnetFIDs] f on f.FID = i.TouchnetFID
  WHERE TouchnetFID IS NOT NULL

  GO

  UPDATE i
  SET i.[FinancialAccountId] = f.id
  FROM [CRP].[dbo].[Items] i
  INNER JOIN [CRP].[dbo].[FinancialAccounts] f on i.UnitId = f.UnitId AND i.TouchnetFID = f.Name
  WHERE i.[TouchnetFID] IS NOT NULL

  GO

  UPDATE t
  SET t.[FinancialAccountId] = f.id
  FROM [CRP].[dbo].[Transactions] t
  INNER JOIN [CRP].[dbo].[Items] i on i.id = t.ItemId
  INNER JOIN [CRP].[dbo].[FinancialAccounts] f on i.UnitId = f.UnitId AND t.FidUsed = f.Name
  WHERE FidUsed IS NOT NULL