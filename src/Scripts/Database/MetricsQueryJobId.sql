DECLARE @collectionperiod INT = 6
DECLARE @ukprnList TABLE (ukprn BIGINT NOT NULL PRIMARY KEY CLUSTERED)
--INSERT INTO @ukprnList --whitelist of suceeded ukprns if required
--VALUES
--(10004285)
DECLARE @GivenUkprnCount INT = (
		SELECT COUNT(1)
		FROM @ukprnList
		)
DECLARE @LatestJobIds TABLE (
	JobId INT NOT NULL PRIMARY KEY CLUSTERED
	, UkPrn BIGINT NOT NULL
	)

INSERT INTO @LatestJobIds
SELECT DcJobId
	, Ukprn
FROM Payments2.LatestSuccessfulJobs

IF (@GivenUkprnCount > 0)
BEGIN
	DELETE
	FROM @LatestJobIds
	WHERE UkPrn NOT IN (
			SELECT UkPrn
			FROM @ukprnList
			)
END

DECLARE @CurrentRequiredPayments DECIMAL(15, 5)
DECLARE @CurrentRequiredPaymentsHeldBack DECIMAL(15, 5)
DECLARE @PrevPayments DECIMAL(15, 5)
DECLARE @CurrentPayments DECIMAL(15, 5)
DECLARE @CurrentPaymentsAct1 DECIMAL(15, 5)
DECLARE @CurrentPaymentsAct2 DECIMAL(15, 5)
DECLARE @TotalPayments DECIMAL(15, 5)
DECLARE @DatalockedPayments DECIMAL(15, 5)
DECLARE @DatalockedEarnings DECIMAL(15, 5)

--Required payment table  totals
SELECT @CurrentRequiredPayments = notHB
	, @CurrentRequiredPaymentsHeldBack = HbCompletion
FROM (
	SELECT Sum(CASE WHEN RPE.NonPaymentReason IS NULL THEN Amount ELSE 0 END) AS notHB
		, Sum(CASE WHEN RPE.NonPaymentReason IS NOT NULL THEN Amount ELSE 0 END) AS HbCompletion -- current required payments
	FROM Payments2.RequiredPaymentEvent RPE WITH (NOLOCK)
	INNER JOIN @LatestJobIds lji
		ON lji.JobId = RPE.JobId
	WHERE RPE.CollectionPeriod = @collectionperiod
		AND RPE.NonPaymentReason IS NULL
	) RPs

--payment table totals
SELECT @PrevPayments = previousPayments
	, @CurrentPayments = currentPayments
	, @CurrentPaymentsAct1 = currentPaymentsAct1
	, @CurrentPaymentsAct2 = currentPaymentsAct2
	, @TotalPayments = totalPayments
FROM (
	SELECT Sum(Amount) AS totalPayments
		, Sum(CASE WHEN CollectionPeriod < @collectionperiod THEN Amount ELSE 0 END) AS previousPayments
		, --previous payments  1920
		Sum(CASE WHEN CollectionPeriod = @collectionperiod THEN Amount ELSE 0 END) AS currentPayments
		, --current  1920
		Sum(CASE WHEN contracttype = 1 THEN Amount ELSE 0 END) AS currentPaymentsAct1
		, --current aCT1 1920
		Sum(CASE WHEN contracttype = 2 THEN Amount ELSE 0 END) AS currentPaymentsAct2 --current   ACT2 1920
	FROM Payments2.Payment WITH (NOLOCK)
	WHERE AcademicYear = 1920
		AND (
			(@GivenUkprnCount = 0)
			OR (
				ukprn IN (
					SELECT ids.ukprn
					FROM @ukprnList ids
					)
				)
			)
	) Payments

