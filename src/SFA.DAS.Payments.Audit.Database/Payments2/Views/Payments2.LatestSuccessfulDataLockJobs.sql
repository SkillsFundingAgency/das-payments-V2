CREATE VIEW [Payments2].[LatestSuccessfulDataLockJobs] AS

WITH Validjobs AS
    (
        SELECT max(Ilrsubmissiontime) [IlrSubmissionTime], Ukprn, AcademicYear
        FROM Payments2.Job
        WHERE Status IN (1, 2, 3)
        AND Dcjobsucceeded = 1
        AND Jobtype = 1
        GROUP BY Ukprn, AcademicYear
    )
SELECT J.*
FROM Payments2.Job AS J
JOIN Validjobs AS Vj 
     ON J.Ilrsubmissiontime = Vj.Ilrsubmissiontime 
     AND J.Ukprn = Vj.Ukprn 
     AND J.AcademicYear = VJ.AcademicYear 
