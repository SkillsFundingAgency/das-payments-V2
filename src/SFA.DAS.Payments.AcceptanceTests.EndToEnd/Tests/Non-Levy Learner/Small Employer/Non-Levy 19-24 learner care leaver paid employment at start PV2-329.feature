@ignore
# issue with KeyNotFoundException - can't find academic year when populating learner price episodes
#@supports_dc_e2e
Feature:  Non-levy learner 19-24 is a care leaver employed with a small employer at start fully funded PV2-329
	As a provider,
	I want a non-levy learner, 1 learner aged 19-24 that is a care leaver, in paid employment at start, is fully funded for on programme and completion payments, to be paid the correct amount
	So that I am accurately paid my apprenticeship provision.
Scenario: Non-levy learner 19-24 is a care leaver employed with a small employer at start fully funded PV2-329
#Note: care leavers are flagged on the ILR through EEF code = 4*
#Learner Type                 | LearnDelFAM | 
#19-24 programme only non-DAS | EEF4        | 
	Given the provider previously submitted the following learner details
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 06/Aug/Last Academic Year | 12 months        | 7500                 | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 19-24 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 100%                        |
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
		| 06/Aug/Last Academic Year | 12 months        | 7500                 | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | 12 months       | completed         | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 19-24 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 100%                        |
	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Contract Type | Aim Sequence Number |
		| pe-1             | 7500                 | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | 0                       |                                        | 0                         |                                          | 100%                        | Act2          | 1                   |
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Second16To18EmployerIncentive | Second16To18ProviderIncentive | Completion16To18FrameworkUplift | Price Episode Identifier |
		| Aug/Current Academic Year | 0            | 1500       | 0         | 500                           | 500                           | 360                             | pe-1                     |
		| Sep/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               | pe-1                     |
		| Oct/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               | pe-1                     |
		| Nov/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               | pe-1                     |
		| Dec/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               | pe-1                     |
		| Jan/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               | pe-1                     |
		| Feb/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               | pe-1                     |
		| Mar/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               | pe-1                     |
		| Apr/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               | pe-1                     |
		| May/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               | pe-1                     |
		| Jun/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               | pe-1                     |
		| Jul/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | 0                               | pe-1                     |
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
