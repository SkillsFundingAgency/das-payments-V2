PRINT 'start ' + CONVERT(VARCHAR(150), SYSDATETIME());

DECLARE @collectionPeriod AS TINYINT= 8;
DECLARE @academicYear AS SMALLINT= 1920;

IF OBJECT_ID('tempdb..#JobDataToBeDeleted') IS NOT NULL DROP TABLE #JobDataToBeDeleted;

SELECT DISTINCT JobId INTO #JobDataToBeDeleted FROM Payments2.EarningEvent
WHERE CollectionPeriod = @collectionPeriod AND AcademicYear = @academicYear
UNION
SELECT DISTINCT JobId FROM Payments2.RequiredPaymentEvent
WHERE CollectionPeriod = @collectionPeriod AND AcademicYear = @academicYear
UNION
SELECT DISTINCT JobId FROM Payments2.FundingSourceEvent
WHERE CollectionPeriod = @collectionPeriod AND AcademicYear = @academicYear
UNION
SELECT DISTINCT JobId FROM Payments2.DataLockEvent
WHERE CollectionPeriod = @collectionPeriod AND AcademicYear = @academicYear;

-- keep all sucessful Jobs, 
DELETE FROM #JobDataToBeDeleted WHERE JobId IN ( SELECT DcJobId FROM Payments2.LatestSuccessfulJobs );

-- and Keep all in progress and Timed-out and Failed on DC Jobs
DELETE FROM #JobDataToBeDeleted WHERE JobId IN ( SELECT DcJobId FROM Payments2.Job WHERE [Status] in (1, 4, 5) );

-- and any jobs completed on our side but DC status is unknown
DELETE FROM #JobDataToBeDeleted WHERE JobId IN ( SELECT DcJobId FROM Payments2.Job Where [Status] in (2, 3) AND Dcjobsucceeded IS NULL );

PRINT 'Final list includes Jobs not in Payments2.Job '

SELECT JobId FROM #JobDataToBeDeleted

BEGIN TRAN;

PRINT 'select jobid finished ' + CONVERT(VARCHAR(150), SYSDATETIME());

IF OBJECT_ID('tempdb..#EarningEventIdsToDelete') IS NOT NULL DROP TABLE #EarningEventIdsToDelete;

SELECT EventId INTO #EarningEventIdsToDelete
FROM
(
    SELECT EventId FROM Payments2.EarningEvent EE
    WHERE EE.JobId IN ( SELECT JobId FROM #JobDataToBeDeleted )
    AND CollectionPeriod = @collectionPeriod AND AcademicYear = @academicYear
) q;

PRINT 'select earningeventid finished ' + CONVERT(VARCHAR(150), SYSDATETIME());

DELETE Payments2.EarningEventPeriod
WHERE EarningEventId IN ( SELECT EventId FROM #EarningEventIdsToDelete );

PRINT 'delete EarningEventPeriod finished ' + CONVERT(VARCHAR(150), SYSDATETIME());

DELETE Payments2.EarningEventPriceEpisode
WHERE EarningEventId IN ( SELECT EventId FROM #EarningEventIdsToDelete );

PRINT 'delete EarningEventPriceEpisode finished ' + CONVERT(VARCHAR(150), SYSDATETIME());

DELETE Payments2.EarningEvent
WHERE EventId IN ( SELECT EventId FROM #EarningEventIdsToDelete );

PRINT 'delete EarningEvent finished ' + CONVERT(VARCHAR(150), SYSDATETIME());

COMMIT TRAN;
--ROLLBACK TRAN;

BEGIN TRAN;

IF OBJECT_ID('tempdb..#RequiredPaymentsToDelete') IS NOT NULL DROP TABLE #RequiredPaymentsToDelete;

SELECT EventId INTO #RequiredPaymentsToDelete
FROM
(
    SELECT EventId FROM Payments2.RequiredPaymentEvent 
	WHERE JobId IN ( SELECT JobId FROM #JobDataToBeDeleted )
          AND CollectionPeriod = @collectionPeriod
          AND AcademicYear = @academicYear
) q;​
​
PRINT 'SELECT RequiredPaymentEventid finished ' + CONVERT(VARCHAR(150), SYSDATETIME());

DELETE Payments2.FundingSourceEvent
WHERE RequiredPaymentEventId IN ( SELECT EventId FROM #RequiredPaymentsToDelete );
​
PRINT 'delete FundingSourceEvent finished ' + CONVERT(VARCHAR(150), SYSDATETIME());

DELETE Payments2.RequiredPaymentEvent
WHERE EventId IN ( SELECT EventId FROM #RequiredPaymentsToDelete );
​
PRINT 'delete RequiredPaymentEvent finished ' + CONVERT(VARCHAR(150), SYSDATETIME());

COMMIT TRAN;
--ROLLBACK TRAN;

BEGIN TRAN;

IF OBJECT_ID('tempdb..#DatalocksToDelete') IS NOT NULL DROP TABLE #DatalocksToDelete;
​
SELECT EventId INTO #DatalocksToDelete
FROM
(
	SELECT EventId FROM Payments2.DataLockEvent DLE
	WHERE DLE.JobId IN(SELECT JobId FROM #JobDataToBeDeleted)
		AND CollectionPeriod = @collectionPeriod
		AND AcademicYear = @academicYear
) q;

PRINT 'SELECT DataLockEventid finished ' + CONVERT(VARCHAR(150), SYSDATETIME());

DELETE Payments2.DataLockEventNonPayablePeriodFailures
WHERE DataLockEventNonPayablePeriodId IN
(
    SELECT DataLockEventNonPayablePeriodId FROM Payments2.DataLockEventNonPayablePeriod
    WHERE DataLockEventId IN ( SELECT EventId FROM #DatalocksToDelete )
);

PRINT 'delete DataLockEventNonPayablePeriodFailures finished ' + CONVERT(VARCHAR(150), SYSDATETIME());

DELETE Payments2.DataLockEventNonPayablePeriod
WHERE DataLockEventId IN ( SELECT EventId FROM #DatalocksToDelete );

PRINT 'delete DataLockEventNonPayablePeriod finished ' + CONVERT(VARCHAR(150), SYSDATETIME());

DELETE Payments2.DataLockEventPayablePeriod
WHERE DataLockEventId IN ( SELECT EventId FROM #DatalocksToDelete );

PRINT 'delete DataLockEventPayablePeriod finished ' + CONVERT(VARCHAR(150), SYSDATETIME());

DELETE Payments2.DataLockEventPriceEpisode
WHERE DataLockEventId IN ( SELECT EventId FROM #DatalocksToDelete );

PRINT 'delete DataLockEventPriceEpisode finished ' + CONVERT(VARCHAR(150), SYSDATETIME());

DELETE Payments2.DataLockEvent
WHERE EventId IN ( SELECT EventId FROM #DatalocksToDelete );

PRINT 'delete DataLockEvent finished ' + CONVERT(VARCHAR(150), SYSDATETIME());

COMMIT TRAN;
--ROLLBACK TRAN;
