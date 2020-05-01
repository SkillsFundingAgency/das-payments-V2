CREATE VIEW [Payments2].[SubmissionJobsToBeDeleted]
AS
     SELECT DCJobId
     FROM Payments2.Job
     WHERE STATUS = 5 --get all jobs where STATUS is DCTasksFailed
     UNION
     SELECT OldJob.DCJobId
     FROM Payments2.Job NewJob
     INNER JOIN Payments2.Job OldJob
     ON NewJob.Ukprn = OldJob.Ukprn
        AND NewJob.AcademicYear = OldJob.AcademicYear
        AND NewJob.CollectionPeriod = OldJob.CollectionPeriod
        AND NewJob.IlrSubmissionTime > OldJob.IlrSubmissionTime --new job must be in future
        AND NewJob.[Status] IN (2, 3) -- when comparing jobs Only include new jobs with statu IN (Completed, CompletedWithErrors). i.e ignore InProgress, TimedOut, DcTasksFailed 
	    AND NewJob.DCJobSucceeded IS NOT NULL -- exclude all New jobs with DCJobSucceeded is null
	    AND OldJob.[Status] IN (2, 3) -- when comparing jobs Only include new jobs with statu IN (Completed, CompletedWithErrors). i.e ignore InProgress, TimedOut, DcTasksFailed 
	    AND OldJob.DCJobSucceeded IS NOT NULL -- exclude all old jobs with DCJobSucceeded is null
--compare old and new jobs with statu IN (Completed, CompletedWithErrors) and DCJobSucceeded IS NOT NULL, 
--this means it will ignore InProgress and DcTasksFailed regardless as well as ignore all jobs where DCJobSucceeded IS NULL
--and after that produces a list of DcJobIds to be removed
;