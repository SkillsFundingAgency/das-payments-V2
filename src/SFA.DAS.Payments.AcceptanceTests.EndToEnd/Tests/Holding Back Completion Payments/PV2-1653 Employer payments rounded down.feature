Feature: Holding back completion payments - PV2-1653
	As a provider,
	I want a levy learner with co-funding, where the employer has paid their 10% co-investment for the on-program element only, but has not yet paid the employer completion payment element
	So that I am accurately paid the completion payment by SFA

Scenario Outline: Levy Learner-in co-funding completion payment made as enough employer contribution PV2-1653
	
	Given the employer levy account balance in collection period <Collection_Period> is 0
	
	And the following commitments exist
        | start date                | end date                     | agreed price | status |   Framework Code | Pathway Code | Programme Type |
        | 01/Jun/Last Academic Year | 01/Jun/Current Academic Year | 9000         | active |   593            | 1            | 20             |
	
	And the provider previously submitted the following learner details
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 01/Jun/Last Academic Year | 12 months        | 9000                 | 06/Jun/Last Academic Year           | 0                      | 06/Jun/Last Academic Year             |                 | continuing        | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
	
	And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing |
        | Aug/Last Academic Year | 0            | 0          | 0         |
        | Sep/Last Academic Year | 0            | 0          | 0         |
        | Oct/Last Academic Year | 0            | 0          | 0         |
        | Nov/Last Academic Year | 0            | 0          | 0         |
        | Dec/Last Academic Year | 0            | 0          | 0         |
        | Jan/Last Academic Year | 0            | 0          | 0         |
        | Feb/Last Academic Year | 0            | 0          | 0         |
        | Mar/Last Academic Year | 0            | 0          | 0         |
        | Apr/Last Academic Year | 0            | 0          | 0         |
        | May/Last Academic Year | 0            | 0          | 0         |
        | Jun/Last Academic Year | 600          | 0          | 0         |
        | Jul/Last Academic Year | 600          | 0          | 0         |
    
	And the following provider payments had been generated 
        | Collection Period      | Delivery Period        | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
        | R11/Last Academic Year | Jun/Last Academic Year | 540                    | 60.55                       | 0             | Learning         |
        | R12/Last Academic Year | Jul/Last Academic Year | 540                    | 60.55                       | 0             | Learning         |	
	# New field - Employer Contribution
    
	But the Provider now changes the Learner details as follows
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage | Employer Contribution |
		| 01/Jun/Last Academic Year | 12 months        | 9000                 | 01/Jun/Last Academic Year           | 0                      |                                       | 12 months       | completed         | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         | 720                   |
	
	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
		| pe-1             | 9000                 | 01/Jun/Last Academic Year           | 0                      | 01/Jun/Last Academic Year             | 0                       |                                        | 0                         |                                          | Act1          | 1                   | 90%                         |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| Aug/Current Academic Year | 666          | 0          | 0         | pe-1                     |
		| Sep/Current Academic Year | 666          | 0          | 0         | pe-1                     |
		| Oct/Current Academic Year | 666          | 0          | 0         | pe-1                     |
		| Nov/Current Academic Year | 666          | 0          | 0         | pe-1                     |
		| Dec/Current Academic Year | 666          | 0          | 0         | pe-1                     |
		| Jan/Current Academic Year | 666          | 0          | 0         | pe-1                     |
		| Feb/Current Academic Year | 666          | 0          | 0         | pe-1                     |
		| Mar/Current Academic Year | 666          | 0          | 0         | pe-1                     |
		| Apr/Current Academic Year | 666          | 0          | 0         | pe-1                     |
		| May/Current Academic Year | 666          | 0          | 0         | pe-1                     |
		| Jun/Current Academic Year | 0            | 1800       | 0         | pe-1                     |
		| Jul/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    
	And at month end only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
        | R01/Current Academic Year | Aug/Current Academic Year | 666          | 0          | 0         |
        | R02/Current Academic Year | Sep/Current Academic Year | 666          | 0          | 0         |
        | R03/Current Academic Year | Oct/Current Academic Year | 666          | 0          | 0         |
        | R04/Current Academic Year | Nov/Current Academic Year | 666          | 0          | 0         |
        | R05/Current Academic Year | Dec/Current Academic Year | 666          | 0          | 0         |
        | R06/Current Academic Year | Jan/Current Academic Year | 666          | 0          | 0         |
        | R07/Current Academic Year | Feb/Current Academic Year | 666          | 0          | 0         |
        | R08/Current Academic Year | Mar/Current Academic Year | 666          | 0          | 0         |
        | R09/Current Academic Year | Apr/Current Academic Year | 666          | 0          | 0         |
        | R10/Current Academic Year | May/Current Academic Year | 666          | 0          | 0         |
        | R11/Current Academic Year | Jun/Current Academic Year | 0            | 1800       | 0         |
	
	And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 599.40                 | 66.60                       | 0             | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 599.40                 | 66.60                       | 0             | Learning         |
        | R03/Current Academic Year | Oct/Current Academic Year | 599.40                 | 66.60                       | 0             | Learning         |
        | R04/Current Academic Year | Nov/Current Academic Year | 599.40                 | 66.60                       | 0             | Learning         |
        | R05/Current Academic Year | Dec/Current Academic Year | 599.40                 | 66.60                       | 0             | Learning         |
        | R06/Current Academic Year | Jan/Current Academic Year | 599.40                 | 66.60                       | 0             | Learning         |
        | R07/Current Academic Year | Feb/Current Academic Year | 599.40                 | 66.60                       | 0             | Learning         |
        | R08/Current Academic Year | Mar/Current Academic Year | 599.40                 | 66.60                       | 0             | Learning         |
        | R09/Current Academic Year | Apr/Current Academic Year | 599.40                 | 66.60                       | 0             | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 599.40                 | 66.60                       | 0             | Learning         |
        | R11/Current Academic Year | Jun/Current Academic Year | 1620                   | 180                         | 0             | Completion       |
	
	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 599.40                 | 66.60                       | 0             | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 599.40                 | 66.60                       | 0             | Learning         |
        | R03/Current Academic Year | Oct/Current Academic Year | 599.40                 | 66.60                       | 0             | Learning         |
        | R04/Current Academic Year | Nov/Current Academic Year | 599.40                 | 66.60                       | 0             | Learning         |
        | R05/Current Academic Year | Dec/Current Academic Year | 599.40                 | 66.60                       | 0             | Learning         |
        | R06/Current Academic Year | Jan/Current Academic Year | 599.40                 | 66.60                       | 0             | Learning         |
        | R07/Current Academic Year | Feb/Current Academic Year | 599.40                 | 66.60                       | 0             | Learning         |
        | R08/Current Academic Year | Mar/Current Academic Year | 599.40                 | 66.60                       | 0             | Learning         |
        | R09/Current Academic Year | Apr/Current Academic Year | 599.40                 | 66.60                       | 0             | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 599.40                 | 66.60                       | 0             | Learning         |
        | R11/Current Academic Year | Jun/Current Academic Year | 1620                   | 180                         | 0             | Completion       |

Examples: 
        | Collection_Period         |
        | R01/Current Academic Year |
        | R02/Current Academic Year |
        | R03/Current Academic Year |
        | R04/Current Academic Year |
        | R05/Current Academic Year |
        | R06/Current Academic Year |
        | R07/Current Academic Year |
        | R08/Current Academic Year |
        | R09/Current Academic Year |
        | R10/Current Academic Year |
        | R11/Current Academic Year |



