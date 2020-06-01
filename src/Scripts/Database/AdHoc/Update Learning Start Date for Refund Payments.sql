--SELECT COUNT(1) AS MissingLearningStartDate, [AcademicYear], [CollectionPeriod] 
--FROM [Payments2].[Payment]
--WHERE [LearningStartDate] IS NULL AND [Amount] < 0
--GROUP BY AcademicYear, CollectionPeriod

--SELECT COUNT(1) AS MissingLearningStartDate
--FROM [Payments2].[Payment]
--WHERE [LearningStartDate] is null AND [Amount] < 0


PRINT 'Started Updating All MissingLearningStartDate'

BEGIN TRAN

--first Positive payment that has non null start date from latest collection period after the refund payment
--i.e. if refund was in collection period 5 then find first matching payment from collection period 4 if non found then go to collection period 3, 2, 1


; WITH CTE AS (
SELECT P.Id, MO.LearningStartDate FROM [Payments2].[Payment]  AS P
CROSS APPLY
(SELECT TOP (1) LearningStartDate FROM [Payments2].[Payment] AS M -- <== this ensures we always get latest collection period
WHERE  
	m.[AcademicYear]			 = p.[AcademicYear]
AND m.[Ukprn]					 = p.[Ukprn]
AND m.[LearnerReferenceNumber] 	 = p.[LearnerReferenceNumber] 
AND m.[LearningAimReference]	 = p.[LearningAimReference]
AND m.[LearningAimProgrammeType] = p.[LearningAimProgrammeType]
AND m.[LearningAimStandardCode]  = p.[LearningAimStandardCode] 
AND m.[LearningAimFrameworkCode] = p.[LearningAimFrameworkCode]
AND m.[LearningAimPathwayCode]   = p.[LearningAimPathwayCode]
AND m.[DeliveryPeriod]			 = p.[DeliveryPeriod]
AND m.CollectionPeriod			 < p.CollectionPeriod 
AND M.CollectionPeriod			 < 5
AND m.LearningStartDate			 IS NOT NULL
ORDER BY M.CollectionPeriod DESC) AS MO 
WHERE P.Amount < 0 
AND   P.LearningStartDate IS NULL
)
UPDATE P
SET p.LearningStartDate = u.LearningStartDate
FROM [Payments2].[Payment]  AS P
INNER JOIN CTE as U ON p.id = u.id

PRINT 'Finished Updating All MissingLearningStartDate'

SELECT CONVERT(VARCHAR, COUNT(1)) + ' records have null start date' 
FROM [Payments2].[Payment] 
WHERE LearningStartDate is null 


ROLLBACK