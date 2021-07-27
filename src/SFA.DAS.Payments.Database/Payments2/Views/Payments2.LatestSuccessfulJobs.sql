CREATE VIEW [Payments2].[LatestSuccessfulJobs]
AS

WITH validJobs AS (
    SELECT MAX(CreationDate) AS CreationDate, Ukprn, AcademicYear
        FROM   Payments2.Job
        WHERE (Status IN (2, 3)) 
        AND (DCJobSucceeded = 1) 
        AND (JobType = 1)
        GROUP BY Ukprn, AcademicYear)
    SELECT j.*
  FROM   Payments2.Job AS j 
  INNER JOIN validJobs AS vj 
    ON j.CreationDate = vj.CreationDate 
    AND j.Ukprn = vj.Ukprn 
    AND j.AcademicYear = vj.AcademicYear

