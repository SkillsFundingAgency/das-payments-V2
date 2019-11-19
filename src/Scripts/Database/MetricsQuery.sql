DECLARE @monthendStartTime DATETIME = '2019-11-07 20:00'
DECLARE @startDate DATETIME = @monthendStartTime
DECLARE @collectionperiod INT = 3
DECLARE @ukprnFilter bigint = null



SELECT
	(SELECT FORMAT(SUM(Amount), 'C', 'en-gb')
	 FROM Payments2.RequiredPaymentEvent
	 WHERE CreationDate > @startDate
	 AND ((@ukprnFilter IS NULL) OR (Ukprn = @ukprnFilter))
	 ) [Required Payments made this month],

	(SELECT FORMAT(SUM(Amount), 'C', 'en-gb')
	 FROM Payments2.Payment
	 WHERE CreationDate < @startDate
	 AND AcademicYear = 1920
	 AND ((@ukprnFilter IS NULL) OR (Ukprn = @ukprnFilter))
	 ) [Payments made before this month YTD],

    (SELECT FORMAT((SELECT SUM(Amount) 
     FROM Payments2.RequiredPaymentEvent
     WHERE CreationDate > @startDate
	 AND ((@ukprnFilter IS NULL) OR (Ukprn = @ukprnFilter))
	 ) +
    (SELECT SUM(Amount) 
     FROM Payments2.Payment
     WHERE CreationDate < @startDate
	 AND ((@ukprnFilter IS NULL) OR (Ukprn = @ukprnFilter))
	 AND AcademicYear = 1920 ), 'C', 'en-gb') 
	) [Expected Payments YTD after running Period End],

	(SELECT FORMAT(SUM(Amount), 'C', 'en-gb')
     FROM Payments2.Payment
     where AcademicYear = 1920
	 and CollectionPeriod = @collectionperiod 
	 AND ((@ukprnFilter IS NULL) OR (Ukprn = @ukprnFilter))
	) [Total payments this month],

	--CASE WHEN IsLevyPayer = 0 THEN 1 ELSE 0 END
	(SELECT FORMAT(sum(CASE WHEN contracttype = 1 THEN amount ELSE 0 END), 'C', 'en-gb') 
     FROM Payments2.Payment
     where AcademicYear = 1920
	 AND ((@ukprnFilter IS NULL) OR (Ukprn = @ukprnFilter))
	 ) [Total ACT 1 payments YTD],

	(SELECT FORMAT(sum(CASE WHEN contracttype = 2 THEN amount ELSE 0 END), 'C', 'en-gb') 
     FROM Payments2.Payment
     where AcademicYear = 1920
	 AND ((@ukprnFilter IS NULL) OR (Ukprn = @ukprnFilter))
	 ) [Total ACT 2 payments YTD],

	(SELECT FORMAT(SUM(Amount), 'C', 'en-gb') 
     FROM Payments2.Payment
     where AcademicYear = 1920
	 AND ((@ukprnFilter IS NULL) OR (Ukprn = @ukprnFilter))
	 ) [Total payments YTD],

	(SELECT FORMAT(SUM(Amount), 'C', 'en-gb')
	 FROM Payments2.RequiredPaymentEvent
	 WHERE CreationDate > @startDate
	 and NonPaymentReason = 0
	 AND ((@ukprnFilter IS NULL) OR (Ukprn = @ukprnFilter))
	 ) [Held Back Completion Payments this month]
--Expected Payments

SELECT FORMAT(SUM(Amount), 'C', 'en-gb') as [DAS Earnings]
FROM Payments2.EarningEvent EE
JOIN Payments2.EarningEventPeriod EEP
    ON EEP.EarningEventId = EE.EventId
AND DeliveryPeriod <= @collectionperiod
AND Amount != 0
AND EE.CreationDate > @monthendStartTime
AND ((@ukprnFilter IS NULL) OR (EE.Ukprn = @ukprnFilter))

--DAS Earnings

;WITH DatalockedEarnings AS (
    SELECT EE.LearnerReferenceNumber, EE.LearnerUln, EE.LearningAimFrameworkCode,
        EE.LearningAimPathwayCode, EE.LearningAimProgrammeType, EE.LearningAimReference,
        EE.LearningAimStandardCode, EE.Ukprn, EEP.Amount, EEP.DeliveryPeriod,
        EEP.TransactionType
    FROM Payments2.EarningEvent EE
    JOIN Payments2.EarningEventPeriod EEP
        ON EEP.EarningEventId = EE.EventId
    WHERE EXISTS (
        SELECT *
        FROM Payments2.DataLockEvent DLE
        
        JOIN Payments2.DataLockEventNonPayablePeriod DLENP
            ON DLE.EventId = DLENP.DataLockEventId
        WHERE DLE.EarningEventId = EE.EventId
        AND DLENP.DeliveryPeriod = EEP.DeliveryPeriod
        AND DLENP.PriceEpisodeIdentifier = EEP.PriceEpisodeIdentifier
    )
    AND DeliveryPeriod <= @collectionperiod
    AND Amount != 0
	AND ((@ukprnFilter IS NULL) OR (EE.Ukprn = @ukprnFilter))
    AND EE.CreationDate > @monthendStartTime
)
, DatalockedPayments AS (
    SELECT *
    FROM Payments2.Payment P
    WHERE EXISTS (
        SELECT *
        FROM DatalockedEarnings E
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
		AND ((@ukprnFilter IS NULL) OR (p.Ukprn = @ukprnFilter))

)
SELECT (SELECT FORMAT(SUM(Amount), 'C', 'en-gb') FROM DatalockedEarnings) [Datalocked Earnigs],
    (SELECT FORMAT(SUM(Amount), 'C', 'en-gb') FROM DatalockedPayments) [Datalocked Payments],
    (SELECT FORMAT((SELECT SUM(Amount) FROM DatalockedEarnings) - (SELECT SUM(Amount) FROM DatalockedPayments), 'C', 'en-gb')) [Adjusted Datalocks]

--DataLock











