SET STATISTICS TIME ON;  
GO 

DECLARE @monthendStartTime DATETIME = '2019-11-07 20:00'
DECLARE @startDate DATETIME = @monthendStartTime
DECLARE @collectionperiod INT = 3
DECLARE @ukprnList table
(
	ukprn bigint NOT NULL PRIMARY KEY CLUSTERED
)
--INSERT INTO @ukprnList --whitelist of suceeded ukprns if required
--VALUES
--(10000060),
--(10000061)

Declare @GivenUkprnCount INT = (SELECT COUNT(1) from @ukprnList)

Declare @LatestJobIds Table(
	JobId INT NOT NULL PRIMARY KEY CLUSTERED
)

INSERT INTO @LatestJobIds
SELECT DISTINCT(DcJobId)
FROM Payments2.LatestSuccessfulJobs

SELECT
	(SELECT FORMAT(SUM(Amount), 'C', 'en-gb')
	 FROM Payments2.RequiredPaymentEvent with (nolock)
	 WHERE CreationDate > @startDate
     AND NonPaymentReason IS NULL
	 AND ((@GivenUkprnCount = 0) OR  (ukprn in (SELECT ids.ukprn FROM @ukprnList ids)))
	 AND (JobId in (SELECT JobId FROM @LatestJobIds))
	 ) [Required Payments made this month],

	(SELECT FORMAT(SUM(Amount), 'C', 'en-gb')
	 FROM Payments2.Payment with (nolock)
	 WHERE CreationDate < @startDate
	 AND AcademicYear = 1920
	 AND ((@GivenUkprnCount = 0) OR  (ukprn in (SELECT ids.ukprn FROM @ukprnList ids)))
	 AND (JobId in (SELECT JobId FROM @LatestJobIds))
	 ) [Payments made before this month YTD],

    (SELECT FORMAT((SELECT SUM(Amount) 
     FROM Payments2.RequiredPaymentEvent with (nolock)
     WHERE CreationDate > @startDate
     AND NonPaymentReason IS NULL
	 AND ((@GivenUkprnCount = 0) OR  (ukprn in (SELECT ids.ukprn FROM @ukprnList ids)))
	 AND (JobId in (SELECT JobId FROM @LatestJobIds))
	 ) +
    (SELECT ISNULL(SUM(Amount),0) 
     FROM Payments2.Payment with (nolock)
     WHERE CreationDate < @startDate
	AND ((@GivenUkprnCount = 0) OR  (ukprn in (SELECT ids.ukprn FROM @ukprnList ids)))
	AND AcademicYear = 1920 ), 'C', 'en-gb') 
	) [Expected Payments YTD after running Period End],

	(SELECT FORMAT(SUM(Amount), 'C', 'en-gb')
     FROM Payments2.Payment with (nolock)
     where AcademicYear = 1920
	 and CollectionPeriod = @collectionperiod 
	 AND ((@GivenUkprnCount = 0) OR  (ukprn in (SELECT ids.ukprn FROM @ukprnList ids)))
	) [Total payments this month],

	
	(SELECT FORMAT(sum(CASE WHEN contracttype = 1 THEN amount ELSE 0 END), 'C', 'en-gb') 
     FROM Payments2.Payment with (nolock)
     where AcademicYear = 1920
	AND ((@GivenUkprnCount = 0) OR  (ukprn in (SELECT ids.ukprn FROM @ukprnList ids)))
	 ) [Total ACT 1 payments YTD],

	(SELECT FORMAT(sum(CASE WHEN contracttype = 2 THEN amount ELSE 0 END), 'C', 'en-gb') 
     FROM Payments2.Payment with (nolock)
     where AcademicYear = 1920
	 AND ((@GivenUkprnCount = 0) OR  (ukprn in (SELECT ids.ukprn FROM @ukprnList ids)))
	 ) [Total ACT 2 payments YTD],

	(SELECT FORMAT(SUM(Amount), 'C', 'en-gb') 
     FROM Payments2.Payment with (nolock)
     where AcademicYear = 1920
	 AND ((@GivenUkprnCount = 0) OR  (ukprn in (SELECT ids.ukprn FROM @ukprnList ids)))
	 ) [Total payments YTD],

	(SELECT FORMAT(SUM(Amount), 'C', 'en-gb')
	 FROM Payments2.RequiredPaymentEvent with (nolock)
	 WHERE CreationDate > @startDate
	 and NonPaymentReason = 0
	 AND ((@GivenUkprnCount = 0) OR  (ukprn in (SELECT ids.ukprn FROM @ukprnList ids)))
	 AND (JobId in (SELECT JobId FROM @LatestJobIds))
	 ) [Held Back Completion Payments this month]
