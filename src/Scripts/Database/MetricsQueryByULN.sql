DECLARE @collectionperiod INT = 11
DECLARE @academicYear INT = 2021

IF OBJECT_ID('tempdb..#submissionLearners') IS NOT NULL DROP TABLE #submissionLearners
IF OBJECT_ID('tempdb..#manualMetrics') IS NOT NULL DROP TABLE #manualMetrics
IF OBJECT_ID('tempdb..#Payments') IS NOT NULL DROP TABLE #Payments
IF OBJECT_ID('tempdb..#RequiredPayments') IS NOT NULL DROP TABLE #RequiredPayments
IF OBJECT_ID('tempdb..#EarningEvents') IS NOT NULL DROP TABLE #EarningEvents
IF OBJECT_ID('tempdb..#DASEarnings') IS NOT NULL DROP TABLE #DASEarnings
IF OBJECT_ID('tempdb..#DataLockedEarningEvents') IS NOT NULL DROP TABLE #DataLockedEarningEvents
IF OBJECT_ID('tempdb..#DlockedEarnings') IS NOT NULL DROP TABLE #DlockedEarnings
IF OBJECT_ID('tempdb..#DatalockedPayments') IS NOT NULL DROP TABLE #DatalockedPayments

DECLARE @ukprnList table
(
	ukprn bigint NOT NULL PRIMARY KEY CLUSTERED
)

INSERT INTO @ukprnList --whitelist of suceeded ukprns if required
VALUES
(10004303)

Declare @GivenUkprnCount INT = (SELECT COUNT(1) from @ukprnList)
Declare @LatestJobIds Table(
	JobId INT NOT NULL PRIMARY KEY CLUSTERED,
	UkPrn bigint not null,
	CreationDate DATETIME2 not null
)

;WITH validJobs AS (
	SELECT MAX(IlrSubmissionTime) AS IlrSubmissionTime, Ukprn, AcademicYear
	FROM   Payments2.Job
	WHERE (Status IN (2, 3))  AND (DCJobSucceeded = 1)  AND (JobType = 1) and CollectionPeriod = @collectionperiod AND AcademicYear = @academicYear
	GROUP BY Ukprn, AcademicYear, CollectionPeriod
)
INSERT INTO @LatestJobIds
SELECT j.DcJobId, j.Ukprn, j.CreationDate
FROM   Payments2.Job AS j 
INNER JOIN validJobs AS vj 
ON j.IlrSubmissionTime = vj.IlrSubmissionTime 
AND j.Ukprn = vj.Ukprn 
AND j.AcademicYear = vj.AcademicYear

IF(@GivenUkprnCount > 0)
BEGIN
 DELETE FROM @LatestJobIds WHERE UkPrn NOT IN (SELECT UkPrn FROM @ukprnList)
END

--SELECT * FROM @LatestJobIds

--Required Payments
;WITH requiredPaymentCte as (
SELECT
	lji.Ukprn,
    LearnerUln,
	Sum(case when RPE.NonPaymentReason is null then Amount else 0 end) as RequiredPayment,
	Sum(case when RPE.NonPaymentReason is not null then Amount else 0 end) as HeldbackCompletionPayment
FROM @LatestJobIds lji
left JOIN Payments2.RequiredPaymentEvent RPE with (nolock) ON lji.JobId = RPE.JobId
WHERE RPE.CollectionPeriod = @collectionperiod
GROUP BY lji.Ukprn, LearnerUln
),
clawbackPayments as (
SELECT
	lji.Ukprn,
    LearnerUln,
	Sum(Amount) as RequiredPayment,
	0 as HeldbackCompletionPayment
FROM @LatestJobIds lji
left JOIN Payments2.Payment RPE with (nolock) ON lji.JobId = RPE.JobId
WHERE RPE.CollectionPeriod = @collectionperiod AND RPE.ClawbackSourcePaymentEventId != CAST(0x0 AS UNIQUEIDENTIFIER) AND RPE.ClawbackSourcePaymentEventId IS NOT NULL AND RPE.FundingSource IN (2, 3, 4)
GROUP BY lji.Ukprn, LearnerUln
)
SELECT ISNULL(r.Ukprn, p.UkPrn) as UkPrn, ISNULL(r.LearnerUln, p.LearnerUln) as LearnerUln, (ISNULL(r.RequiredPayment, 0) + ISNULL(p.RequiredPayment, 0)) as RequiredPayment, r.HeldbackCompletionPayment
INTO #RequiredPayments
FROM requiredPaymentCte as r 
FULL OUTER JOIN clawbackPayments as p ON r.Ukprn = p.Ukprn AND p.LearnerUln = r.LearnerUln

--SELECT * FROM #RequiredPayments

