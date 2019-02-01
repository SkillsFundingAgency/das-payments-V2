Feature: Two Non Levy-Learners One Finishes Late And Other Finishes Early PV2-199

@EndToEnd

Scenario Outline: Two non-LEVY learners, one learner finishes early, one finishes late PV2-199
	Given the provider previously submitted the following learner details
        | Learner ID | Priority | Start Date             | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     |
        | learner a  | 1        | Sep/Last Academic Year | 15 months        | 18750                | 1st day of Sep/Last Academic Year   | 0                      | 1st day of Sep/Last Academic Year     |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) |
        | learner b  | 1        | Sep/Last Academic Year | 12 months        | 15000                | 1st day of Sep/Last Academic Year   | 0                      | 1st day of Sep/Last Academic Year     |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) |
	And the following earnings had been generated for the learner
        | Learner ID | Delivery Period        | On-Programme | Completion | Balancing | SFA Contribution Percentage |
        | learner a  | Aug/Last Academic Year | 0            | 0          | 0         | 90%                         |
        | learner a  | Sep/Last Academic Year | 1000         | 0          | 0         | 90%                         |
        | learner a  | Oct/Last Academic Year | 1000         | 0          | 0         | 90%                         |
        | learner a  | Nov/Last Academic Year | 1000         | 0          | 0         | 90%                         |
        | learner a  | Dec/Last Academic Year | 1000         | 0          | 0         | 90%                         |
        | learner a  | Jan/Last Academic Year | 1000         | 0          | 0         | 90%                         |
        | learner a  | Feb/Last Academic Year | 1000         | 0          | 0         | 90%                         |
        | learner a  | Mar/Last Academic Year | 1000         | 0          | 0         | 90%                         |
        | learner a  | Apr/Last Academic Year | 1000         | 0          | 0         | 90%                         |
        | learner a  | May/Last Academic Year | 1000         | 0          | 0         | 90%                         |
        | learner a  | Jun/Last Academic Year | 1000         | 0          | 0         | 90%                         |
        | learner a  | Jul/Last Academic Year | 1000         | 0          | 0         | 90%                         |
        | learner b  | Aug/Last Academic Year | 0            | 0          | 0         | 90%                         |
        | learner b  | Sep/Last Academic Year | 1000         | 0          | 0         | 90%                         |
        | learner b  | Oct/Last Academic Year | 1000         | 0          | 0         | 90%                         |
        | learner b  | Nov/Last Academic Year | 1000         | 0          | 0         | 90%                         |
        | learner b  | Dec/Last Academic Year | 1000         | 0          | 0         | 90%                         |
        | learner b  | Jan/Last Academic Year | 1000         | 0          | 0         | 90%                         |
        | learner b  | Feb/Last Academic Year | 1000         | 0          | 0         | 90%                         |
        | learner b  | Mar/Last Academic Year | 1000         | 0          | 0         | 90%                         |
        | learner b  | Apr/Last Academic Year | 1000         | 0          | 0         | 90%                         |
        | learner b  | May/Last Academic Year | 1000         | 0          | 0         | 90%                         |
        | learner b  | Jun/Last Academic Year | 1000         | 0          | 0         | 90%                         |
        | learner b  | Jul/Last Academic Year | 1000         | 0          | 0         | 90%                         |
    And the following provider payments had been generated
        | Learner ID | Collection Period      | Delivery Period        | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | learner a  | R02/Last Academic Year | Sep/Last Academic Year | 900                    | 100                         | Learning         |
        | learner a  | R03/Last Academic Year | Oct/Last Academic Year | 900                    | 100                         | Learning         |
        | learner a  | R04/Last Academic Year | Nov/Last Academic Year | 900                    | 100                         | Learning         |
        | learner a  | R05/Last Academic Year | Dec/Last Academic Year | 900                    | 100                         | Learning         |
        | learner a  | R06/Last Academic Year | Jan/Last Academic Year | 900                    | 100                         | Learning         |
        | learner a  | R07/Last Academic Year | Feb/Last Academic Year | 900                    | 100                         | Learning         |
        | learner a  | R08/Last Academic Year | Mar/Last Academic Year | 900                    | 100                         | Learning         |
        | learner a  | R09/Last Academic Year | Apr/Last Academic Year | 900                    | 100                         | Learning         |
        | learner a  | R10/Last Academic Year | May/Last Academic Year | 900                    | 100                         | Learning         |
        | learner a  | R11/Last Academic Year | Jun/Last Academic Year | 900                    | 100                         | Learning         |
        | learner a  | R12/Last Academic Year | Jul/Last Academic Year | 900                    | 100                         | Learning         |
        | learner b  | R02/Last Academic Year | Sep/Last Academic Year | 900                    | 100                         | Learning         |
        | learner b  | R03/Last Academic Year | Oct/Last Academic Year | 900                    | 100                         | Learning         |
        | learner b  | R04/Last Academic Year | Nov/Last Academic Year | 900                    | 100                         | Learning         |
        | learner b  | R05/Last Academic Year | Dec/Last Academic Year | 900                    | 100                         | Learning         |
        | learner b  | R06/Last Academic Year | Jan/Last Academic Year | 900                    | 100                         | Learning         |
        | learner b  | R07/Last Academic Year | Feb/Last Academic Year | 900                    | 100                         | Learning         |
        | learner b  | R08/Last Academic Year | Mar/Last Academic Year | 900                    | 100                         | Learning         |
        | learner b  | R09/Last Academic Year | Apr/Last Academic Year | 900                    | 100                         | Learning         |
        | learner b  | R10/Last Academic Year | May/Last Academic Year | 900                    | 100                         | Learning         |
        | learner b  | R11/Last Academic Year | Jun/Last Academic Year | 900                    | 100                         | Learning         |
        | learner b  | R12/Last Academic Year | Jul/Last Academic Year | 900                    | 100                         | Learning         |
    But the Provider now changes the Learner details as follows
        | Learner ID | Priority | Start Date             | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     |
        | learner a  | 1        | Sep/Last Academic Year | 15 months        | 18750                | 1st day of Sep/Last Academic Year   | 0                      | 1st day of Sep/Last Academic Year     | 12 months       | completed         | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) |
        | learner b  | 1        | Sep/Last Academic Year | 12 months        | 15000                | 1st day of Sep/Last Academic Year   | 0                      | 1st day of Sep/Last Academic Year     | 15 months       | completed         | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
    Then the following learner earnings should be generated
		| Learner ID | Delivery Period           | On-Programme | Completion | Balancing | SFA Contribution Percentage |
		| learner a  | Aug/Current Academic Year | 1000         | 0          | 0         | 90%                         |
		| learner a  | Sep/Current Academic Year | 0            | 3750       | 3000      | 90%                         |
		| learner a  | Oct/Current Academic Year | 0            | 0          | 0         | 90%                         |
		| learner a  | Nov/Current Academic Year | 0            | 0          | 0         | 90%                         |
		| learner a  | Dec/Current Academic Year | 0            | 0          | 0         | 90%                         |
		| learner a  | Jan/Current Academic Year | 0            | 0          | 0         | 90%                         |
		| learner a  | Feb/Current Academic Year | 0            | 0          | 0         | 90%                         |
		| learner a  | Mar/Current Academic Year | 0            | 0          | 0         | 90%                         |
		| learner a  | Apr/Current Academic Year | 0            | 0          | 0         | 90%                         |
		| learner a  | May/Current Academic Year | 0            | 0          | 0         | 90%                         |
		| learner a  | Jun/Current Academic Year | 0            | 0          | 0         | 90%                         |
		| learner a  | Jul/Current Academic Year | 0            | 0          | 0         | 90%                         |
		| learner b  | Aug/Current Academic Year | 1000         | 0          | 0         | 90%                         |
		| learner b  | Sep/Current Academic Year | 0            | 0          | 0         | 90%                         |
		| learner b  | Oct/Current Academic Year | 0            | 0          | 0         | 90%                         |
		| learner b  | Nov/Current Academic Year | 0            | 0          | 0         | 90%                         |
		| learner b  | Dec/Current Academic Year | 0            | 3000       | 0         | 90%                         |
		| learner b  | Jan/Current Academic Year | 0            | 0          | 0         | 90%                         |
		| learner b  | Feb/Current Academic Year | 0            | 0          | 0         | 90%                         |
		| learner b  | Mar/Current Academic Year | 0            | 0          | 0         | 90%                         |
		| learner b  | Apr/Current Academic Year | 0            | 0          | 0         | 90%                         |
		| learner b  | May/Current Academic Year | 0            | 0          | 0         | 90%                         |
		| learner b  | Jun/Current Academic Year | 0            | 0          | 0         | 90%                         |
		| learner b  | Jul/Current Academic Year | 0            | 0          | 0         | 90%                         |
    And only the following payments will be calculated
		| Learner ID | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
		| learner a  | R01/Current Academic Year | Aug/Current Academic Year | 1000         | 0          | 0         |
		| learner a  | R02/Current Academic Year | Sep/Current Academic Year | 0            | 3750       | 3000      |
		| learner b  | R01/Current Academic Year | Aug/Current Academic Year | 1000         | 0          | 0         |
		| learner b  | R05/Current Academic Year | Dec/Current Academic Year | 0            | 3000       | 0         |
    And only the following provider payments will be recorded
		| Learner ID | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| learner a  | R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | Learning         |
		| learner a  | R02/Current Academic Year | Sep/Current Academic Year | 3375                   | 375                         | Completion       |
		| learner a  | R02/Current Academic Year | Sep/Current Academic Year | 2700                   | 300                         | Balancing        |
		| learner b  | R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | Learning         |
		| learner b  | R05/Current Academic Year | Dec/Current Academic Year | 2700                   | 300                         | Completion       |
	And at month end only the following provider payments will be generated
        | Learner ID | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | learner a  | R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | Learning         |
        | learner a  | R02/Current Academic Year | Sep/Current Academic Year | 3375                   | 375                         | Completion       |
        | learner a  | R02/Current Academic Year | Sep/Current Academic Year | 2700                   | 300                         | Balancing        |
        | learner b  | R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | Learning         |
        | learner b  | R05/Current Academic Year | Dec/Current Academic Year | 2700                   | 300                         | Completion       |
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