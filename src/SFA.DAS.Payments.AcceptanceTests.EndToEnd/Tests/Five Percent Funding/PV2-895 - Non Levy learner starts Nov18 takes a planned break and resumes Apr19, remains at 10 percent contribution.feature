@ignore
Feature: 5% Contribution from April 2019 PV2-895
As a provider,
I want a Non Levy learner, starting prior to Apr 2019, taking a planned break and resuming from Apr 2019, where completion payment remains at 10% contribution
So that I am paid the correct apprenticeship funding by SFA

Scenario Outline: Non Levy Learner, started learning before Apr19, has planned break and resumes learning from Apr19, remains on 10% contribution PV2 -895
    Given the provider previously submitted the following learner details
        | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
        | 01/Nov/Current Academic Year | 12 months        | 12000                | 01/Nov/Current Academic Year        | 3000                   | 01/Nov/Current Academic Year          | 4 months        | planned break     | Act2          | 1                   | ZPROG001      | 17            | 25             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |

    And the following earnings had been generated for the learner
        | Delivery Period           | On-Programme | Completion | Balancing |
        | Aug/Current Academic Year | 0            | 0          | 0         |
        | Sep/Current Academic Year | 0            | 0          | 0         |
        | Oct/Current Academic Year | 0            | 0          | 0         |
        | Nov/Current Academic Year | 1000         | 0          | 0         |
        | Dec/Current Academic Year | 1000         | 0          | 0         |
        | Jan/Current Academic Year | 1000         | 0          | 0         |
        | Feb/Current Academic Year | 1000         | 0          | 0         |
        | Mar/Current Academic Year | 0            | 0          | 0         |
        | Apr/Current Academic Year | 0            | 0          | 0         |
        | May/Current Academic Year | 0            | 0          | 0         |
        | Jun/Current Academic Year | 0            | 0          | 0         |
        | Jul/Current Academic Year | 0            | 0          | 0         |
    And the following provider payments had been generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | R04/Current Academic Year | Nov/Current Academic Year | 900                    | 100                         | Learning         |
        | R05/Current Academic Year | Dec/Current Academic Year | 900                    | 100                         | Learning         |
		| R06/Current Academic Year | Jan/Current Academic Year | 900                    | 100                         | Learning         |
		| R07/Current Academic Year | Feb/Current Academic Year | 900                    | 100                         | Learning         |

    But the Provider now changes the Learner details as follows
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 03/Apr/Current Academic Year | 10 months        | 15000                | 03/Apr/Current Academic Year        | 3000                   | 03/Apr/Current Academic Year          |                 | continuing        | Act2          | 1                   | ZPROG001      | 17            | 25             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |
	And price details as follows
        | Price details     | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | SFA Contribution Percentage |
        | 1st price details | 12000                | 01/Nov/Current Academic Year        | 3000                   | 01/Nov/Current Academic Year          | 90%                         |
        | 2nd price details | 12000                | 03/Apr/Current Academic Year        | 3000                   | 03/Apr/Current Academic Year          | 90%                         |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing |
        | Aug/Current Academic Year | 0            | 0          | 0         |
        | Sep/Current Academic Year | 0            | 0          | 0         |
        | Oct/Current Academic Year | 0            | 0          | 0         |
        | Nov/Current Academic Year | 1000         | 0          | 0         |
        | Dec/Current Academic Year | 1000         | 0          | 0         |
        | Jan/Current Academic Year | 1000         | 0          | 0         |
        | Feb/Current Academic Year | 1000         | 0          | 0         |
        | Mar/Current Academic Year | 0            | 0          | 0         |
        | Apr/Current Academic Year | 1000         | 0          | 0         |
        | May/Current Academic Year | 1000         | 0          | 0         |
        | Jun/Current Academic Year | 1000         | 0          | 0         |
        | Jul/Current Academic Year | 1000         | 0          | 0         |
    And at month end only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
        | R08/Current Academic Year | Mar/Current Academic Year | 0            | 0          | 0         |
        | R09/Current Academic Year | Apr/Current Academic Year | 1000         | 0          | 0         |
        | R10/Current Academic Year | May/Current Academic Year | 1000         | 0          | 0         |
        | R11/Current Academic Year | Jun/Current Academic Year | 1000         | 0          | 0         |
        | R12/Current Academic Year | Jul/Current Academic Year | 1000         | 0          | 0         |
	And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | R08/Current Academic Year | Mar/Current Academic Year | 0                      | 0                           | Learning         |
        | R09/Current Academic Year | Apr/Current Academic Year | 900                    | 100                         | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 900                    | 100                         | Learning         |
        | R11/Current Academic Year | Jun/Current Academic Year | 900                    | 100                         | Learning         |
        | R12/Current Academic Year | Jul/Current Academic Year | 900                    | 100                         | Learning         |
	And at month end only the following provider payments will be generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | R08/Current Academic Year | Mar/Current Academic Year | 0                      | 0                           | Learning         |
        | R09/Current Academic Year | Apr/Current Academic Year | 900                    | 100                         | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 900                    | 100                         | Learning         |
        | R11/Current Academic Year | Jun/Current Academic Year | 900                    | 100                         | Learning         |
        | R12/Current Academic Year | Jul/Current Academic Year | 900                    | 100                         | Learning         |
Examples: 
        | Collection_Period         |        
        | R08/Current Academic Year |
        | R09/Current Academic Year |
        | R10/Current Academic Year |
        | R11/Current Academic Year |
        | R12/Current Academic Year |