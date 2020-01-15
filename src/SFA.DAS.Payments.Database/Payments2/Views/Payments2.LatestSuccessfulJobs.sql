CREATE VIEW [Payments2].[LatestSuccessfulJobs]
AS

with validJobs (DcJobId, Ukprn)
as
(
Select
max(DcJobId),
Ukprn
from Payments2.Job
where status in (2,3)
group by Ukprn
)
Select
j.*
from Payments2.Job j
join validJobs vj on j.DCJobId = vj.DcJobId

