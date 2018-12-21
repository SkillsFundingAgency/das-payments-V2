Feature: Provider earnings and payments where learner changes apprenticeship standard and negotiated price remains the same, (remaining with the same employer and provider)
	As a provider,
	I want a non-levy learner, changes apprenticeship standard and the negotiated price remains the same, to be paid the correct amount
	So that I am accurately paid my apprenticeship provision.
	
Scenario Outline: Non-levy learner changes standard with no change to negotiated price PV2-207
	# the provider changes the Standard Type to 52 effective from 03/Nov/Current Academic Year
    Given the following learners
		| Learner Reference Number | Uln      |
		| abc123                   | 12345678 |
	And the following aims
		| Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Programme Type | Standard Code | Funding Line Type             | Completion Status |
		| ZPROG001      | 03/Aug/Current Academic Year | 12 months        | 3 months        | 1                   | 25             | 51            | 16-18 Apprenticeship Non-Levy | withdrawn         |
		| ZPROG001      | 03/Nov/Current Academic Year | 9 months         |                 | 2                   | 25             | 52            | 16-18 Apprenticeship Non-Levy | continuing        |
	And price details as follows		
         | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
         | pe-1             | 12000                | 03/Aug/Current Academic Year        | 3000                   | 03/Aug/Current Academic Year          | Act2          | 1                   | 90%                         |
         | pe-2             | 12000                | 03/Nov/Current Academic Year        | 3000                   | 03/Nov/Current Academic Year          | Act2          | 2                   | 90%                         |

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
        | Aug/Current Academic Year | 0            | 0          | 0         | 2                   | pe-2                     |
        | Sep/Current Academic Year | 0            | 0          | 0         | 2                   | pe-2                     |
        | Oct/Current Academic Year | 0            | 0          | 0         | 2                   | pe-2                     |
        | Nov/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-2                     |
        | Dec/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-2                     |
        | Jan/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-2                     |
        | Feb/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-2                     |
        | Mar/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-2                     |
        | Apr/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-2                     |
        | May/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-2                     |
        | Jun/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-2                     |
        | Jul/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-2                     |

    And only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
		| R01/Current Academic Year | Aug/Current Academic Year | 1000         | 0          | 0         |
		| R02/Current Academic Year | Sep/Current Academic Year | 1000         | 0          | 0         |
		| R03/Current Academic Year | Oct/Current Academic Year | 1000         | 0          | 0         |
		| R04/Current Academic Year | Nov/Current Academic Year | 1000         | 0          | 0         |
		| R05/Current Academic Year | Dec/Current Academic Year | 1000         | 0          | 0         |
		| R06/Current Academic Year | Jan/Current Academic Year | 1000         | 0          | 0         |
		| R07/Current Academic Year | Feb/Current Academic Year | 1000         | 0          | 0         |
		| R08/Current Academic Year | Mar/Current Academic Year | 1000         | 0          | 0         |
		| R09/Current Academic Year | Apr/Current Academic Year | 1000         | 0          | 0         |
		| R10/Current Academic Year | May/Current Academic Year | 1000         | 0          | 0         |
		| R11/Current Academic Year | Jun/Current Academic Year | 1000         | 0          | 0         |
		| R12/Current Academic Year | Jul/Current Academic Year | 1000         | 0          | 0         |
    And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 900                    | 100                         | Learning         |
        | R03/Current Academic Year | Oct/Current Academic Year | 900                    | 100                         | Learning         |
        | R04/Current Academic Year | Nov/Current Academic Year | 900                    | 100                         | Learning         |
        | R05/Current Academic Year | Dec/Current Academic Year | 900                    | 100                         | Learning         |
        | R06/Current Academic Year | Jan/Current Academic Year | 900                    | 100                         | Learning         |
        | R07/Current Academic Year | Feb/Current Academic Year | 900                    | 100                         | Learning         |
        | R08/Current Academic Year | Mar/Current Academic Year | 900                    | 100                         | Learning         |
        | R09/Current Academic Year | Apr/Current Academic Year | 900                    | 100                         | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 900                    | 100                         | Learning         |
        | R11/Current Academic Year | Jun/Current Academic Year | 900                    | 100                         | Learning         |
        | R12/Current Academic Year | Jul/Current Academic Year | 900                    | 100                         | Learning         |
	And at month end only the following provider payments will be generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type | Standard Code |
		| R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | Learning         | 51            |
		| R02/Current Academic Year | Sep/Current Academic Year | 900                    | 100                         | Learning         | 51            |
		| R03/Current Academic Year | Oct/Current Academic Year | 900                    | 100                         | Learning         | 51            |
		| R04/Current Academic Year | Nov/Current Academic Year | 900                    | 100                         | Learning         | 52            |
		| R05/Current Academic Year | Dec/Current Academic Year | 900                    | 100                         | Learning         | 52            |
		| R06/Current Academic Year | Jan/Current Academic Year | 900                    | 100                         | Learning         | 52            |
		| R07/Current Academic Year | Feb/Current Academic Year | 900                    | 100                         | Learning         | 52            |
		| R08/Current Academic Year | Mar/Current Academic Year | 900                    | 100                         | Learning         | 52            |
		| R09/Current Academic Year | Apr/Current Academic Year | 900                    | 100                         | Learning         | 52            |
		| R10/Current Academic Year | May/Current Academic Year | 900                    | 100                         | Learning         | 52            |
		| R11/Current Academic Year | Jun/Current Academic Year | 900                    | 100                         | Learning         | 52            |
		| R12/Current Academic Year | Jul/Current Academic Year | 900                    | 100                         | Learning         | 52            |
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
		| R12/Current Academic Year |
