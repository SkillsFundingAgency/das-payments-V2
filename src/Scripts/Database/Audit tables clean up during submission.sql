begin tran

print 'start ' + convert(varchar(150), SYSDATETIME())

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

print 'select jobid finished ' + convert(varchar(150), SYSDATETIME())

IF OBJECT_ID('tempdb..#EarningEventIdsToDelete') IS NOT NULL DROP TABLE #EarningEventIdsToDelete

SELECT EventId INTO #EarningEventIdsToDelete 
FROM (
    SELECT EventId FROM Payments2.EarningEvent EE
    WHERE EE.JobId IN (SELECT JobId FROM #JobDataToBeDeleted)
    AND CollectionPeriod = @collectionPeriod
    AND AcademicYear = @academicYear
) q

print 'select earningeventid finished ' + convert(varchar(150), SYSDATETIME())

DELETE Payments2.EarningEventPeriod
WHERE EarningEventId IN (
    SELECT EventId FROM #EarningEventIdsToDelete
)

print 'delete EarningEventPeriod finished ' + convert(varchar(150), SYSDATETIME())

DELETE Payments2.EarningEventPriceEpisode
WHERE EarningEventId IN (
    SELECT EventId FROM #EarningEventIdsToDelete
)

print 'delete EarningEventPriceEpisode finished ' + convert(varchar(150), SYSDATETIME())

DELETE Payments2.EarningEvent
WHERE EventId IN (
    SELECT EventId FROM #EarningEventIdsToDelete
)

print 'delete EarningEvent finished ' + convert(varchar(150), SYSDATETIME())

IF OBJECT_ID('tempdb..#RequiredPaymentsToDelete') IS NOT NULL DROP TABLE #RequiredPaymentsToDelete

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
print 'SELECT RequiredPaymentEventid finished ' + convert(varchar(150), SYSDATETIME())

DELETE Payments2.FundingSourceEvent
WHERE RequiredPaymentEventId IN (
    SELECT EventId FROM #RequiredPaymentsToDelete
)
​
print 'delete FundingSourceEvent finished ' + convert(varchar(150), SYSDATETIME())

DELETE Payments2.RequiredPaymentEvent
WHERE EventId IN (
	SELECT EventId FROM #RequiredPaymentsToDelete
)
​
print 'delete RequiredPaymentEvent finished ' + convert(varchar(150), SYSDATETIME())

IF OBJECT_ID('tempdb..#DatalocksToDelete') IS NOT NULL DROP TABLE #DatalocksToDelete
​
SELECT EventId INTO #DatalocksToDelete
FROM (
	SELECT EventId FROM Payments2.DataLockEvent DLE
	WHERE DLE.JobId IN (SELECT JobId FROM #JobDataToBeDeleted)
	AND CollectionPeriod = @collectionPeriod
    AND AcademicYear = @academicYear
) q

print 'SELECT DataLockEventid finished ' + convert(varchar(150), SYSDATETIME())

DELETE Payments2.DataLockEventNonPayablePeriodFailures
WHERE DataLockEventNonPayablePeriodId IN (
	SELECT DataLockEventNonPayablePeriodId FROM Payments2.DataLockEventNonPayablePeriod
	WHERE DataLockEventId IN (
		SELECT EventId FROM #DatalocksToDelete
	)
)

print 'delete DataLockEventNonPayablePeriodFailures finished ' + convert(varchar(150), SYSDATETIME())

DELETE Payments2.DataLockEventNonPayablePeriod
WHERE DataLockEventId IN (
	SELECT EventId FROM #DatalocksToDelete
)

print 'delete DataLockEventNonPayablePeriod finished ' + convert(varchar(150), SYSDATETIME())

DELETE Payments2.DataLockEventPayablePeriod
WHERE DataLockEventId IN (
	SELECT EventId FROM #DatalocksToDelete
)

print 'delete DataLockEventPayablePeriod finished ' + convert(varchar(150), SYSDATETIME())

DELETE Payments2.DataLockEventPriceEpisode
WHERE DataLockEventId IN (
	SELECT EventId FROM #DatalocksToDelete
)

print 'delete DataLockEventPriceEpisode finished ' + convert(varchar(150), SYSDATETIME())

DELETE Payments2.DataLockEvent
WHERE EventId IN (
	SELECT EventId FROM #DatalocksToDelete
)

print 'delete DataLockEvent finished ' + convert(varchar(150), SYSDATETIME())

rollback tran