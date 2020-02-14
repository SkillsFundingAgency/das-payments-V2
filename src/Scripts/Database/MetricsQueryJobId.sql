SET STATISTICS TIME ON;  
GO 

DECLARE @collectionperiod INT = 6
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
	UkPrn bigint not null
)

INSERT INTO @LatestJobIds
SELECT DcJobId, Ukprn
FROM Payments2.LatestSuccessfulJobs


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


Declare @CurrentRequiredPayments decimal(15,5)
Declare @CurrentRequiredPaymentsHeldBack decimal(15,5)
Declare @PrevPayments decimal(15,5)
Declare @CurrentPayments decimal(15,5)
Declare @CurrentPaymentsAct1 decimal(15,5)
Declare @CurrentPaymentsAct2 decimal(15,5)
Declare @TotalPayments decimal(15,5)
Declare @DatalockedPayments decimal(15,5)
Declare @DatalockedEarnings decimal(15,5)



--Required payment table  totals
select  
	@CurrentRequiredPayments = notHB,
	@CurrentRequiredPaymentsHeldBack = HbCompletion
from
(SELECT  Sum(case when RPE.NonPaymentReason is null then Amount else 0 end) as notHB,  Sum(case when RPE.NonPaymentReason is not null then Amount else 0 end) as HbCompletion -- current required payments
	 FROM Payments2.RequiredPaymentEvent RPE with (nolock)
	 JOIN @LatestJobIds lji
		ON lji.JobId = RPE.JobId
	 WHERE RPE.CollectionPeriod = @collectionperiod
	) RPs



--payment table totals
SELECT 
	@PrevPayments=previousPayments,
	@CurrentPayments=currentPayments,
	@CurrentPaymentsAct1=currentPaymentsAct1,
	@CurrentPaymentsAct2=currentPaymentsAct2,
	@TotalPayments = totalPayments
FROM
	(SELECT
		Sum(Amount) as totalPayments,
		Sum(Case when CollectionPeriod < @collectionperiod then Amount else 0 end) as previousPayments,--previous payments  1920
		Sum(Case when CollectionPeriod = @collectionperiod  then Amount else 0 end) as currentPayments,--current  1920
		Sum(Case when contracttype = 1 then Amount else 0 end) as currentPaymentsAct1,--current aCT1 1920
		Sum(Case when contracttype = 2 then Amount else 0 end) as currentPaymentsAct2--current   ACT2 1920
	 FROM Payments2.Payment with (nolock)
	 WHERE AcademicYear = 1920
	 AND ((@GivenUkprnCount = 0) OR  (ukprn in (SELECT ids.ukprn FROM @ukprnList ids)))
	 )Payments
	
	
--payment summaries
SELECT 
FORMAT(@CurrentRequiredPayments, 'C', 'en-gb') AS [Required Payments made this month],
FORMAT(@PrevPayments, 'C', 'en-gb') AS [Payments made before this month YTD],
FORMAT(@CurrentRequiredPayments + @PrevPayments, 'C', 'en-gb') AS [Expected Payments YTD after running Period End],
FORMAT(@CurrentPayments, 'C', 'en-gb') AS [Total payments this month],
FORMAT(@CurrentPaymentsAct1, 'C', 'en-gb') AS [Total ACT 1 payments YTD],
FORMAT(@CurrentPaymentsAct2, 'C', 'en-gb') AS [Total ACT 2 payments YTD],
FORMAT(@TotalPayments, 'C', 'en-gb') AS [Total payments YTD],
FORMAT(@CurrentRequiredPaymentsHeldBack, 'C', 'en-gb') AS [Held Back Completion Payments this month]


----DAS earnings
SELECT FORMAT(SUM(Amount), 'C', 'en-gb') as [DAS Earnings]
FROM Payments2.EarningEvent EE with (nolock)
JOIN Payments2.EarningEventPeriod EEP with (nolock)
    ON EEP.EarningEventId = EE.EventId
JOIN @LatestJobIds lji
	ON lji.JobId = EE.JobId
AND DeliveryPeriod <= @collectionperiod
AND EE.CollectionPeriod = @CollectionPeriod



IF OBJECT_ID('tempdb..#DataLockedEarnings') IS NOT NULL DROP TABLE #DataLockedEarnings


--Datalocks

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
        JOIN Payments2.DataLockEventNonPayablePeriod DLENP with (nolock)
            ON DLE.EventId = DLENP.DataLockEventId
        WHERE DLE.EarningEventId = EE.EventId
        AND DLENP.DeliveryPeriod = EEP.DeliveryPeriod
        AND DLENP.PriceEpisodeIdentifier = EEP.PriceEpisodeIdentifier
		AND (DLE.JobId in (SELECT JobId FROM @LatestJobIds))
    )
    AND DeliveryPeriod <= @collectionperiod
    AND Amount != 0
	 AND ((@GivenUkprnCount = 0) OR  (EE.ukprn in (SELECT ids.ukprn FROM @ukprnList ids)))
    AND EE.CollectionPeriod = @collectionperiod
	AND (ee.JobId in (SELECT JobId FROM @LatestJobIds))
)

SELECT * into #DataLockedEarnings FROM DatalockedEarnings


set @DatalockedEarnings = (SELECT SUM(Amount)FROM #DatalockedEarnings)
set @DatalockedPayments = (
SELECT Sum(amount)
    FROM Payments2.Payment P with (nolock)
    WHERE EXISTS (
        SELECT *
        FROM #DatalockedEarnings E with (nolock)
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

SELECT 
	FORMAT(@DatalockedEarnings, 'C', 'en-gb') AS [Datalocked Earnings],
	FORMAT(@DatalockedPayments, 'C', 'en-gb') AS [Datalocked Payments],
	FORMAT(@DatalockedEarnings - @DatalockedPayments, 'C', 'en-gb') AS [Adjusted Datalocks]


	
IF OBJECT_ID('tempdb..#DataLockedEarnings') IS NOT NULL DROP TABLE #DataLockedEarnings
