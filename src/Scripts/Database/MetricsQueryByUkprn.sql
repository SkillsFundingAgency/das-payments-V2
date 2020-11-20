--SET STATISTICS TIME ON;
--GO
DECLARE @collectionperiod INT = 1
DECLARE @academicYear INT = 2021

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

--INSERT INTO @ukprnList --whitelist of suceeded ukprns if required
--VALUES
--(10004285)

Declare @GivenUkprnCount INT = (SELECT COUNT(1) from @ukprnList)
Declare @LatestJobIds Table(
	JobId INT NOT NULL PRIMARY KEY CLUSTERED,
	UkPrn bigint not null,
	CreationDate DATETIME2 not null
)

INSERT INTO @LatestJobIds
SELECT DcJobId, Ukprn, CreationDate
FROM Payments2.LatestSuccessfulJobs 
WHERE CollectionPeriod = @collectionperiod AND AcademicYear = @academicYear

IF(@GivenUkprnCount > 0)
BEGIN
 DELETE FROM @LatestJobIds WHERE UkPrn NOT IN (SELECT UkPrn FROM @ukprnList)
END

----ukprns  - for whitelist
--SELECT STRING_AGG (CAST(UkPrn AS nvarchar(max)), ',') AS ukprncsv
--FROM @LatestJobIds
----jobids -- for info
--SELECT STRING_AGG (CAST(JobId AS nvarchar(max)), ',') AS jobidcsv
--FROM @LatestJobIds

--Payments
SELECT
	lji.Ukprn,
	Sum(case when RPE.NonPaymentReason is null then Amount else 0 end) as RequiredPayment,
	Sum(case when RPE.NonPaymentReason is not null then Amount else 0 end) as HeldbackCompletionPayment
INTO #RequiredPayments
FROM @LatestJobIds lji
left JOIN Payments2.RequiredPaymentEvent RPE with (nolock) ON lji.JobId = RPE.JobId
WHERE RPE.CollectionPeriod = @collectionperiod
GROUP BY lji.Ukprn

--payment table totals
SELECT
	Ukprn,
	Sum(Amount) as totalPaymentsYtd,
	Sum(Case when CollectionPeriod < @collectionperiod then Amount else 0 end) as previousPaymentsYtd,--previous payments  1920
	Sum(Case when CollectionPeriod = @collectionperiod  then Amount else 0 end) as currentCollectionPayments--current  1920
INTO #Payments
FROM Payments2.Payment with (nolock)
WHERE AcademicYear = @academicYear
AND ((@GivenUkprnCount = 0) OR  (ukprn in (SELECT ids.ukprn FROM @ukprnList ids)))
GROUP BY Ukprn
	
----DAS earnings
SELECT EE.LearnerReferenceNumber, EE.LearnerUln, EE.LearningAimFrameworkCode,
    EE.LearningAimPathwayCode, EE.LearningAimProgrammeType, EE.LearningAimReference,
    EE.LearningAimStandardCode, EE.Ukprn, EEP.Amount, EEP.DeliveryPeriod, EE.CollectionPeriod, 
    EEP.TransactionType, EE.EventId, EE.JobId
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
	SUM(Amount) as DASEarnings
INTO #DASEarnings
FROM #EarningEvents 
GROUP BY Ukprn

--Datalocks
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
)

SELECT
	Ukprn,
	SUM(Amount) as DatalockedEarnings
INTO #DlockedEarnings
FROM #DataLockedEarningEvents
GROUP BY Ukprn

SELECT
	Ukprn,
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
)
AND ((@GivenUkprnCount = 0) OR  (ukprn in (SELECT ids.ukprn FROM @ukprnList ids)))
GROUP BY Ukprn
	
SELECT distinct
	j.*,
	ISNULL(e.DASEarnings, 0) AS DASEarnings,
	ISNULL(de.DatalockedEarnings, 0) AS DatalockedEarnings,
	ISNULL(dp.DatalockedPayments, 0) AS DatalockedPayments,
    ISNULL(de.DatalockedEarnings, 0) - ISNULL(dp.DatalockedPayments, 0) AS AdjustedDatalocke,
	ISNULL(r.RequiredPayment, 0) AS RequiredPayment,
	ISNULL(r.HeldbackCompletionPayment, 0) AS HeldbackCompletionPayment,
	ISNULL(p.currentCollectionPayments, 0) AS currentCollectionPayments,
	ISNULL(p.previousPaymentsYtd, 0) AS previousPaymentsYtd,
	ISNULL(p.totalPaymentsYtd, 0) AS totalPaymentsYtd,
    (ISNULL(de.DatalockedEarnings, 0) - ISNULL(dp.DatalockedPayments, 0)) + ISNULL(r.HeldbackCompletionPayment, 0) + ISNULL(p.totalPaymentsYtd, 0) AS AdjustedPaymentYtd,
    ISNULL(e.DASEarnings, 0) - ((ISNULL(de.DatalockedEarnings, 0) - ISNULL(dp.DatalockedPayments, 0)) + ISNULL(r.HeldbackCompletionPayment, 0) + ISNULL(p.totalPaymentsYtd, 0)) AS MissingDasPayments,
    CASE WHEN ISNULL(e.DASEarnings, 0) = 0 THEN 0 ELSE ((ISNULL(de.DatalockedEarnings, 0) - ISNULL(dp.DatalockedPayments, 0)) + ISNULL(r.HeldbackCompletionPayment, 0) + ISNULL(p.totalPaymentsYtd, 0)) / ISNULL(e.DASEarnings, 0) END AS [Missing%]
FROM @LatestJobIds AS J
LEFT JOIN #DASEarnings AS e         ON j.ukprn = e.ukprn
LEFT JOIN #DlockedEarnings as de	ON j.ukprn = de.ukprn
LEFT JOIN #DatalockedPayments as dp ON j.ukprn = dp.ukprn
LEFT JOIN #RequiredPayments as r    ON j.ukprn = r.ukprn
LEFT JOIN #Payments as p            ON j.ukprn = p.ukprn