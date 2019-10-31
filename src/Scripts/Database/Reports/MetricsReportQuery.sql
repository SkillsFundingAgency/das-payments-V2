DECLARE @academicYear SMALLINT = 1920
DECLARE @collectionPeriod TINYINT = 3
DECLARE @ukprn bigInt = null -- 10000060 /*Set ukprn to required value to get metrics for a single provider*/

;WITH IncludedTransactionTypes AS
(
	SELECT n AS TransactionType FROM (VALUES (1),(2),(3),(4),(5),(6),(7),(8),(9),(10),(11),(12),(13),(14),(15),(16)) v(n)
),
EarningsYTD AS
(
	SELECT TransactionType
		   ,COUNT(DISTINCT e.LearnerUln) [EarningLearnerCount]
		   ,SUM(CASE WHEN Amount > 0 THEN Amount END) [EarningsYTD]
		   ,SUM(CASE WHEN ContractType = 1
							   AND Amount > 0 THEN Amount END) [EarningsACT1]
		   ,sum(CASE WHEN ContractType = 2
							   AND Amount > 0 THEN Amount END) [EarningsACT2]
		   ,sum(CASE WHEN Amount < 0 THEN Amount END) [NegativeEarningsYTD]
		   ,sum(CASE WHEN ContractType = 1
							   AND Amount < 0 THEN Amount END) [NegativeEarningsACT1]
		   ,sum(CASE WHEN ContractType = 2
							   AND Amount < 0 THEN Amount END) [NegativeEarningsACT2]
	FROM [Payments2].[EarningEvent] e WITH (NOLOCK)
	INNER JOIN [Payments2].[EarningEventPeriod] p WITH (NOLOCK)
		   ON p.EarningEventId = e.EventId
	WHERE e.AcademicYear = @academicYear
		   AND e.CollectionPeriod <= @collectionPeriod
		   AND ((@ukprn IS NULL) or (@ukprn = ukprn))
	GROUP BY p.TransactionType
),
RequiredPaymentsYTD AS
(
	SELECT RP.TransactionType
		   ,count(DISTINCT RP.LearnerUln) [RequiredPaymentLearnerCount]
		   ,sum(Amount) [RequiredPaymentsYTD]
		   ,sum(CASE WHEN ContractType = 1 THEN Amount END) [RequiredPaymentsACT1]
		   ,sum(CASE WHEN ContractType = 2 THEN Amount END) [RequiredPaymentsACT2]
	FROM [Payments2].[RequiredPaymentEvent] RP WITH (NOLOCK)
	WHERE CollectionPeriod <= @collectionPeriod
		   AND AcademicYear = @academicYear
		   AND ((@ukprn IS NULL) or (@ukprn = ukprn))
	GROUP BY TransactionType
),
ActualPaymentsYTD AS
(
	SELECT TransactionType
		   ,count(DISTINCT LearnerUln) [ActualPaymentLearnerCount]
		   ,sum(Amount) [ActualPaymentsYTD]
		   ,sum(CASE WHEN ContractType = 1 THEN Amount END) [ActualPaymentsACT1]
		   ,sum(CASE WHEN ContractType = 2 THEN Amount END) [ActualPaymentsACT2]
	FROM Payments2.Payment WITH (NOLOCK)
	WHERE AcademicYear = @academicYear
		   AND CollectionPeriod <= @collectionPeriod
		   AND ((@ukprn IS NULL) or (@ukprn = ukprn))
	GROUP BY TransactionType
),
DataLocks AS
(
	SELECT dlenpp.TransactionType
		   ,count(DISTINCT dle.LearnerUln) AS [DataLockErrorCount]
		   ,sum(dlenpp.amount) AS [DataLockErrors]
	FROM [Payments2].[DataLockEvent] dle
	INNER JOIN [Payments2].[DataLockEventNonPayablePeriod] dlenpp
		   ON dle.EventId = dlenpp.DataLockEventId
	WHERE dle.CollectionPeriod = @collectionPeriod
		   AND dle.AcademicYear = @academicYear
		   AND dle.IsPayable = 0
		   AND dlenpp.amount > 0
		   AND ((@ukprn IS NULL) or (@ukprn = ukprn))
	GROUP BY dlenpp.TransactionType
), 
HeldBackCompletionPayments AS
(
	SELECT ep.TransactionType
		   ,sum(CASE WHEN p.Id IS NULL
							   AND f.Id IS NULL THEN ep.Amount END) AS HeldBackCompletionPayments
		   ,sum(CASE WHEN p.Id IS NULL
							   AND f.Id IS NULL
							   AND e.ContractType = 1 THEN ep.Amount END) AS HeldBackCompletionPaymentsAct1
		   ,sum(CASE WHEN p.Id IS NULL
							   AND f.Id IS NULL
							   AND e.ContractType = 2 THEN ep.Amount END) AS HeldBackCompletionPaymentsAct2
	FROM Payments2.EarningEvent e WITH (NOLOCK)
	INNER JOIN Payments2.EarningEventPeriod ep WITH (NOLOCK)
		   ON ep.EarningEventId = e.EventId
	LEFT JOIN Payments2.RequiredPaymentEvent p WITH (NOLOCK)
		   ON p.EarningEventId = ep.EarningEventId
				  AND p.DeliveryPeriod = ep.DeliveryPeriod
				  AND p.TransactionType = ep.TransactionType
	LEFT JOIN Payments2.DataLockFailure f WITH (NOLOCK)
		   ON f.EarningEventId = e.EventId
				  AND f.DeliveryPeriod = ep.DeliveryPeriod
				  AND f.TransactionType = ep.TransactionType
	WHERE ep.TransactionType = 2
		   AND ep.Amount <> 0
		   AND e.AcademicYear = @academicYear
		   AND e.CollectionPeriod = @collectionPeriod
		   AND ((@ukprn IS NULL) or (@ukprn = e.ukprn))
	GROUP BY ep.TransactionType
)



