BEGIN TRAN
​
;WITH cte
AS (
	SELECT
	ROW_NUMBER() over (
				PARTITION BY 
			[RequiredPaymentId]
			ORDER BY PlannedEndDate
				) as RN,
				p.RequiredPaymentId
	FROM [PaymentsDue].[Earnings] P
)
​
DELETE
FROM CTE
WHERE rn > 1
​
ROLLBACK