--payment table totals
SELECT
	Ukprn,
	LearnerUln,
    Sum(Amount) as totalPaymentsYtd,
	Sum(Case when CollectionPeriod < @collectionperiod then Amount else 0 end) as previousPaymentsYtd,--previous payments  1920
	Sum(Case when CollectionPeriod = @collectionperiod  then Amount else 0 end) as currentCollectionPayments--current  1920
INTO #Payments
FROM Payments2.Payment with (nolock)
WHERE AcademicYear = @academicYear
AND ((@GivenUkprnCount = 0) OR  (ukprn in (SELECT ids.ukprn FROM @ukprnList ids)))
GROUP BY Ukprn, LearnerUln

--SELECT * FROM #Payments

----DAS earnings
SELECT EE.LearnerReferenceNumber, EE.LearnerUln, EE.LearningAimFrameworkCode,
    EE.LearningAimPathwayCode, EE.LearningAimProgrammeType, EE.LearningAimReference,
    EE.LearningAimStandardCode, EE.Ukprn, EEP.Amount, EEP.DeliveryPeriod, EE.CollectionPeriod, 
    EEP.TransactionType, EE.EventId, EE.JobId, EE.ContractType
INTO #EarningEvents
FROM Payments2.EarningEvent EE with (nolock)
JOIN Payments2.EarningEventPeriod EEP with (nolock)
    ON EEP.EarningEventId = EE.EventId
WHERE DeliveryPeriod <= @collectionperiod
AND EE.CollectionPeriod = @collectionperiod AND EE.AcademicYear = @academicYear
AND Amount != 0
AND ((@GivenUkprnCount = 0) OR  (EE.ukprn in (SELECT ids.ukprn FROM @ukprnList ids)))
AND (ee.JobId in (SELECT JobId FROM @LatestJobIds))

SELECT
	Ukprn,
	LearnerUln,
	SUM(Amount) as DASEarnings
INTO #DASEarnings
FROM #EarningEvents 
GROUP BY Ukprn, LearnerUln

--SELECT * FROM #DASEarnings
--Dlocked Earnings
SELECT EE.LearnerReferenceNumber, EE.LearnerUln, EE.LearningAimFrameworkCode,
    EE.LearningAimPathwayCode, EE.LearningAimProgrammeType, EE.LearningAimReference,
    EE.LearningAimStandardCode, EE.Ukprn, EE.Amount, EE.DeliveryPeriod,
    EE.TransactionType, EE.CollectionPeriod
INTO #DataLockedEarningEvents
FROM #EarningEvents AS EE
WHERE EXISTS (
    SELECT *
    FROM Payments2.DataLockEvent DLE with (nolock)
    JOIN Payments2.DataLockEventNonPayablePeriod DLENP with (nolock)
        ON DLE.EventId = DLENP.DataLockEventId
    WHERE DLE.EarningEventId = EE.EventId
    AND DLENP.DeliveryPeriod = EE.DeliveryPeriod
    --AND DLENP.PriceEpisodeIdentifier = EEP.PriceEpisodeIdentifier --this is removed after the PV2-2080 R13/R02 ME: BUG FIX: Drop in Earnings - (Output from Spike 2073)
	AND (DLE.JobId in (SELECT JobId FROM @LatestJobIds))
) AND EE.ContractType = 1

SELECT
	Ukprn,
	LearnerUln,
	SUM(Amount) as DatalockedEarnings
INTO #DlockedEarnings
FROM #DataLockedEarningEvents
GROUP BY Ukprn, LearnerUln

--SELECT * FROM #DlockedEarnings

--Datalocked Payments
SELECT
	Ukprn,
	LearnerUln,
	Sum(amount) as DatalockedPayments
INTO #DatalockedPayments
FROM Payments2.Payment P with (nolock)
WHERE EXISTS (
    SELECT *
    FROM #DataLockedEarningEvents E with (nolock)
    WHERE P.Ukprn = E.Ukprn
    AND P.LearnerReferenceNumber = E.LearnerReferenceNumber
    AND P.DeliveryPeriod = E.DeliveryPeriod
    AND E.LearningAimFrameworkCode = P.LearningAimFrameworkCode
    AND E.LearningAimPathwayCode = P.LearningAimPathwayCode
    AND E.LearningAimProgrammeType = P.LearningAimProgrammeType
    AND E.LearningAimReference = P.LearningAimReference
    AND E.LearningAimStandardCode = P.LearningAimStandardCode
    AND E.TransactionType = P.TransactionType
	AND P.AcademicYear = @academicYear
    AND p.collectionperiod < E.CollectionPeriod
	AND p.ContractType = 1
)
AND ((@GivenUkprnCount = 0) OR  (ukprn in (SELECT ids.ukprn FROM @ukprnList ids)))
GROUP BY Ukprn, LearnerUln