--MAIN Query
SELECT 
	TransactionTypes.TransactionType
	,ISNULL(RequiredPayments.RequiredPaymentsYTD, 0) AS [Required Payments YTD]
	,ISNULL(RequiredPayments.[RequiredPaymentsACT1], 0) AS [Required Payments ACT1 YTD]
	,ISNULL(RequiredPayments.[RequiredPaymentsACT2], 0) AS [Required Payments ACT2 YTD]
	,ISNULL(ActualPayments.ActualPaymentsYTD, 0) AS [Actual Payments YTD]
	,ISNULL(ActualPayments.[ActualPaymentsACT1], 0) AS [Actual Payments ACT1 YTD]
	,ISNULL(ActualPayments.[ActualPaymentsACT2], 0) AS [Actual Payments ACT2 YTD]
	,ISNULL(Earnings.EarningsYTD, 0) AS [Earnings YTD]
	,ISNULL(DataLocks.[DataLockErrors], 0) AS [DataLock Errors]
	,ISNULL(Earnings.[EarningsACT1], 0) AS [Earnings ACT1 YTD]
	,ISNULL(Earnings.[EarningsACT2], 0) AS [Earnings ACT2 YTD]
	,ISNULL(Heldbackcompletion.[HeldBackCompletionPayments], 0) AS [Held back completion payments]
	,ISNULL(Earnings.NegativeEarningsYTD, 0) AS [Negatvive Earnings YTD]
FROM 
	IncludedTransactionTypes TransactionTypes
LEFT JOIN 
	EarningsYTD Earnings
 ON 
	Earnings.TransactionType = TransactionTypes.TransactionType
LEFT JOIN 
	RequiredPaymentsYTD RequiredPayments
ON 
	RequiredPayments.TransactionType = TransactionTypes.TransactionType
LEFT JOIN
	ActualPaymentsYTD ActualPayments
ON 
	ActualPayments.TransactionType = TransactionTypes.TransactionType
LEFT JOIN 
	DataLocks DataLocks
ON
	DataLocks.TransactionType = TransactionTypes.TransactionType
LEFT JOIN 
	HeldBackCompletionPayments HeldBackCompletion
ON 
	HeldBackCompletion.TransactionType = TransactionTypes.TransactionType
ORDER BY
    TransactionTypes.TransactionType