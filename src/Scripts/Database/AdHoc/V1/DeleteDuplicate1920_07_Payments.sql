BEGIN TRANSACTION
;WITH cte
AS (
	
SELECT
ROW_NUMBER() over (
			PARTITION BY 
		[RequiredPaymentId]
      ,[DeliveryMonth]
      ,[DeliveryYear]
      ,[CollectionPeriodName]
      ,[CollectionPeriodMonth]
      ,[CollectionPeriodYear]
      ,[FundingSource]
      ,[TransactionType]
      ,[Amount]
	    ORDER BY paymentId
			) as RN,
			p.paymentId
FROM [Payments].[Payments] P
WHERE CollectionPeriodName = '1920-R07'
	)


--select test prior to deletion (mutally exclusive with the query below)

SELECT payments.*
FROM CTE payments
INNER JOIN [Payments].[Payments] P
	ON p.PaymentId = payments.PaymentId
WHERE payments.RN > 1
ORDER BY
	   [p].[RequiredPaymentId]
      ,[p].[DeliveryMonth]
      ,[p].[DeliveryYear]
      ,[p].[CollectionPeriodName]
      ,[p].[CollectionPeriodMonth]
      ,[p].[CollectionPeriodYear]
      ,[p].[FundingSource]
      ,[p].[TransactionType]
      ,[p].[Amount]

--Deletion

--DELETE
--FROM [Payments].[Payments]
--WHERE PaymentId IN (
--		SELECT PaymentId
--		FROM cte
--		WHERE rn > 1
--		)

ROLLBACK TRANSACTION
