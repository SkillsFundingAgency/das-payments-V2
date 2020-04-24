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
     WHERE OldJob.JobId IS NOT NULL
           AND NewJob.STATUS IN (2, 3, 4); -- get all old jobs where a new job exists with STATUS IN (Completed, CompletedWithErrors, TimedOut)