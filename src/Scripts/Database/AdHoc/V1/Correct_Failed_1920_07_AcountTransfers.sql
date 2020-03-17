USE [DAS_PeriodEnd]
BEGIN TRAN
​
;WITH cte
AS (
	SELECT
	ROW_NUMBER() over (
				PARTITION BY 
			[SendingAccountId], [ReceivingAccountId], [RequiredPaymentId], [CommitmentId], [Amount], [TransferType], [CollectionPeriodName], [CollectionPeriodMonth], [CollectionPeriodYear]
			ORDER BY TransferId
				) as RN,
				AT.TransferId
	FROM [TransferPayments].[AccountTransfers] AT
	where RequiredPaymentId IN (
	'FF02F4D6-B272-4058-ABB1-E9D6FACACA40',
	'C40880D0-E658-4282-84B4-0A751ADD16E4',
	'7E616CE3-08E4-464A-8992-957FE7253BF4',
	'C7F9D2A8-A003-4B0C-A8C7-154F3DF3C03B',
	'9FADBE75-858B-4F1F-BB5B-39A230168DC7',
	'E89D1148-BA07-47F5-A85C-450CAD930C58',
	'E2215898-E9D2-4042-BC07-DD3EFA1CA370',
	'42B5023A-F2C9-42BF-AC71-62CB6D18FEB4'
	)
)

select cte.rn, AT.* from cte join [TransferPayments].[AccountTransfers] AT
on cte.TransferId = AT.TransferId
where rn > 1


-- Delete
--DELETE
--FROM [TransferPayments].[AccountTransfers]
--WHERE TransferId IN (
--		SELECT TransferId
--		FROM cte
--		WHERE rn > 1
--		)

ROLLBACK