--Expected Payments

SELECT FORMAT(SUM(Amount), 'C', 'en-gb') as [DAS Earnings]
FROM Payments2.EarningEvent EE with (nolock)
JOIN Payments2.EarningEventPeriod EEP with (nolock)
    ON EEP.EarningEventId = EE.EventId
AND DeliveryPeriod <= @collectionperiod
AND Amount != 0
AND EE.CreationDate > @monthendStartTime
AND ((@GivenUkprnCount = 0) OR  (EE.ukprn in (SELECT ids.ukprn FROM @ukprnList ids)))
AND (JobId in (SELECT JobId FROM @LatestJobIds))

--DAS Earnings

;WITH DatalockedEarnings AS (
    SELECT EE.LearnerReferenceNumber, EE.LearnerUln, EE.LearningAimFrameworkCode,
        EE.LearningAimPathwayCode, EE.LearningAimProgrammeType, EE.LearningAimReference,
        EE.LearningAimStandardCode, EE.Ukprn, EEP.Amount, EEP.DeliveryPeriod,
        EEP.TransactionType
    FROM Payments2.EarningEvent EE with (nolock)
    JOIN Payments2.EarningEventPeriod EEP with (nolock)
        ON EEP.EarningEventId = EE.EventId
    WHERE EXISTS (
        SELECT *
        FROM Payments2.DataLockEvent DLE with (nolock)
        
        JOIN Payments2.DataLockEventNonPayablePeriod  DLENP with (nolock)
            ON DLE.EventId = DLENP.DataLockEventId
        WHERE DLE.EarningEventId = EE.EventId
        AND DLENP.DeliveryPeriod = EEP.DeliveryPeriod
        AND DLENP.PriceEpisodeIdentifier = EEP.PriceEpisodeIdentifier
		AND (DLE.JobId in (SELECT JobId FROM @LatestJobIds))
    )
    AND DeliveryPeriod <= @collectionperiod
    AND Amount != 0
	 AND ((@GivenUkprnCount = 0) OR  (EE.ukprn in (SELECT ids.ukprn FROM @ukprnList ids)))
    AND EE.CreationDate > @monthendStartTime
	AND (ee.JobId in (SELECT JobId FROM @LatestJobIds))
)
, DatalockedPayments AS (
    SELECT *
    FROM Payments2.Payment P with (nolock)
    WHERE EXISTS (
        SELECT *
        FROM DatalockedEarnings E with (nolock)
        WHERE P.Ukprn = E.Ukprn
        AND P.LearnerReferenceNumber = E.LearnerReferenceNumber
        AND P.DeliveryPeriod = E.DeliveryPeriod
        AND E.LearningAimFrameworkCode = P.LearningAimFrameworkCode
        AND E.LearningAimPathwayCode = P.LearningAimPathwayCode
        AND E.LearningAimProgrammeType = P.LearningAimProgrammeType
        AND E.LearningAimReference = P.LearningAimReference
        AND E.LearningAimStandardCode = P.LearningAimStandardCode
        AND E.TransactionType = P.TransactionType
		AND P.AcademicYear = 1920
    )   
		 AND ((@GivenUkprnCount = 0) OR  (ukprn in (SELECT ids.ukprn FROM @ukprnList ids)))
)
SELECT (SELECT FORMAT(SUM(Amount), 'C', 'en-gb') FROM DatalockedEarnings) [Datalocked Earnigs],
    (SELECT FORMAT(SUM(Amount), 'C', 'en-gb') FROM DatalockedPayments) [Datalocked Payments],
    (SELECT FORMAT((SELECT SUM(Amount) FROM DatalockedEarnings) - (SELECT SUM(Amount) FROM DatalockedPayments), 'C', 'en-gb')) [Adjusted Datalocks]

--DataLock











