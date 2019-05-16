#@supports_dc_e2e
Feature: Non-levy learner 19-24 with Education Health Care (EHC) plan, in paid employment with a small employer at start - PV2-328
	As a provider,
	I want 1 non-levy learner aged 19-24, with an Education Health Care (EHC) plan, In paid employment with a small employer at start, is fully funded for on programme and completion payments, to be paid the correct amount
	So that I am accurately paid my apprenticeship provision.
Scenario: Non-levy learner 19-24 with Education Health Care (EHC) plan, in paid employment with a small employer at start
	Given the provider previously submitted the following learner details
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 06/Aug/Last Academic Year | 12 months        | 7500                 | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             |                 | continuing        | Act2          | 1                   | ZPROG001      | 403            | 1            | 2              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 100%                        |
		# 100% contribution for small employer, 19-24 learner due to EEF code
    And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing | First16To18EmployerIncentive | First16To18ProviderIncentive | OnProgramme16To18FrameworkUplift |
        | Aug/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              |
        | Sep/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              |
        | Oct/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              |
        | Nov/Last Academic Year | 500          | 0          | 0         | 500                          | 500                          | 120                              |
        | Dec/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              |
        | Jan/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              |
        | Feb/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              |
        | Mar/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              |
        | Apr/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              |
        | May/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              |
        | Jun/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              |
        | Jul/Last Academic Year | 500          | 0          | 0         | 0                            | 0                            | 120                              |
    And the following provider payments had been generated
        | Collection Period      | Delivery Period        | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type                 |
        | R01/Last Academic Year | Aug/Last Academic Year | 500                    | 0                           | 0                         | Learning                         |
        | R02/Last Academic Year | Sep/Last Academic Year | 500                    | 0                           | 0                         | Learning                         |
        | R03/Last Academic Year | Oct/Last Academic Year | 500                    | 0                           | 0                         | Learning                         |
        | R04/Last Academic Year | Nov/Last Academic Year | 500                    | 0                           | 0                         | Learning                         |
        | R05/Last Academic Year | Dec/Last Academic Year | 500                    | 0                           | 0                         | Learning                         |
        | R06/Last Academic Year | Jan/Last Academic Year | 500                    | 0                           | 0                         | Learning                         |
        | R07/Last Academic Year | Feb/Last Academic Year | 500                    | 0                           | 0                         | Learning                         |
        | R08/Last Academic Year | Mar/Last Academic Year | 500                    | 0                           | 0                         | Learning                         |
        | R09/Last Academic Year | Apr/Last Academic Year | 500                    | 0                           | 0                         | Learning                         |
        | R10/Last Academic Year | May/Last Academic Year | 500                    | 0                           | 0                         | Learning                         |
        | R11/Last Academic Year | Jun/Last Academic Year | 500                    | 0                           | 0                         | Learning                         |
        | R12/Last Academic Year | Jul/Last Academic Year | 500                    | 0                           | 0                         | Learning                         |
        | R01/Last Academic Year | Aug/Last Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R02/Last Academic Year | Sep/Last Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R03/Last Academic Year | Oct/Last Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R04/Last Academic Year | Nov/Last Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R05/Last Academic Year | Dec/Last Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R06/Last Academic Year | Jan/Last Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R07/Last Academic Year | Feb/Last Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R08/Last Academic Year | Mar/Last Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R09/Last Academic Year | Apr/Last Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R10/Last Academic Year | May/Last Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R11/Last Academic Year | Jun/Last Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R12/Last Academic Year | Jul/Last Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R04/Last Academic Year | Nov/Last Academic Year | 0                      | 0                           | 500                       | First16To18EmployerIncentive     |
        | R04/Last Academic Year | Nov/Last Academic Year | 0                      | 0                           | 500                       | First16To18ProviderIncentive     |
    But the Provider now changes the Learner details as follows
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 06/Aug/Last Academic Year | 12 months        | 7500                 | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | 12 months       | completed         | Act2          | 1                   | ZPROG001      | 403            | 1            | 2              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 100%                        |
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