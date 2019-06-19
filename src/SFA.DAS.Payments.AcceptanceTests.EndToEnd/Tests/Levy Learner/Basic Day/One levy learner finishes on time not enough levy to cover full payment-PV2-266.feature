Feature:One levy learner finishes on time not enough levy to cover full payment-PV2-266

Scenario Outline: One levy learner, not enough levy available to cover full payment, finished on time PV2-266
	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>

	And the following commitments exist
        | start date                | end date                     | agreed price | Framework Code | Pathway Code | Programme Type |
        | 01/Sep/Last Academic Year | 08/Sep/Current Academic Year | 15000        | 593            | 1            | 20             |
	And the provider previously submitted the following learner details
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                | SFA Contribution Percentage |
		| 01/Sep/Last Academic Year | 12 months        | 15000                | 01/Sep/Last Academic Year           |                        |                                       |                 | continuing        | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Levy Contract | 90%                         |
    And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing |
        | Aug/Last Academic Year | 0            | 0          | 0         |
        | Sep/Last Academic Year | 1000         | 0          | 0         |
        | Oct/Last Academic Year | 1000         | 0          | 0         |
        | Nov/Last Academic Year | 1000         | 0          | 0         |
        | Dec/Last Academic Year | 1000         | 0          | 0         |
        | Jan/Last Academic Year | 1000         | 0          | 0         |
        | Feb/Last Academic Year | 1000         | 0          | 0         |
        | Mar/Last Academic Year | 1000         | 0          | 0         |
        | Apr/Last Academic Year | 1000         | 0          | 0         |
        | May/Last Academic Year | 1000         | 0          | 0         |
        | Jun/Last Academic Year | 1000         | 0          | 0         |
        | Jul/Last Academic Year | 1000         | 0          | 0         |
	
    And the following provider payments had been generated
        | Collection Period      | Delivery Period        | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
        | R02/Last Academic Year | Sep/Last Academic Year | 450                    | 50                          | 500           | Learning         |
        | R03/Last Academic Year | Oct/Last Academic Year | 450                    | 50                          | 500           | Learning         |
        | R04/Last Academic Year | Nov/Last Academic Year | 450                    | 50                          | 500           | Learning         |
        | R05/Last Academic Year | Dec/Last Academic Year | 450                    | 50                          | 500           | Learning         |
        | R06/Last Academic Year | Jan/Last Academic Year | 450                    | 50                          | 500           | Learning         |
        | R07/Last Academic Year | Feb/Last Academic Year | 450                    | 50                          | 500           | Learning         |
        | R08/Last Academic Year | Mar/Last Academic Year | 450                    | 50                          | 500           | Learning         |
        | R09/Last Academic Year | Apr/Last Academic Year | 450                    | 50                          | 500           | Learning         |
        | R10/Last Academic Year | May/Last Academic Year | 450                    | 50                          | 500           | Learning         |
        | R11/Last Academic Year | Jun/Last Academic Year | 450                    | 50                          | 500           | Learning         |
        | R12/Last Academic Year | Jul/Last Academic Year | 450                    | 50                          | 500           | Learning         |
    But the Provider now changes the Learner details as follows
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                | SFA Contribution Percentage |
		| 01/Sep/Last Academic Year | 12 months        | 15000                | 01/Sep/Last Academic Year           |                        |                                       | 12 months       | completed         | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Levy Contract | 90%                         |
	 And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
		| pe-1             | 15000                | 01/Sep/Last Academic Year           |                        |                                       | 0                       |                                        | 0                         |                                          | Act1          | 1                   | 90%                         |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| Aug/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Sep/Current Academic Year | 0            | 3000       | 0         | pe-1                     |
		| Oct/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Nov/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Dec/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Jan/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Feb/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Mar/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Apr/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| May/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Jun/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Jul/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    And at month end only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
        | R01/Current Academic Year | Aug/Current Academic Year | 1000         | 0          | 0         |
        | R02/Current Academic Year | Sep/Current Academic Year | 0            | 3000       | 0         |

	And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 450                    | 50                          | 500           | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 1350                   | 150                         | 1500          | Completion       |
	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 450                    | 50                          | 500           | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 1350                   | 150                         | 1500          | Completion       |
Examples: 
        | Collection_Period         | Levy Balance |
        | R01/Current Academic Year | 500          |
        | R02/Current Academic Year | 1500         |
        | R03/Current Academic Year | 0            |