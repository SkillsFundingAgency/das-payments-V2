Feature: Levy learner, with a framework uplift, finishes on time PV2-590
		As a provider,
		I want a Levy learner with a framework uplift, where the learner finishes on time
		So that I am accurately paid my apprenticeship provision

Scenario: A Levy learner with a framework uplift and 16-18 Additional Incentives finishes on time PV2-590

	Given the employer levy account balance in collection period R01/Current Academic Year is 8750
	And the following commitments exist
		| start date                | end date                  | agreed price |Framework Code | Pathway Code | Programme Type |
		| 06/Aug/Last Academic Year | 09/Aug/Next Academic Year | 8250         |593            | 1            | 20             |
	And the provider previously submitted the following learner details
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                 | SFA Contribution Percentage |
		| 06/Aug/Last Academic Year | 12 months        | 8250                 | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             |                 | continuing        | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Levy Contract (non-procured) | 90%                         |
    And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing | OnProgramme16To18FrameworkUplift | First16To18EmployerIncentive | First16To18ProviderIncentive |
        | Aug/Last Academic Year | 550          | 0          | 0         | 120                              | 0                            | 0                            |
        | Sep/Last Academic Year | 550          | 0          | 0         | 120                              | 0                            | 0                            |
        | Oct/Last Academic Year | 550          | 0          | 0         | 120                              | 0                            | 0                            |
        | Nov/Last Academic Year | 550          | 0          | 0         | 120                              | 500                          | 500                          |
        | Dec/Last Academic Year | 550          | 0          | 0         | 120                              | 0                            | 0                            |
        | Jan/Last Academic Year | 550          | 0          | 0         | 120                              | 0                            | 0                            |
        | Feb/Last Academic Year | 550          | 0          | 0         | 120                              | 0                            | 0                            |
        | Mar/Last Academic Year | 550          | 0          | 0         | 120                              | 0                            | 0                            |
        | Apr/Last Academic Year | 550          | 0          | 0         | 120                              | 0                            | 0                            |
        | May/Last Academic Year | 550          | 0          | 0         | 120                              | 0                            | 0                            |
        | Jun/Last Academic Year | 550          | 0          | 0         | 120                              | 0                            | 0                            |
        | Jul/Last Academic Year | 550          | 0          | 0         | 120                              | 0                            | 0                            |
    And the following provider payments had been generated
        | Collection Period      | Delivery Period        | Levy Payments | SFA Fully-Funded Payments | Transaction Type                 |
        | R01/Last Academic Year | Aug/Last Academic Year | 550           | 0                         | Learning                         |
        | R02/Last Academic Year | Sep/Last Academic Year | 550           | 0                         | Learning                         |
        | R03/Last Academic Year | Oct/Last Academic Year | 550           | 0                         | Learning                         |
        | R04/Last Academic Year | Nov/Last Academic Year | 550           | 0                         | Learning                         |
        | R05/Last Academic Year | Dec/Last Academic Year | 550           | 0                         | Learning                         |
        | R06/Last Academic Year | Jan/Last Academic Year | 550           | 0                         | Learning                         |
        | R07/Last Academic Year | Feb/Last Academic Year | 550           | 0                         | Learning                         |
        | R08/Last Academic Year | Mar/Last Academic Year | 550           | 0                         | Learning                         |
        | R09/Last Academic Year | Apr/Last Academic Year | 550           | 0                         | Learning                         |
        | R10/Last Academic Year | May/Last Academic Year | 550           | 0                         | Learning                         |
        | R11/Last Academic Year | Jun/Last Academic Year | 550           | 0                         | Learning                         |
        | R12/Last Academic Year | Jul/Last Academic Year | 550           | 0                         | Learning                         |
        | R01/Last Academic Year | Aug/Last Academic Year | 0             | 120                       | OnProgramme16To18FrameworkUplift |
        | R02/Last Academic Year | Sep/Last Academic Year | 0             | 120                       | OnProgramme16To18FrameworkUplift |
        | R03/Last Academic Year | Oct/Last Academic Year | 0             | 120                       | OnProgramme16To18FrameworkUplift |
        | R04/Last Academic Year | Nov/Last Academic Year | 0             | 120                       | OnProgramme16To18FrameworkUplift |
        | R05/Last Academic Year | Dec/Last Academic Year | 0             | 120                       | OnProgramme16To18FrameworkUplift |
        | R06/Last Academic Year | Jan/Last Academic Year | 0             | 120                       | OnProgramme16To18FrameworkUplift |
        | R07/Last Academic Year | Feb/Last Academic Year | 0             | 120                       | OnProgramme16To18FrameworkUplift |
        | R08/Last Academic Year | Mar/Last Academic Year | 0             | 120                       | OnProgramme16To18FrameworkUplift |
        | R09/Last Academic Year | Apr/Last Academic Year | 0             | 120                       | OnProgramme16To18FrameworkUplift |
        | R10/Last Academic Year | May/Last Academic Year | 0             | 120                       | OnProgramme16To18FrameworkUplift |
        | R11/Last Academic Year | Jun/Last Academic Year | 0             | 120                       | OnProgramme16To18FrameworkUplift |
        | R12/Last Academic Year | Jul/Last Academic Year | 0             | 120                       | OnProgramme16To18FrameworkUplift |
        | R04/Last Academic Year | Nov/Last Academic Year | 0             | 500                       | First16To18EmployerIncentive     |
        | R04/Last Academic Year | Nov/Last Academic Year | 0             | 500                       | First16To18ProviderIncentive     |
    But the Provider now changes the Learner details as follows
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                 | SFA Contribution Percentage |
		| 06/Aug/Last Academic Year | 12 months        | 8250                 | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | 12 months       | completed         | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Levy Contract (non-procured) | 90%                         |
	When the amended ILR file is re-submitted for the learners in collection period R01/Current Academic Year
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Second16To18EmployerIncentive | Second16To18ProviderIncentive | Completion16To18FrameworkUplift |
		| Aug/Current Academic Year | 0            | 1650       | 0         | 500                           | 500                           | 360                             |
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
    And at month end only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | Second16To18EmployerIncentive | Second16To18ProviderIncentive | Completion16To18FrameworkUplift |
		| R01/Current Academic Year | Aug/Current Academic Year | 0            | 1650       | 0         | 500                           | 500                           | 360                             |
	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type                |
		| R01/Current Academic Year | Aug/Current Academic Year | 1650          | 0                         | Completion                      |
		| R01/Current Academic Year | Aug/Current Academic Year | 0             | 500                       | Second16To18EmployerIncentive   |
		| R01/Current Academic Year | Aug/Current Academic Year | 0             | 500                       | Second16To18ProviderIncentive   |
		| R01/Current Academic Year | Aug/Current Academic Year | 0             | 360                       | Completion16To18FrameworkUplift |
	And only the following provider payments will be generated
		| Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type                |
		| R01/Current Academic Year | Aug/Current Academic Year | 1650          | 0                         | Completion                      |
		| R01/Current Academic Year | Aug/Current Academic Year | 0             | 500                       | Second16To18EmployerIncentive   |
		| R01/Current Academic Year | Aug/Current Academic Year | 0             | 500                       | Second16To18ProviderIncentive   |
		| R01/Current Academic Year | Aug/Current Academic Year | 0             | 360                       | Completion16To18FrameworkUplift |