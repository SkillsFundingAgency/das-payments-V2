﻿Feature:  Non-levy learner 16-18 employed with a large employer change to small employer PV2-352
	As a provider,
	I want a non-levy learner, 1 learner aged 16-18. Second employment status record added with same employer id but small employer flag removed. Learner retains small employer funding, to be paid the correct amount
	So that I am accurately paid my apprenticeship provision.
Scenario: Non-levy learner 16-18 employed with a large employer change to small employer PV2-352
 # Notes: Multiple employers
 # And the employment status in the ILR is
 #       | Employer   | Employment Status  | Employment Status Applies | Small Employer |
 #       | employer 1 | in paid employment | 05/Aug/Last Academic Year |                |
 #       | employer 2 | in paid employment | 05/Oct/Last Academic Year | SEM1           |

# SFA Contribution Percentage is moved to the earnings table

	Given the provider previously submitted the following learner details
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     |
		| 06/Aug/Last Academic Year | 12 months        | 7500                 | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             |                 | continuing        | Act2          | 1                   | ZPROG001      | 403            | 1            | 25             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) |
	# SFA Contribution Percentage is moved to the earnings table
    And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing | First16To18EmployerIncentive | First16To18ProviderIncentive | OnProgramme16To18FrameworkUplift | SFA Contribution Percentage |
        | Aug/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              | 90%                         |
        | Sep/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              | 90%                         |
        | Oct/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              | 100%                        |
        | Nov/Last Academic Year | 500          | 0          | 0         | 500                          | 500                          | 120                              | 100%                        |
        | Dec/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              | 100%                        |
        | Jan/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              | 100%                        |
        | Feb/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              | 100%                        |
        | Mar/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              | 100%                        |
        | Apr/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              | 100%                        |
        | May/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              | 100%                        |
        | Jun/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              | 100%                        |
        | Jul/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              | 100%                        |
    And the following provider payments had been generated
        | Collection Period      | Delivery Period        | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type                 |
	# Large employer initially
        | R01/Last Academic Year | Jul/Last Academic Year | 450                    | 50                          | 0                         | Learning                         |
        | R02/Last Academic Year | Aug/Last Academic Year | 450                    | 50                          | 0                         | Learning                         |
        | R01/Last Academic Year | Jul/Last Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R02/Last Academic Year | Aug/Last Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
	# Switched to small employer
        | R03/Last Academic Year | Sep/Last Academic Year | 500                    | 0                           | 0                         | Learning                         |
        | R04/Last Academic Year | Oct/Last Academic Year | 500                    | 0                           | 0                         | Learning                         |
        | R05/Last Academic Year | Nov/Last Academic Year | 500                    | 0                           | 0                         | Learning                         |
        | R06/Last Academic Year | Dec/Last Academic Year | 500                    | 0                           | 0                         | Learning                         |
        | R07/Last Academic Year | Jan/Last Academic Year | 500                    | 0                           | 0                         | Learning                         |
        | R08/Last Academic Year | Feb/Last Academic Year | 500                    | 0                           | 0                         | Learning                         |
        | R09/Last Academic Year | Mar/Last Academic Year | 500                    | 0                           | 0                         | Learning                         |
        | R10/Last Academic Year | Apr/Last Academic Year | 500                    | 0                           | 0                         | Learning                         |
        | R11/Last Academic Year | May/Last Academic Year | 500                    | 0                           | 0                         | Learning                         |
        | R12/Last Academic Year | Jun/Last Academic Year | 500                    | 0                           | 0                         | Learning                         |
        | R03/Last Academic Year | Sep/Last Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R04/Last Academic Year | Oct/Last Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R05/Last Academic Year | Nov/Last Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R06/Last Academic Year | Dec/Last Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R07/Last Academic Year | Jan/Last Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R08/Last Academic Year | Feb/Last Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R09/Last Academic Year | Mar/Last Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R10/Last Academic Year | Apr/Last Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R11/Last Academic Year | May/Last Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R12/Last Academic Year | Jun/Last Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
	# Incentives
        | R04/Last Academic Year | Oct/Last Academic Year | 0                      | 0                           | 500                       | First16To18EmployerIncentive     |
        | R04/Last Academic Year | Oct/Last Academic Year | 0                      | 0                           | 500                       | First16To18ProviderIncentive     |
    But the Provider now changes the Learner details as follows
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 06/Aug/Last Academic Year | 12 months        | 7500                 | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | 12 months       | completed         | Act2          | 1                   | ZPROG001      | 403            | 1            | 25             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 100%                        |
	When the amended ILR file is re-submitted for the learners in collection period R01/Current Academic Year
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Second16To18EmployerIncentive | Second16To18ProviderIncentive | Completion16To18FrameworkUplift |
		| Aug/Current Academic Year | 0            | 1500       | 0         | 500                           | 500                           | 360                             |
		| Sep/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               |
		| Oct/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               |
		| Nov/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               |
		| Dec/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               |
		| Jan/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               |
		| Feb/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               |
		| Mar/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               |
		| Apr/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               |
		| May/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               |
		| Jun/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               |
		| Jul/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               |
    And only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | Second16To18EmployerIncentive | Second16To18ProviderIncentive | Completion16To18FrameworkUplift |
		| R01/Current Academic Year | Aug/Current Academic Year | 0            | 1500       | 0         | 500                           | 500                           | 360                             |
	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type                |
		| R01/Current Academic Year | Aug/Current Academic Year | 1500                   | 0                           | 0                         | Completion                      |
		| R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 500                       | Second16To18EmployerIncentive   |
		| R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 500                       | Second16To18ProviderIncentive   |
		| R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 360                       | Completion16To18FrameworkUplift |
	And at month end only the following provider payments will be generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type                |
		| R01/Current Academic Year | Aug/Current Academic Year | 1500                   | 0                           | 0                         | Completion                      |
		| R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 500                       | Second16To18EmployerIncentive   |
		| R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 500                       | Second16To18ProviderIncentive   |
		| R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 360                       | Completion16To18FrameworkUplift |