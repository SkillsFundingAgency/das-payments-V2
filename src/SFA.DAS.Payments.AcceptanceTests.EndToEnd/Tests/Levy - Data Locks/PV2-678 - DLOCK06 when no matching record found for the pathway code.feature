
Feature: DLOCK06 - When no matching record found in an employer digital account for the pathway code then datalock DLOCK_06 will be produced PV2-678
	As a Provider,
	I want to be notified with a DLOCK06 when no matching record found in an employer digital account for the pathway code
	So that I can correct the data mis-match between the Commitment and ILR - PV2-668
 
Scenario Outline: DLOCK06 - When no matching record found in an employer digital account for the pathway code then datalock DLOCK_06 will be produced PV2-678
 
 Given the employer levy account balance in collection period R12/Current Academic Year is 11000 
	And the following apprenticeships exist
 		| Identifier       | framework code | programme type | pathway code | agreed price | start date                   | end date                  | status | effective from               |
		| Apprenticeship a | 593            | 20             | 1            | 10000        | 01/Aug/Current Academic Year | 01/Aug/Next Academic Year | active | 01/Aug/Current Academic Year |				
	And the provider is providing training for the following learners
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework code | Programme type | Pathway code | Funding Line Type                                  | SFA Contribution Percentage |
		| 01/Aug/Current Academic Year | 12 months        | 10000                | 01/Aug/Current Academic Year        | continuing        | Act1          | 1                   | ZPROG001      | 593            | 20             | 2            | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
    And price details as follows
		| Price Episode Id  | Total Training Price | Total Training Price Effective Date | Contract Type  | SFA Contribution Percentage |
		| pe-1              | 10000                | 01/Aug/Current Academic Year        | Act1           | 90%                         |
	When the ILR file is submitted for the learners for collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | First16To18EmployerIncentive | First16To18ProviderIncentive | Second16To18EmployerIncentive | Second16To18ProviderIncentive | OnProgramme16To18FrameworkUplift | LearningSupport | FirstDisadvantagePayment | SecondDisadvantagePayment |  Price Episode Identifier |
		| Aug/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 0                        | 0                         |  pe-1                     |
		| Sep/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 0                        | 0                         |  pe-1                     |
		| Oct/Current Academic Year | 666.66667    | 0          | 0         | 500                          | 500                          | 0                             | 0                             | 120                              | 150             | 0                        | 0                         |  pe-1                     |
		| Nov/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 300                      | 0                         |  pe-1                     |
		| Dec/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 0                        | 0                         |  pe-1                     |
		| Jan/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 0                        | 0                         |  pe-1                     |
		| Feb/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 0                        | 0                         |  pe-1                     |
		| Mar/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 0                        | 0                         |  pe-1                     |
		| Apr/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 0                        | 0                         |  pe-1                     |
		| May/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 0                        | 0                         |  pe-1                     |
		| Jun/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 0                        | 0                         |  pe-1                     |
		| Jul/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 500                           | 500                           | 120                              | 150             | 0                        | 300                       |  pe-1                     |
	And the following data lock failures were generated
        | Apprenticeship   | Delivery Period           | Framework Code | Programme Type | Pathway Code | Transaction Type | Error Code | Price Episode Identifier |
        | Apprenticeship a | Aug/Current Academic Year | 593            | 20             | 2          | Learning         | DLOCK_06   | pe-1                     |
		| Apprenticeship a | Sep/Current Academic Year | 593            | 20             | 2          | Learning         | DLOCK_06   | pe-1                     |
		| Apprenticeship a | Oct/Current Academic Year | 593            | 20             | 2          | Learning         | DLOCK_06   | pe-1                     |
		| Apprenticeship a | Nov/Current Academic Year | 593            | 20             | 2          | Learning         | DLOCK_06   | pe-1                     |
		| Apprenticeship a | Dec/Current Academic Year | 593            | 20             | 2          | Learning         | DLOCK_06   | pe-1                     |
		| Apprenticeship a | Jan/Current Academic Year | 593            | 20             | 2          | Learning         | DLOCK_06   | pe-1                     |
		| Apprenticeship a | Feb/Current Academic Year | 593            | 20             | 2          | Learning         | DLOCK_06   | pe-1                     |
		| Apprenticeship a | Mar/Current Academic Year | 593            | 20             | 2          | Learning         | DLOCK_06   | pe-1                     |
		| Apprenticeship a | Apr/Current Academic Year | 593            | 20             | 2          | Learning         | DLOCK_06   | pe-1                     |
		| Apprenticeship a | May/Current Academic Year | 593            | 20             | 2          | Learning         | DLOCK_06   | pe-1                     |
		| Apprenticeship a | Jun/Current Academic Year | 593            | 20             | 2          | Learning         | DLOCK_06   | pe-1                     |
		| Apprenticeship a | Jul/Current Academic Year | 593            | 20             | 2          | Learning         | DLOCK_06   | pe-1                     |
	And Month end is triggered
	And no provider payments will be generated
	And no provider payments will be recorded
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





#Scenario: DLOCK06 - When no matching record found in an employer digital account for the pathway code then datalock DLOCK_06 will be produced
#
#    Given the following commitments exist:
#        | commitment Id | version Id | Provider   | ULN       | framework code | programme type | pathway code | agreed price | start date | end date   | status | effective from |
#        | 73            | 73-125     | Provider a | learner a | 450            | 2              | 6            | 10000        | 01/May/Last Academic Year | 01/05/2019 | active | 01/May/Last Academic Year     |
#        
#    When an ILR file is submitted with the following data:  
#        | Provider   | ULN       | framework code | programme type | pathway code | start date | planned end date | completion status | Total training price | Total training price effective date |
#        | Provider a | learner a | 450            | 2              | 1            | 01/May/Last Academic Year | 08/08/2019       | continuing        | 10000                | 01/May/Last Academic Year                          |
#    
#    Then the following data lock event is returned:
#        | Price Episode identifier  | Apprenticeship Id | ULN       | ILR Start Date | ILR Training Price | 
#        | 2-450-1-01/May/Last Academic Year        | 73                | learner a | 01/05/2017     | 10000              |
#    And the data lock event has the following errors:    
#        | Price Episode identifier  | Error code | Error Description										                       |
#        | 2-450-1-01/May/Last Academic Year        | DLOCK_06   | No matching record found in the employer digital account for the pathway code   |
#    And the data lock event has the following periods    
#        | Price Episode identifier | Period   | Payable Flag | Transaction Type |
#        | 2-450-1-01/May/Last Academic Year       | 1718-R10 | false        | Learning         |
#        | 2-450-1-01/May/Last Academic Year       | 1718-R11 | false        | Learning         |
#        | 2-450-1-01/May/Last Academic Year       | 1718-R12 | false        | Learning         |
#    And the data lock event used the following commitments   
#        | Price Episode identifier | Apprentice Version | Start Date | framework code | programme type | pathway code | Negotiated Price | Effective Date |
#        | 2-450-1-01/May/Last Academic Year       | 73-125             | 01/May/Last Academic Year | 450            | 2              | 6            | 10000            | 01/May/Last Academic Year     |
