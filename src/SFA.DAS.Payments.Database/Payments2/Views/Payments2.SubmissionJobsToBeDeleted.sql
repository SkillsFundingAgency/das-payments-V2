CREATE VIEW [Payments2].[SubmissionJobsToBeDeleted]
AS
     SELECT DCJobId
     FROM Payments2.Job
     WHERE STATUS = 5 --get all jobs where STATUS is 5
     UNION
     SELECT J2.DCJobId
     FROM Payments2.Job J1 --new
     INNER JOIN Payments2.Job J2 --old
     ON J1.Ukprn = J2.Ukprn --must have same Ukprn
        AND J1.AcademicYear = J2.AcademicYear --must have same AcademicYear
        AND J1.CollectionPeriod = J2.CollectionPeriod --must have same CollectionPeriod
        AND J1.IlrSubmissionTime > J2.IlrSubmissionTime --new job must be in future
     WHERE J2.JobId IS NOT NULL
           AND J1.STATUS IN (2, 3, 4); -- get all old jobs where a new job exists with STATUS IN (2, 3, 4)