Feature: PV2-1464 Ignore_Apprenticeship_That_Stop_Within_A_Day_Of_Starting
	
Scenario Outline: PV2-1464 - Ignore_Apprenticeship_That_Stop_Within_A_Day_Of_Starting
	Given the employer IsLevyPayer flag is true
	And the following commitments exist
		| Identifier       | framework code | programme type | pathway code | agreed price | start date                    | end date                  | status  | effective from               | Stop Effective From          |
		| Apprenticeship a | 594            | 20             | 1            | 10000        | 01/Aug/Current Academic Year  | 01/Aug/Next Academic Year | stopped | 01/Aug/Current Academic Year | 02/Aug/Current Academic Year |
		| Apprenticeship b | 594            | 20             | 1            | 10000        | 01/Aug/Current Academic Year  | 01/Aug/Next Academic Year | stopped | 01/Aug/Current Academic Year | 01/Aug/Current Academic Year |
		| Apprenticeship c | 593            | 20             | 1            | 10000        | 01/Aug/Current Academic Year  | 01/Aug/Next Academic Year | active  | 01/Aug/Current Academic Year |                              |
	
	
	And the provider is providing training for the following learners
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework code | Programme type | Pathway code | Funding Line Type                                  | SFA Contribution Percentage |
		| 01/Aug/Current Academic Year | 12 months        | 10000                | 01/Aug/Current Academic Year        | continuing        | Act1          | 1                   | ZPROG001      | 594            | 20             | 1            | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
    
	And price details as follows
		| Price Episode Id  | Total Training Price | Total Training Price Effective Date | Contract Type  | SFA Contribution Percentage |
		| pe-1              | 10000                | 01/Aug/Current Academic Year        | Act1           | 90%                         |	

	When the ILR file is submitted for the learners for collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | First16To18EmployerIncentive | First16To18ProviderIncentive | Second16To18EmployerIncentive | Second16To18ProviderIncentive | OnProgramme16To18FrameworkUplift | LearningSupport | FirstDisadvantagePayment | SecondDisadvantagePayment | Price Episode Identifier |
		| Aug/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 0                        | 0                         | pe-1                     |
		| Sep/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 0                        | 0                         | pe-1                     |
		| Oct/Current Academic Year | 666.66667    | 0          | 0         | 500                          | 500                          | 0                             | 0                             | 120                              | 150             | 0                        | 0                         | pe-1                     |
		| Nov/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 300                      | 0                         | pe-1                     |
		| Dec/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 0                        | 0                         | pe-1                     |
		| Jan/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 0                        | 0                         | pe-1                     |
		| Feb/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 0                        | 0                         | pe-1                     |
		| Mar/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 0                        | 0                         | pe-1                     |
		| Apr/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 0                        | 0                         | pe-1                     |
		| May/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 0                        | 0                         | pe-1                     |
		| Jun/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 0                        | 0                         | pe-1                     |
		| Jul/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 500                           | 500                           | 120                              | 150             | 0                        | 300                       | pe-1                     |
	
	And the following data lock failures were generated
        | Apprenticeship   | Delivery Period           | Framework Code | Programme Type | Pathway Code | Transaction Type | Error Code | Price Episode Identifier |
        | Apprenticeship c | Aug/Current Academic Year | 594            | 20             | 1            | Learning         | DLOCK_04   | pe-1                     |
		| Apprenticeship c | Sep/Current Academic Year | 594            | 20             | 1            | Learning         | DLOCK_04   | pe-1                     |
		| Apprenticeship c | Oct/Current Academic Year | 594            | 20             | 1            | Learning         | DLOCK_04   | pe-1                     |
		| Apprenticeship c | Nov/Current Academic Year | 594            | 20             | 1            | Learning         | DLOCK_04   | pe-1                     |
		| Apprenticeship c | Dec/Current Academic Year | 594            | 20             | 1            | Learning         | DLOCK_04   | pe-1                     |
		| Apprenticeship c | Jan/Current Academic Year | 594            | 20             | 1            | Learning         | DLOCK_04   | pe-1                     |
		| Apprenticeship c | Feb/Current Academic Year | 594            | 20             | 1            | Learning         | DLOCK_04   | pe-1                     |
		| Apprenticeship c | Mar/Current Academic Year | 594            | 20             | 1            | Learning         | DLOCK_04   | pe-1                     |
		| Apprenticeship c | Apr/Current Academic Year | 594            | 20             | 1            | Learning         | DLOCK_04   | pe-1                     |
		| Apprenticeship c | May/Current Academic Year | 594            | 20             | 1            | Learning         | DLOCK_04   | pe-1                     |
		| Apprenticeship c | Jun/Current Academic Year | 594            | 20             | 1            | Learning         | DLOCK_04   | pe-1                     |
		| Apprenticeship c | Jul/Current Academic Year | 594            | 20             | 1            | Learning         | DLOCK_04   | pe-1                     |

	And Month end is triggered
	And no provider payments will be recorded
	And no provider payments will be generated
	

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



