CREATE VIEW [Payments2].[LatestSuccessfulJobs]
AS

WITH validJobs AS (
    SELECT MAX(IlrSubmissionTime) AS IlrSubmissionTime, Ukprn, AcademicYear
        FROM   Payments2.Job
        WHERE (Status IN (2, 3)) 
        AND (DCJobSucceeded = 1) 
        AND (JobType = 1)
        GROUP BY Ukprn, AcademicYear)
    SELECT j.*
  FROM   Payments2.Job AS j 
  INNER JOIN validJobs AS vj 
    ON j.IlrSubmissionTime = vj.IlrSubmissionTime 
    AND j.Ukprn = vj.Ukprn 
    AND j.AcademicYear = vj.AcademicYear

