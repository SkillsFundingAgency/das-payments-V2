@ignore
# similar to PV2-526 - need to specify provider previously submitted learner details and price details	
#@supports_dc_e2e

Feature: Change standard and price but not from start - PV2-205
	As a provider,
	I want a non-levy learner, changes standard with change to negotiated price, to be paid the correct amount
	So that I am accurately paid my apprenticeship provision.

Scenario Outline: Non-Levy learner changes standard with accompanying change to the negotiated price PV2-205	
	Given the following learners
        | Learner Reference Number | Uln |
        | na                       |     |
	And the following aims
		| Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Programme Type | Standard Code | Funding Line Type                               | Completion Status |
		| ZPROG001      | 03/Aug/Current Academic Year | 12 months        | 3 months        | 1                   | 25             | 52            | 19+ Apprenticeship Non-Levy Contract (procured) | withdrawn         |
		| ZPROG001      | 12/Nov/Current Academic Year | 12 months        |                 | 3                   | 25             | 53            | 19+ Apprenticeship Non-Levy Contract (procured) | continuing        |
	And price details as follows		
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 12000                | 03/Aug/Current Academic Year        | 3000                   | 03/Aug/Current Academic Year          | Act2          | 1                   | 90%                         |
        | pe-2             | 6000                 | 12/Nov/Current Academic Year        | 1500                   | 12/Nov/Current Academic Year          | Act2          | 3                   | 90%                         |

    When the ILR file is submitted for the learners for collection period <Collection_Period>
    Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing | Aim Sequence Number | Price Episode Identifier |
        #p1
        | Aug/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
        | Sep/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
        | Oct/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
        | Nov/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
        | Dec/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
        | Jan/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
        | Feb/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
        | Mar/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
        | Apr/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
        | May/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
        | Jun/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
        | Jul/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
        #p2
        | Aug/Current Academic Year | 0            | 0          | 0         | 3                   | pe-2                     |
        | Sep/Current Academic Year | 0            | 0          | 0         | 3                   | pe-2                     |
        | Oct/Current Academic Year | 0            | 0          | 0         | 3                   | pe-2                     |
        | Nov/Current Academic Year | 500          | 0          | 0         | 3                   | pe-2                     |
        | Dec/Current Academic Year | 500          | 0          | 0         | 3                   | pe-2                     |
        | Jan/Current Academic Year | 500          | 0          | 0         | 3                   | pe-2                     |
        | Feb/Current Academic Year | 500          | 0          | 0         | 3                   | pe-2                     |
        | Mar/Current Academic Year | 500          | 0          | 0         | 3                   | pe-2                     |
        | Apr/Current Academic Year | 500          | 0          | 0         | 3                   | pe-2                     |
        | May/Current Academic Year | 500          | 0          | 0         | 3                   | pe-2                     |
        | Jun/Current Academic Year | 500          | 0          | 0         | 3                   | pe-2                     |
        | Jul/Current Academic Year | 500          | 0          | 0         | 3                   | pe-2                     |
    And only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
		| R04/Current Academic Year | Aug/Current Academic Year | 1000         | 0          | 0         |
		| R04/Current Academic Year | Sep/Current Academic Year | 1000         | 0          | 0         |
		| R04/Current Academic Year | Oct/Current Academic Year | 1000         | 0          | 0         |
		| R04/Current Academic Year | Nov/Current Academic Year | 500          | 0          | 0         |
		| R05/Current Academic Year | Dec/Current Academic Year | 500          | 0          | 0         |
		| R06/Current Academic Year | Jan/Current Academic Year | 500          | 0          | 0         |
		| R07/Current Academic Year | Feb/Current Academic Year | 500          | 0          | 0         |
		| R08/Current Academic Year | Mar/Current Academic Year | 500          | 0          | 0         |
		| R09/Current Academic Year | Apr/Current Academic Year | 500          | 0          | 0         |
		| R10/Current Academic Year | May/Current Academic Year | 500          | 0          | 0         |
		| R11/Current Academic Year | Jun/Current Academic Year | 500          | 0          | 0         |
		| R12/Current Academic Year | Jul/Current Academic Year | 500          | 0          | 0         |
    And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type | Standard Code |
        | R04/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | Learning         | 52            |
        | R04/Current Academic Year | Sep/Current Academic Year | 900                    | 100                         | Learning         | 52            |
        | R04/Current Academic Year | Oct/Current Academic Year | 900                    | 100                         | Learning         | 52            |
        | R04/Current Academic Year | Nov/Current Academic Year | 450                    | 50                          | Learning         | 53            |
        | R05/Current Academic Year | Dec/Current Academic Year | 450                    | 50                          | Learning         | 53            |
        | R06/Current Academic Year | Jan/Current Academic Year | 450                    | 50                          | Learning         | 53            |
        | R07/Current Academic Year | Feb/Current Academic Year | 450                    | 50                          | Learning         | 53            |
        | R08/Current Academic Year | Mar/Current Academic Year | 450                    | 50                          | Learning         | 53            |
        | R09/Current Academic Year | Apr/Current Academic Year | 450                    | 50                          | Learning         | 53            |
        | R10/Current Academic Year | May/Current Academic Year | 450                    | 50                          | Learning         | 53            |
        | R11/Current Academic Year | Jun/Current Academic Year | 450                    | 50                          | Learning         | 53            |
        | R12/Current Academic Year | Jul/Current Academic Year | 450                    | 50                          | Learning         | 53            |
	And at month end only the following provider payments will be generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type | Standard Code |
		| R04/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | Learning         | 52            |
		| R04/Current Academic Year | Sep/Current Academic Year | 900                    | 100                         | Learning         | 52            |
		| R04/Current Academic Year | Oct/Current Academic Year | 900                    | 100                         | Learning         | 52            |
		| R04/Current Academic Year | Nov/Current Academic Year | 450                    | 50                          | Learning         | 53            |
		| R05/Current Academic Year | Dec/Current Academic Year | 450                    | 50                          | Learning         | 53            |
		| R06/Current Academic Year | Jan/Current Academic Year | 450                    | 50                          | Learning         | 53            |
		| R07/Current Academic Year | Feb/Current Academic Year | 450                    | 50                          | Learning         | 53            |
		| R08/Current Academic Year | Mar/Current Academic Year | 450                    | 50                          | Learning         | 53            |
		| R09/Current Academic Year | Apr/Current Academic Year | 450                    | 50                          | Learning         | 53            |
		| R10/Current Academic Year | May/Current Academic Year | 450                    | 50                          | Learning         | 53            |
		| R11/Current Academic Year | Jun/Current Academic Year | 450                    | 50                          | Learning         | 53            |
		| R12/Current Academic Year | Jul/Current Academic Year | 450                    | 50                          | Learning         | 53            |
	Examples:
		| Collection_Period         |
		| R04/Current Academic Year |
		| R05/Current Academic Year |
		| R06/Current Academic Year |
		| R07/Current Academic Year |
		| R08/Current Academic Year |
		| R09/Current Academic Year |
		| R10/Current Academic Year |
		| R11/Current Academic Year |
		| R12/Current Academic Year |