;With submissionLearners AS (
SELECT DISTINCT Ukprn, LearnerUln FROM #DASEarnings UNION
SELECT DISTINCT Ukprn, LearnerUln FROM #DlockedEarnings UNION
SELECT DISTINCT Ukprn, LearnerUln FROM #DatalockedPayments UNION
SELECT DISTINCT Ukprn, LearnerUln FROM #RequiredPayments UNION
SELECT DISTINCT Ukprn, LearnerUln FROM #Payments
)
SELECT 
* 
INTO #submissionLearners
FROM submissionLearners

--SELECT * FROM #submissionLearners where LearnerUln = 3221244060

SELECT distinct
    j.LearnerUln,
	ISNULL(e.DASEarnings, 0) AS DASEarnings,
	ISNULL(de.DatalockedEarnings, 0) AS DatalockedEarnings,
	ISNULL(dp.DatalockedPayments, 0) AS DatalockedPayments,
    ISNULL(de.DatalockedEarnings, 0) - ISNULL(dp.DatalockedPayments, 0) AS AdjustedDatalocke,
	ISNULL(r.HeldbackCompletionPayment, 0) AS HeldbackCompletionPayment,
	ISNULL(p.previousPaymentsYtd, 0) AS previousPaymentsYtd,
	ISNULL(p.totalPaymentsYtd, 0) AS totalPaymentsYtd,
	ISNULL(r.RequiredPayment, 0) AS RequiredPayments,
	ISNULL(p.currentCollectionPayments, 0) AS Payments,
                                                                               (ISNULL(de.DatalockedEarnings, 0) - ISNULL(dp.DatalockedPayments, 0)) + ISNULL(r.HeldbackCompletionPayment, 0) + ISNULL(p.previousPaymentsYtd, 0) + ISNULL(r.RequiredPayment, 0) AS EstimatedPaymentYtd,
	ISNULL(e.DASEarnings, 0) -                                                ((ISNULL(de.DatalockedEarnings, 0) - ISNULL(dp.DatalockedPayments, 0)) + ISNULL(r.HeldbackCompletionPayment, 0) + ISNULL(p.previousPaymentsYtd, 0) + ISNULL(r.RequiredPayment, 0)) AS MissingEstimatedPayments,

	CASE WHEN ISNULL(e.DASEarnings, 0) = 0 THEN 0 ELSE Convert(decimal(15,2),(((ISNULL(de.DatalockedEarnings, 0) - ISNULL(dp.DatalockedPayments, 0)) + ISNULL(r.HeldbackCompletionPayment, 0) + ISNULL(p.previousPaymentsYtd, 0) + ISNULL(r.RequiredPayment, 0)) / ISNULL(e.DASEarnings, 0)) * 100) END AS [Estimated%],

	                                                                           (ISNULL(de.DatalockedEarnings, 0) - ISNULL(dp.DatalockedPayments, 0)) + ISNULL(r.HeldbackCompletionPayment, 0) + ISNULL(p.totalPaymentsYtd, 0) AS AdjustedPaymentYtd,
	ISNULL(e.DASEarnings, 0) -                                                ((ISNULL(de.DatalockedEarnings, 0) - ISNULL(dp.DatalockedPayments, 0)) + ISNULL(r.HeldbackCompletionPayment, 0) + ISNULL(p.totalPaymentsYtd, 0)) AS MissingDasPayments,

    CASE WHEN ISNULL(e.DASEarnings, 0) = 0 THEN 0 ELSE convert(decimal(15,2),(((ISNULL(de.DatalockedEarnings, 0) - ISNULL(dp.DatalockedPayments, 0)) + ISNULL(r.HeldbackCompletionPayment, 0) + ISNULL(p.totalPaymentsYtd, 0)) / ISNULL(e.DASEarnings, 0)) * 100) END AS [Actual%]

into #manualMetrics
FROM #submissionLearners AS J 
LEFT JOIN #DASEarnings AS e         ON j.LearnerUln = e.LearnerUln
LEFT JOIN #DlockedEarnings as de	ON j.LearnerUln = de.LearnerUln
LEFT JOIN #DatalockedPayments as dp ON j.LearnerUln = dp.LearnerUln
LEFT JOIN #RequiredPayments as r    ON j.LearnerUln =  r.LearnerUln
LEFT JOIN #Payments as p            ON j.LearnerUln =  p.LearnerUln

--use bellow queries if you want to investigate individual learner audit data
--SELECT * FROM #DASEarnings where LearnerUln = 0
--SELECT * FROM #DlockedEarnings where LearnerUln = 0
--SELECT * FROM #DatalockedPayments where LearnerUln = 0
--SELECT * FROM #RequiredPayments where LearnerUln = 0
--SELECT * FROM #Payments where LearnerUln = 0

SELECT * FROM #manualMetrics --where MissingEstimatedPayments != 0.000