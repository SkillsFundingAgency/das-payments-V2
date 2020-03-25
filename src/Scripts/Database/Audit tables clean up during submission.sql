declare @collectionPeriod as tinyint = 8
declare @academicYear as smallint = 1920

IF OBJECT_ID('tempdb..#JobDataToBeDeleted') IS NOT NULL DROP TABLE #JobDataToBeDeleted

SELECT DISTINCT JobId INTO #JobDataToBeDeleted FROM Payments2.EarningEvent 
Where CollectionPeriod = @collectionPeriod AND AcademicYear = @academicYear
UNION
SELECT DISTINCT JobId FROM Payments2.RequiredPaymentEvent 
Where CollectionPeriod = @collectionPeriod AND AcademicYear = @academicYear
UNION
SELECT DISTINCT JobId FROM Payments2.FundingSourceEvent 
Where CollectionPeriod = @collectionPeriod AND AcademicYear = @academicYear
UNION
SELECT DISTINCT JobId FROM Payments2.DataLockEvent 
Where CollectionPeriod = @collectionPeriod AND AcademicYear = @academicYear

--keep all sucessful jobs and all in progress jobs, 
DELETE FROM #JobDataToBeDeleted WHERE JobId in (SELECT DcJobId FROM Payments2.LatestSuccessfulDataLockJobs)

SELECT EventId INTO #EarningEventIdsToDelete 
FROM (
    SELECT EventId FROM Payments2.EarningEvent EE
    WHERE EE.JobId IN (SELECT JobId FROM #JobDataToBeDeleted)
    AND CollectionPeriod = @collectionPeriod
    AND AcademicYear = @academicYear
) q
DELETE Payments2.EarningEventPeriod
WHERE EarningEventId IN (
    SELECT EventId FROM #EarningEventIdsToDelete
)
DELETE Payments2.EarningEventPriceEpisode
WHERE EarningEventId IN (
    SELECT EventId FROM #EarningEventIdsToDelete
)
DELETE Payments2.EarningEvent
WHERE EventId IN (
    SELECT EventId FROM #EarningEventIdsToDelete
)

SELECT EventId INTO #RequiredPaymentsToDelete
FROM (
	SELECT EventId 
    FROM Payments2.RequiredPaymentEvent
    WHERE JobId IN (
	    SELECT JobId FROM #JobDataToBeDeleted
    )
    AND CollectionPeriod = @collectionPeriod
    AND AcademicYear = @academicYear
) q​
​
DELETE Payments2.FundingSourceEvent
WHERE RequiredPaymentEventId IN (
    SELECT EventId FROM #RequiredPaymentsToDelete
)
​
DELETE Payments2.RequiredPaymentEvent
WHERE EventId IN (
	SELECT EventId FROM #RequiredPaymentsToDelete
)
​
SELECT EventId INTO #DatalocksToDelete
FROM (
	SELECT EventId FROM Payments2.DataLockEvent DLE
	WHERE DLE.JobId IN (SELECT JobId FROM #JobDataToBeDeleted)
	AND CollectionPeriod = @collectionPeriod
    AND AcademicYear = @academicYear
) q

DELETE Payments2.DataLockEventNonPayablePeriodFailures
WHERE DataLockEventNonPayablePeriodId IN (
	SELECT DataLockEventNonPayablePeriodId FROM Payments2.DataLockEventNonPayablePeriod
	WHERE DataLockEventId IN (
		SELECT EventId FROM #DatalocksToDelete
	)
)

DELETE Payments2.DataLockEventNonPayablePeriod
WHERE DataLockEventId IN (
	SELECT EventId FROM #DatalocksToDelete
)

DELETE Payments2.DataLockEventPayablePeriod
WHERE DataLockEventId IN (
	SELECT EventId FROM #DatalocksToDelete
)

DELETE Payments2.DataLockEventPriceEpisode
WHERE DataLockEventId IN (
	SELECT EventId FROM #DatalocksToDelete
)

DELETE Payments2.DataLockEvent
WHERE EventId IN (
	SELECT EventId FROM #DatalocksToDelete
)