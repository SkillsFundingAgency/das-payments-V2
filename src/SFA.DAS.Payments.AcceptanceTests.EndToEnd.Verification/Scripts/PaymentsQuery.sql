-- DECLARE @monthendStartTime DATETIME = '2019-11-07 20:00'
-- DECLARE @startDate DATETIME = @monthendStartTime
-- DECLARE @collectionperiod INT = 3


SELECT
	(SELECT SUM(Amount)
	 FROM Payments2.RequiredPaymentEvent
	 WHERE CreationDate > @startDate
      and NonPaymentReason IS NULL
	 AND ukprn in (@ukprnList)
	 ) [Required Payments made this month],

	(SELECT SUM(Amount)
	 FROM Payments2.Payment
	 WHERE CreationDate < @startDate
	 AND AcademicYear = 1920
	 AND ukprn in (@ukprnList)
	 ) [Payments made before this month YTD],

    (SELECT (SELECT SUM(Amount) 
     FROM Payments2.RequiredPaymentEvent
     WHERE CreationDate > @startDate
       and NonPaymentReason IS NULL
	 AND ukprn in (@ukprnList)
	 ) +
    (SELECT ISNULL(SUM(Amount),0) 
     FROM Payments2.Payment
     WHERE CreationDate < @startDate
	 AND ukprn in (@ukprnList)
	 AND AcademicYear = 1920 ) 
	) [Expected Payments YTD after running Period End],

	(SELECT SUM(Amount)
     FROM Payments2.Payment
     where AcademicYear = 1920
	 and CollectionPeriod = @collectionperiod 
	 AND ukprn in (@ukprnList)
	) [Total payments this month],

	--CASE WHEN IsLevyPayer = 0 THEN 1 ELSE 0 END
	(SELECT sum(CASE WHEN contracttype = 1 THEN amount ELSE 0 END)
     FROM Payments2.Payment
     where AcademicYear = 1920
	 AND ukprn in (@ukprnList)
	 ) [Total ACT 1 payments YTD],

	(SELECT sum(CASE WHEN contracttype = 2 THEN amount ELSE 0 END) 
     FROM Payments2.Payment
     where AcademicYear = 1920
	 AND ukprn in (@ukprnList)
	 ) [Total ACT 2 payments YTD],

	(SELECT SUM(Amount) 
     FROM Payments2.Payment
     where AcademicYear = 1920
	 AND ukprn in (@ukprnList)
	 ) [Total payments YTD],

	(SELECT SUM(Amount)
	 FROM Payments2.RequiredPaymentEvent
	 WHERE CreationDate > @startDate
	 and NonPaymentReason = 0
	 AND ukprn in (@ukprnList)
	 ) [Held Back Completion Payments this month]
--Expected Payments

SELECT SUM(Amount) as [DAS Earnings]
FROM Payments2.EarningEvent EE
JOIN Payments2.EarningEventPeriod EEP
    ON EEP.EarningEventId = EE.EventId
AND DeliveryPeriod <= @collectionperiod
AND Amount != 0
AND EE.CreationDate > @monthendStartTime
AND EE.Ukprn in (@ukprnList)

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
	AND EE.Ukprn in (@ukprnList)
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
	and p.Ukprn in (@ukprnList)
)
SELECT (SELECT SUM(Amount) FROM DatalockedEarnings) [Datalocked Earnings],
    (SELECT SUM(Amount) FROM DatalockedPayments) [Datalocked Payments],
    (SELECT (SELECT ISNULL( SUM(Amount), 0) FROM DatalockedEarnings) - (SELECT ISNULL( SUM(Amount),0) FROM DatalockedPayments)) [Adjusted Datalocks]

--DataLock











