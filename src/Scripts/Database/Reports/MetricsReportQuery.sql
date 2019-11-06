DECLARE @academicYear SMALLINT = 1920
DECLARE @collectionPeriod TINYINT = 3

DECLARE @ValidUkprns TABLE
(
	ukprn bigInt,
	lastDcJobId bigInt
)


INSERT INTO @ValidUkprns
SELECT 
	j.ukprn,
	j.dcjobid
FROM 
	payments2.job j 
INNER JOIN
	(
	SELECT 
		ukprn, 
		MAX(ilrSubmissionTime) lastIlrSubmission
	FROM 
		payments2.job
	WHERE 
		AcademicYear = @academicYear
	AND
		CollectionPeriod = @collectionPeriod
	GROUP BY 
		ukprn
	) lastSubmission
ON 
	lastSubmission.ukprn = j.ukprn
AND 
	lastsubmission.lastIlrSubmission = j.ilrSubmissionTime
AND 
	j.[status] = 2

--removed any failed ukprns from DC
DELETE @ValidUkprns FROM @ValidUkprns
WHERE UKPRN IN (
10000715
,10001143
,10001148
,10001436
,10001696
,10001778
,10001800
,10002131
,10002167
,10002424
,10002463
,10003240
,10003354
,10003981
,10003990
,10004124
,10004180
,10004285
,10004351
,10004399
,10004499
,10004527
,10006574
,10007013
,10007100
,10007137
,10007140
,10007144
,10007146
,10007152
,10007154
,10007318
,10007788
,10007977
,10010134
,10010548
,10010549
,10010586
,10011138
,10011147
,10011159
,10011332
,10019048
,10019210
,10019217
,10019383
,10020019
,10020068
,10021172
,10021481
,10021539
,10021602
,10021754
,10022567
,10022689
,10022763
,10023592
,10023705
,10023776
,10023787
,10023918
,10024054
,10024293
,10024633
,10024686
,10025700
,10026442
,10026702
,10027269
,10027662
,10028269
,10028342
,10028441
,10028909
,10028930
,10029332
,10029676
,10029887
,10030265
,10030520
,10030670
,10030740
,10031424
,10031544
,10031912
,10032250
,10032449
,10032616
,10032658
,10033440
,10033536
,10033608
,10033670
,10033815
,10034022
,10034128
,10034387
,10034767
,10035735
,10036036
,10036049
,10036106
,10036240
,10037119
,10037126
,10037213
,10037757
,10037909
,10038201
,10038829
,10039793
,10039859
,10039882
,10039956
,10040011
,10040368
,10040391
,10040417
,10040440
,10040525
,10040812
,10040883
,10041069
,10041165
,10041236
,10041319
,10041332
,10042014
,10042119
,10042166
,10042291
,10043126
,10043250
,10043575
,10043661
,10044749
,10045119
,10045159
,10045305
,10045348
,10045505
,10045935
,10046498
,10046705
,10046777
,10047090
,10048409
,10048788
,10048801
,10048806
,10048827
,10048848
,10049099
,10052538
,10052574
,10052606
,10053742
,10054055
,10054118
,10054451
,10054814
,10054898
,10054962
,10055259
,10055479
,10056190
,10056323
,10056694
,10056711
,10056912
,10057175
,10057328
,10058007
,10058111
,10061144
,10061302
,10061312
,10061438
,10061540
,10061548
,10061591
,10061640
,10061808
,10061840
,10061842
,10062041
,10062166
,10062335
,10063272
,10063286
,10063477
,10063587
,10063769
,10064216
,10065535
,10065593
,10065752
,10065814
,10065941
)





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
	INNER JOIN @ValidUkprns UKPRNS
			ON UKPRNS.ukprn = e.ukprn  and ukprns.lastDcJobId = e.jobid
	WHERE e.AcademicYear = @academicYear
		   AND e.CollectionPeriod = @collectionPeriod
	
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
	INNER JOIN @ValidUkprns UKPRNS
			ON UKPRNS.ukprn = rp.ukprn
	WHERE CollectionPeriod <= @collectionPeriod
		   AND AcademicYear = @academicYear
	GROUP BY TransactionType
),
ActualPaymentsYTD AS
(
	SELECT TransactionType
		   ,count(DISTINCT LearnerUln) [ActualPaymentLearnerCount]
		   ,sum(Amount) [ActualPaymentsYTD]
		   ,sum(CASE WHEN ContractType = 1 THEN Amount END) [ActualPaymentsACT1]
		   ,sum(CASE WHEN ContractType = 2 THEN Amount END) [ActualPaymentsACT2]
	FROM Payments2.Payment  p WITH (NOLOCK)
		INNER JOIN @ValidUkprns UKPRNS
			ON UKPRNS.ukprn = p.ukprn
	WHERE AcademicYear = @academicYear
		   AND CollectionPeriod <= @collectionPeriod
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
	INNER JOIN @ValidUkprns UKPRNS
			ON UKPRNS.ukprn = dle.ukprn
	WHERE dle.CollectionPeriod = @collectionPeriod
		   AND dle.AcademicYear = @academicYear
		   AND dle.IsPayable = 0
		   AND dlenpp.amount > 0
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
	INNER JOIN @ValidUkprns UKPRNS
			ON UKPRNS.ukprn = e.ukprn
	WHERE ep.TransactionType = 2
		   AND ep.Amount <> 0
		   AND e.AcademicYear = @academicYear
		   AND e.CollectionPeriod = @collectionPeriod
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