--payment summaries
SELECT FORMAT(@CurrentRequiredPayments, 'C', 'en-gb') AS [Required Payments made this month]
	, FORMAT(@PrevPayments, 'C', 'en-gb') AS [Payments made before this month YTD]
	, FORMAT(@CurrentRequiredPayments + @PrevPayments, 'C', 'en-gb') AS [Expected Payments YTD after running Period End]
	, FORMAT(@CurrentPayments, 'C', 'en-gb') AS [Total payments this month]
	, FORMAT(@CurrentPaymentsAct1, 'C', 'en-gb') AS [Total ACT 1 payments YTD]
	, FORMAT(@CurrentPaymentsAct2, 'C', 'en-gb') AS [Total ACT 2 payments YTD]
	, FORMAT(@TotalPayments, 'C', 'en-gb') AS [Total payments YTD]
	, FORMAT(@CurrentRequiredPaymentsHeldBack, 'C', 'en-gb') AS [Held Back Completion Payments this month]

----DAS earnings
SELECT FORMAT(SUM(Amount), 'C', 'en-gb') AS [DAS Earnings]
FROM Payments2.EarningEvent EE WITH (NOLOCK)
INNER JOIN Payments2.EarningEventPeriod EEP WITH (NOLOCK)
	ON EEP.EarningEventId = EE.EventId
INNER JOIN @LatestJobIds lji
	ON lji.JobId = EE.JobId
		AND DeliveryPeriod <= @collectionperiod
		AND EE.CollectionPeriod = @CollectionPeriod

IF OBJECT_ID('tempdb..#DataLockedEarnings') IS NOT NULL
	DROP TABLE #DataLockedEarnings
		--Datalocks
		;

WITH DatalockedEarnings
AS (
	SELECT EE.LearnerReferenceNumber
		, EE.LearnerUln
		, EE.LearningAimFrameworkCode
		, EE.LearningAimPathwayCode
		, EE.LearningAimProgrammeType
		, EE.LearningAimReference
		, EE.LearningAimStandardCode
		, EE.Ukprn
		, EEP.Amount
		, EEP.DeliveryPeriod
		, EEP.TransactionType
	FROM Payments2.EarningEvent EE WITH (NOLOCK)
	INNER JOIN Payments2.EarningEventPeriod EEP WITH (NOLOCK)
		ON EEP.EarningEventId = EE.EventId
	WHERE EXISTS (
			SELECT *
			FROM Payments2.DataLockEvent DLE WITH (NOLOCK)
			INNER JOIN Payments2.DataLockEventNonPayablePeriod DLENP WITH (NOLOCK)
				ON DLE.EventId = DLENP.DataLockEventId
			WHERE DLE.EarningEventId = EE.EventId
				AND DLENP.DeliveryPeriod = EEP.DeliveryPeriod
				AND DLENP.PriceEpisodeIdentifier = EEP.PriceEpisodeIdentifier
				AND (
					DLE.JobId IN (
						SELECT JobId
						FROM @LatestJobIds
						)
					)
			)
		AND DeliveryPeriod <= @collectionperiod
		AND Amount != 0
		AND (
			(@GivenUkprnCount = 0)
			OR (
				EE.ukprn IN (
					SELECT ids.ukprn
					FROM @ukprnList ids
					)
				)
			)
		AND EE.CollectionPeriod = @collectionperiod
		AND (
			ee.JobId IN (
				SELECT JobId
				FROM @LatestJobIds
				)
			)
	)
SELECT *
INTO #DataLockedEarnings
FROM DatalockedEarnings

SET @DatalockedEarnings = (
		SELECT SUM(Amount)
		FROM #DatalockedEarnings
		)
SET @DatalockedPayments = (
		SELECT Sum(amount)
		FROM Payments2.Payment P WITH (NOLOCK)
		WHERE EXISTS (
				SELECT *
				FROM #DatalockedEarnings E WITH (NOLOCK)
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
			AND (
				(@GivenUkprnCount = 0)
				OR (
					ukprn IN (
						SELECT ids.ukprn
						FROM @ukprnList ids
						)
					)
				)
		)

SELECT FORMAT(@DatalockedEarnings, 'C', 'en-gb') AS [Datalocked Earnings]
	, FORMAT(@DatalockedPayments, 'C', 'en-gb') AS [Datalocked Payments]
	, FORMAT(@DatalockedEarnings - @DatalockedPayments, 'C', 'en-gb') AS [Adjusted Datalocks]

IF OBJECT_ID('tempdb..#DataLockedEarnings') IS NOT NULL
	DROP TABLE #DataLockedEarnings
