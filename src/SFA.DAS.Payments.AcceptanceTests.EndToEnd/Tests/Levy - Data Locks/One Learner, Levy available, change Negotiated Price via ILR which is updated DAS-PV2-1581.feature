Feature: One Learner, Levy available, change Negotiated Price via ILR which is updated DAS-PV2-1581_A
	Levy learner, levy available, and there is a change to the Negotiated Price via the ILR which is updated in DAS

Scenario Outline: One Learner, Levy available, change Negotiated Price via ILR which is updated DAS-PV2-1581_A
	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
	And the following commitments exist
		| Identifier       | start date                   | end date                  | agreed price | status | StandardCode | Framework Code | Pathway Code | Programme Type | effective from               | effective to                 |
		| Apprenticeship a | 01/Aug/Current Academic Year | 01/Aug/Next Academic Year | 12000        | active | 99           | 0              | 0            | 25             | 01/Aug/Current Academic Year | 31/Oct/Current Academic Year |
	And  the provider is providing training for the following learners
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | StandardCode | Funding Line Type                                                 | SFA Contribution Percentage |
		| 01/Aug/Current Academic Year | 12 months        | 12000                | 01/Aug/Current Academic Year        | 3000                   | 01/Aug/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 0              | 0            | 25             | 99           | 16-18 Apprenticeship (From May 2017) Levy Contract (non-procured) | 90%                         |
	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
		| pe-1             | 12000                | 01/Aug/Current Academic Year        | 3000                   | 01/Aug/Current Academic Year          | Act1          | 1                   | 90%                         |
		| pe-2             | 7500                 | 01/Nov/Current Academic Year        | 1875                   | 01/Nov/Current Academic Year          | Act1          | 1                   | 90%                         |
	When the ILR file is submitted for the learners for collection period <Collection_Period>

	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| Aug/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Sep/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Oct/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Nov/Current Academic Year | 500          | 0          | 0         | pe-2                     |
		| Dec/Current Academic Year | 500          | 0          | 0         | pe-2                     |
		| Jan/Current Academic Year | 500          | 0          | 0         | pe-2                     |
		| Feb/Current Academic Year | 500          | 0          | 0         | pe-2                     |
		| Mar/Current Academic Year | 500          | 0          | 0         | pe-2                     |
		| Apr/Current Academic Year | 500          | 0          | 0         | pe-2                     |
		| May/Current Academic Year | 500          | 0          | 0         | pe-2                     |
		| Jun/Current Academic Year | 500          | 0          | 0         | pe-2                     |
		| Jul/Current Academic Year | 500          | 0          | 0         | pe-2                     |

	And ensure that all the price events submitted in the ILR are published to the Apprenticeship Service
		| AimSeqNumber | Price Episode Identifier | IsPayable | Apprenticeship Id | Ilr StartDate                | Ilr StandardCode | Ilr Programme Type | Ilr Framework Code | Ilr Pathway Code | Ilr Training Price | Ilr EndpointAssesor Price | Ilr Price EffectiveFromDate  | Ilr Price EffectiveToDate    | Status |
		| 1            | pe-1                     | 1         | Apprenticeship a  | 01/Aug/Current Academic Year | 99               | 25                 | 0                  | 0                | 12000              | 0                         | 01/Aug/Current Academic Year | 31/Oct/Current Academic Year | 1      |
		| 1            | pe-2                     | 0         | Apprenticeship a  | 01/Aug/Current Academic Year | 99               | 25                 | 0                  | 0                | 9375               | 0                         | 01/Nov/Current Academic Year |                              | 1      |
	
	And ensure only following DataLock Event Errors are published to the Apprenticeship Service
		| Price Episode Identifier | ErrorCode |
		| pe-2                     | DLOCK_07  |












		 
#	And new Apprenticeship Price Episode is Added to the Apprenticeship from Approvals DataLock Triage Event
#		| Identifier       | agreed price | effective from               | effective to |
#		| Apprenticeship a | 9375         | 01/Nov/Current Academic Year |              |
#
#	When the ILR file is re-submitted for the learners for collection period <Collection_Period>
#	
#	Then the following learner earnings should be generated
#		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
#		| Aug/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
#		| Sep/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
#		| Oct/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
#		| Nov/Current Academic Year | 500          | 0          | 0         | pe-2                     |
#		| Dec/Current Academic Year | 500          | 0          | 0         | pe-2                     |
#		| Jan/Current Academic Year | 500          | 0          | 0         | pe-2                     |
#		| Feb/Current Academic Year | 500          | 0          | 0         | pe-2                     |
#		| Mar/Current Academic Year | 500          | 0          | 0         | pe-2                     |
#		| Apr/Current Academic Year | 500          | 0          | 0         | pe-2                     |
#		| May/Current Academic Year | 500          | 0          | 0         | pe-2                     |
#		| Jun/Current Academic Year | 500          | 0          | 0         | pe-2                     |
#		| Jul/Current Academic Year | 500          | 0          | 0         | pe-2                     |
#
#	And ensure that all the price events submitted in the ILR are published to the Apprenticeship Service
#		| AimSeqNumber | PriceEpisodeIdentifier | IsPayable | CommitmentId     | IlrStartDate                 | IlrStandardCode | IlrProgrammeType | IlrFrameworkCode | IlrPathwayCode | IlrTrainingPrice | IlrEndpointAssesorPrice | IlrPriceEffectiveFromDate    | IlrPriceEffectiveToDate | Status | 
#		| 1            | pe-2                   | 1         | Apprenticeship a | 01/Aug/Current Academic Year | 99              | 25               | 0                | 0              | 9375             | 0                       | 01/Nov/Current Academic Year |                         | 1      | 
#
#	And ensure no DataLock Event Errors are published to the Apprenticeship Service
#	
#	
	Examples:
		| Collection_Period         | Levy Balance |
		| R04/Current Academic Year | 50000        |