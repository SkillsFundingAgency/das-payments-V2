CREATE VIEW [Payments2].[LatestSuccessfulJobs]
AS

with validJobs as (
	Select
		max(IlrSubmissionTime) [IlrSubmissionTime],
		Ukprn
	from Payments2.Job
	where status in (2,3) 
		and DCJobSucceeded = 1
		and JobType = 1 
	group by Ukprn
)

Select
	j.*
from Payments2.Job j
join validJobs vj 
	on j.IlrSubmissionTime = vj.IlrSubmissionTime
	AND J.Ukprn = VJ.Ukprn

