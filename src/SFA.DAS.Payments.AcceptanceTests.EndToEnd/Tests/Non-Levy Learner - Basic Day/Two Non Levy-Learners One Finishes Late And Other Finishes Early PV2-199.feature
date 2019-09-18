﻿@basic_day
@supports_dc_e2e
Feature: Two Non Levy-Learners One Finishes Late And Other Finishes Early PV2-199

Scenario Outline: Two non-LEVY learners, one learner finishes early, one finishes late PV2-199
	Given the provider previously submitted the following learner details
        | Learner ID | Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                   | SFA Contribution Percentage |
        | learner a  | 01/Sep/Last Academic Year | 15 months        | 9375                 | 01/Sep/Last Academic Year           |                        |                                       |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%						   |
        | learner b  | 08/Sep/Last Academic Year | 12 months        | 15000                | 08/Sep/Last Academic Year           |                        |                                       |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%						   |
	And the following earnings had been generated for the learner
        | Learner ID | Delivery Period        | On-Programme | Completion | Balancing | SFA Contribution Percentage |
        | learner a  | Aug/Last Academic Year | 0            | 0          | 0         | 90%                         |
        | learner a  | Sep/Last Academic Year | 500          | 0          | 0         | 90%                         |
        | learner a  | Oct/Last Academic Year | 500          | 0          | 0         | 90%                         |
        | learner a  | Nov/Last Academic Year | 500          | 0          | 0         | 90%                         |
        | learner a  | Dec/Last Academic Year | 500          | 0          | 0         | 90%                         |
        | learner a  | Jan/Last Academic Year | 500          | 0          | 0         | 90%                         |
        | learner a  | Feb/Last Academic Year | 500          | 0          | 0         | 90%                         |
        | learner a  | Mar/Last Academic Year | 500          | 0          | 0         | 90%                         |
        | learner a  | Apr/Last Academic Year | 500          | 0          | 0         | 90%                         |
        | learner a  | May/Last Academic Year | 500          | 0          | 0         | 90%                         |
        | learner a  | Jun/Last Academic Year | 500          | 0          | 0         | 90%                         |
        | learner a  | Jul/Last Academic Year | 500          | 0          | 0         | 90%                         |
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
        | learner a  | R02/Last Academic Year | Sep/Last Academic Year | 450                    | 50                          | Learning         |
        | learner a  | R03/Last Academic Year | Oct/Last Academic Year | 450                    | 50                          | Learning         |
        | learner a  | R04/Last Academic Year | Nov/Last Academic Year | 450                    | 50                          | Learning         |
        | learner a  | R05/Last Academic Year | Dec/Last Academic Year | 450                    | 50                          | Learning         |
        | learner a  | R06/Last Academic Year | Jan/Last Academic Year | 450                    | 50                          | Learning         |
        | learner a  | R07/Last Academic Year | Feb/Last Academic Year | 450                    | 50                          | Learning         |
        | learner a  | R08/Last Academic Year | Mar/Last Academic Year | 450                    | 50                          | Learning         |
        | learner a  | R09/Last Academic Year | Apr/Last Academic Year | 450                    | 50                          | Learning         |
        | learner a  | R10/Last Academic Year | May/Last Academic Year | 450                    | 50                          | Learning         |
        | learner a  | R11/Last Academic Year | Jun/Last Academic Year | 450                    | 50                          | Learning         |
        | learner a  | R12/Last Academic Year | Jul/Last Academic Year | 450                    | 50                          | Learning         |
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
        | Learner ID | Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                   | SFA Contribution Percentage |
        | learner a  | 01/Sep/Last Academic Year | 15 months        | 9375                 | 01/Sep/Last Academic Year           |                        |                                       | 12 months       | completed         | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%						   |
        | learner b  | 08/Sep/Last Academic Year | 12 months        | 15000                | 08/Sep/Last Academic Year           |                        |                                       | 15 months       | completed         | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%						   |
	 And price details as follows
		| Learner ID | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Contract Type | Aim Sequence Number |
		| learner a  | pe-1             | 9375                 | 01/Sep/Last Academic Year           |                        |                                       | 0                       |                                        | 0                         |                                          | 90%                         | Act2          | 1                   |
		| learner b  | pe-2             | 15000                | 08/Sep/Last Academic Year           |                        |                                       | 0                       |                                        | 0                         |                                          | 90%                         | Act2          | 1                   |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
    Then the following learner earnings should be generated
		| Learner ID | Delivery Period           | On-Programme | Completion | Balancing | SFA Contribution Percentage | Price Episode Identifier |
		| learner a  | Aug/Current Academic Year | 500          | 0          | 0         | 90%                         | pe-1                     |
		| learner a  | Sep/Current Academic Year | 0            | 1875       | 1500      | 90%                         | pe-1                     |
		| learner a  | Oct/Current Academic Year | 0            | 0          | 0         | 90%                         | pe-1                     |
		| learner a  | Nov/Current Academic Year | 0            | 0          | 0         | 90%                         | pe-1                     |
		| learner a  | Dec/Current Academic Year | 0            | 0          | 0         | 90%                         | pe-1                     |
		| learner a  | Jan/Current Academic Year | 0            | 0          | 0         | 90%                         | pe-1                     |
		| learner a  | Feb/Current Academic Year | 0            | 0          | 0         | 90%                         | pe-1                     |
		| learner a  | Mar/Current Academic Year | 0            | 0          | 0         | 90%                         | pe-1                     |
		| learner a  | Apr/Current Academic Year | 0            | 0          | 0         | 90%                         | pe-1                     |
		| learner a  | May/Current Academic Year | 0            | 0          | 0         | 90%                         | pe-1                     |
		| learner a  | Jun/Current Academic Year | 0            | 0          | 0         | 90%                         | pe-1                     |
		| learner a  | Jul/Current Academic Year | 0            | 0          | 0         | 90%                         | pe-1                     |
		| learner b  | Aug/Current Academic Year | 1000         | 0          | 0         | 90%                         | pe-2                     |
		| learner b  | Sep/Current Academic Year | 0            | 0          | 0         | 90%                         | pe-2                     |
		| learner b  | Oct/Current Academic Year | 0            | 0          | 0         | 90%                         | pe-2                     |
		| learner b  | Nov/Current Academic Year | 0            | 0          | 0         | 90%                         | pe-2                     |
		| learner b  | Dec/Current Academic Year | 0            | 3000       | 0         | 90%                         | pe-2                     |
		| learner b  | Jan/Current Academic Year | 0            | 0          | 0         | 90%                         | pe-2                     |
		| learner b  | Feb/Current Academic Year | 0            | 0          | 0         | 90%                         | pe-2                     |
		| learner b  | Mar/Current Academic Year | 0            | 0          | 0         | 90%                         | pe-2                     |
		| learner b  | Apr/Current Academic Year | 0            | 0          | 0         | 90%                         | pe-2                     |
		| learner b  | May/Current Academic Year | 0            | 0          | 0         | 90%                         | pe-2                     |
		| learner b  | Jun/Current Academic Year | 0            | 0          | 0         | 90%                         | pe-2                     |
		| learner b  | Jul/Current Academic Year | 0            | 0          | 0         | 90%                         | pe-2                     |
    And only the following payments will be calculated
		| Learner ID | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
		| learner a  | R01/Current Academic Year | Aug/Current Academic Year | 500          | 0          | 0         |
		| learner a  | R02/Current Academic Year | Sep/Current Academic Year | 0            | 1875       | 1500      |
		| learner b  | R01/Current Academic Year | Aug/Current Academic Year | 1000         | 0          | 0         |
		| learner b  | R05/Current Academic Year | Dec/Current Academic Year | 0            | 3000       | 0         |
    And only the following provider payments will be recorded
		| Learner ID | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| learner a  | R01/Current Academic Year | Aug/Current Academic Year | 450                    | 50                          | Learning         |
		| learner a  | R02/Current Academic Year | Sep/Current Academic Year | 1687.50                | 187.50                      | Completion       |
		| learner a  | R02/Current Academic Year | Sep/Current Academic Year | 1350                   | 150                         | Balancing        |
		| learner b  | R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | Learning         |
		| learner b  | R05/Current Academic Year | Dec/Current Academic Year | 2700                   | 300                         | Completion       |
	And at month end only the following provider payments will be generated
        | Learner ID | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| learner a  | R01/Current Academic Year | Aug/Current Academic Year | 450                    | 50                          | Learning         |
		| learner a  | R02/Current Academic Year | Sep/Current Academic Year | 1687.50                | 187.50                      | Completion       |
		| learner a  | R02/Current Academic Year | Sep/Current Academic Year | 1350                   | 150                         | Balancing        |
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

    # Scenario: 2 non-DAS learners, one learner finishes early, one finishes late
    #    When an ILR file is submitted with the following data:
    #        | ULN       | learner type           | agreed price | start date | planned end date | actual end date | completion status |
    #        | learner a | programme only non-DAS | 18750        | 01/09/2018 | 08/12/2019       | 08/09/2019      | completed         |
    #        | learner b | programme only non-DAS | 15000        | 08/09/2018 | 08/09/2019       | 08/12/2019      | completed         |
    #    Then the provider earnings and payments break down for learner a as follows:
    #        | Type                           | 09/18 | 10/18 | 11/18 | ... | 08/19 | 09/19 | 10/19 |
    #        | Provider Earned Total          | 1000  | 1000  | 1000  | ... | 1000  | 6750  | 0     |
    #        | Provider Earned from SFA       | 900   | 900   | 900   | ... | 900   | 6075  | 0     |
    #        | Provider Earned from Employer  | 100   | 100   | 100   | ... | 100   | 675   | 0     |
    #        | Provider Paid by SFA           | 0     | 900   | 900   | ... | 900   | 900   | 6075  |
    #        | Payment due from Employer      | 0     | 100   | 100   | ... | 100   | 100   | 675   |
    #        | Levy account debited           | 0     | 0     | 0     | ... | 0     | 0     | 0     |
    #        | SFA Levy employer budget       | 0     | 0     | 0     | ... | 0     | 0     | 0     |
    #        | SFA Levy co-funding budget     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
    #        | SFA non-Levy co-funding budget | 900   | 900   | 900   | ... | 900   | 6075  | 0     |
    #    And the transaction types for the payments are:
    #        | Payment type             | 10/18 | 11/18 | 12/18 | 01/19 | ... | 09/19 | 10/19 |
    #        | On-program               | 900   | 900   | 900   | 900   | ... | 900   | 0     |
    #        | Completion               | 0     | 0     | 0     | 0     | ... | 0     | 3375  |
    #        | Balancing                | 0     | 0     | 0     | 0     | ... | 0     | 2700  |
    #        | Employer 16-18 incentive | 0     | 0     | 0     | 0     | ... | 0     | 0     |
    #        | Provider 16-18 incentive | 0     | 0     | 0     | 0     | ... | 0     | 0     |
    #
    #    And then the provider earnings and payments break down for learner b as follows:
    #        | Type                           | 09/18 | 10/18 | 11/18 | ... | 08/19 | 09/19 | 10/19 | 11/19 | 12/19 | 01/20 |
    #        | Provider Earned Total          | 1000  | 1000  | 1000  | ... | 1000  | 0     | 0     | 0     | 3000  | 0     |
    #        | Provider Earned from SFA       | 900   | 900   | 900   | ... | 900   | 0     | 0     | 0     | 2700  | 0     |
    #        | Provider Earned from Employer  | 100   | 100   | 100   | ... | 100   | 0     | 0     | 0     | 300   | 0     |
    #        | Provider Paid by SFA           | 0     | 900   | 900   | ... | 900   | 900   | 0     | 0     | 0     | 2700  |
    #        | Payment due from Employer      | 0     | 100   | 100   | ... | 100   | 100   | 0     | 0     | 0     | 300   |
    #        | Levy account debited           | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     | 0     |
    #        | SFA Levy employer budget       | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     | 0     |
    #        | SFA Levy co-funding budget     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     | 0     |
    #        | SFA non-Levy co-funding budget | 900   | 900   | 900   | ... | 900   | 0     | 0     | 0     | 2700  | 0     |
    #    And the transaction types for the payments are:
    #        | Payment type             | 10/18 | 11/18 | 12/18 | 01/19 | ... | 09/19 | 10/19 | 11/19 | 12/19 | 01/20 |
    #        | On-program               | 900   | 900   | 900   | 900   | ... | 900   | 0     | 0     | 0     | 0     |
    #        | Completion               | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 2700  |
    #        | Balancing                | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     |
    #        | Employer 16-18 incentive | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     |
    #        | Provider 16-18 incentive | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     |