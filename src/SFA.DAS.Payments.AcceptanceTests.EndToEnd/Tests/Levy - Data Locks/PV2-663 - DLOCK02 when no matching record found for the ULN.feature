
Feature: DLOCK02 - When no matching record found in an employer digital account for the ULN then datalock DLOCK_02 will be produced PV2-663
		As a provider,
		I want to be notified with a DLOCK02 When no matching record found in an employer digital account for the ULN
		So that I can correct the data mis-match between the Commitment and ILR

Scenario Outline: DLOCK02 - When no matching record found in an employer digital account for the ULN then datalock DLOCK_02 will be produced PV2-663
	Given the employer levy account balance in collection period R10/Current Academic Year is 10000
	And the following commitments exist
		| Identifier       | Learner ID | framework code | programme type | pathway code | agreed price | start date                   | end date                  | status | effective from               |
		| Apprenticeship a | learner b  | 593            | 20             | 1            | 10000        | 01/Aug/Current Academic Year | 01/Aug/Next Academic Year | active | 01/Aug/Current Academic Year |
	And the provider is providing training for the following learners
		| Learner ID | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| learner a  | 01/Aug/Current Academic Year | 12 months        | 10000                | 01/Aug/Current Academic Year        |                        |                                       |                 | continuing        | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
    And price details as follows
		| Price Episode Id  | Total Training Price | Total Training Price Effective Date | Contract Type  | SFA Contribution Percentage |
		| pe-1              | 10000                | 01/Aug/Current Academic Year        | Act1           | 90%                         |
	When the ILR file is submitted for the learners for collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | First16To18EmployerIncentive | First16To18ProviderIncentive | Second16To18EmployerIncentive | Second16To18ProviderIncentive | OnProgramme16To18FrameworkUplift | LearningSupport | FirstDisadvantagePayment | SecondDisadvantagePayment | Learner ID | Price Episode Identifier |
		| Aug/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 0                        | 0                         | learner a  | pe-1                     |
		| Sep/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 0                        | 0                         | learner a  | pe-1                     |
		| Oct/Current Academic Year | 666.66667    | 0          | 0         | 500                          | 500                          | 0                             | 0                             | 120                              | 150             | 0                        | 0                         | learner a  | pe-1                     |
		| Nov/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 300                      | 0                         | learner a  | pe-1                     |
		| Dec/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 0                        | 0                         | learner a  | pe-1                     |
		| Jan/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 0                        | 0                         | learner a  | pe-1                     |
		| Feb/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 0                        | 0                         | learner a  | pe-1                     |
		| Mar/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 0                        | 0                         | learner a  | pe-1                     |
		| Apr/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 0                        | 0                         | learner a  | pe-1                     |
		| May/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 0                        | 0                         | learner a  | pe-1                     |
		| Jun/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 0                             | 0                             | 120                              | 150             | 0                        | 0                         | learner a  | pe-1                     |
		| Jul/Current Academic Year | 666.66667    | 0          | 0         | 0                            | 0                            | 500                           | 500                           | 120                              | 150             | 0                        | 300                       | learner a  | pe-1                     |
     And the following data lock failures were generated
        | Apprenticeship | Learner ID | Delivery Period           | Framework Code | Programme Type | Pathway Code | Transaction Type | Error Code | Price Episode Identifier |
        |                | learner a  | Aug/Current Academic Year | 593            | 20             | 1            | Learning         | DLOCK_02   | pe-1                     |
        |                | learner a  | Sep/Current Academic Year | 593            | 20             | 1            | Learning         | DLOCK_02   | pe-1                     |
        |                | learner a  | Oct/Current Academic Year | 593            | 20             | 1            | Learning         | DLOCK_02   | pe-1                     |
        |                | learner a  | Nov/Current Academic Year | 593            | 20             | 1            | Learning         | DLOCK_02   | pe-1                     |
        |                | learner a  | Dec/Current Academic Year | 593            | 20             | 1            | Learning         | DLOCK_02   | pe-1                     |
        |                | learner a  | Jan/Current Academic Year | 593            | 20             | 1            | Learning         | DLOCK_02   | pe-1                     |
        |                | learner a  | Feb/Current Academic Year | 593            | 20             | 1            | Learning         | DLOCK_02   | pe-1                     |
        |                | learner a  | Mar/Current Academic Year | 593            | 20             | 1            | Learning         | DLOCK_02   | pe-1                     |
        |                | learner a  | Apr/Current Academic Year | 593            | 20             | 1            | Learning         | DLOCK_02   | pe-1                     |
        |                | learner a  | May/Current Academic Year | 593            | 20             | 1            | Learning         | DLOCK_02   | pe-1                     |
        |                | learner a  | Jun/Current Academic Year | 593            | 20             | 1            | Learning         | DLOCK_02   | pe-1                     |
        |                | learner a  | Jul/Current Academic Year | 593            | 20             | 1            | Learning         | DLOCK_02   | pe-1                     |
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


#Scenario: DLOCK02 - When no matching record found in an employer digital account for the ULN then datalock DLOCK_02 will be produced
#
#    Given the following commitments exist:
#        | commitment Id | version Id | Provider   | ULN       | framework code | programme type | pathway code | agreed price | start date | end date   | status | effective from |
#        | 73            | 73-125     | Provider a | learner b | 450            | 2              | 1            | 10000        | 01/05/2018 | 01/05/2019 | active | 01/05/2018     |
#        
#    When an ILR file is submitted with the following data:  
#        | Provider   | ULN       | framework code | programme type | pathway code | start date | planned end date | completion status | Total training price | Total training price effective date |
#        | Provider a | learner a | 450            | 2              | 1            | 01/05/2018 | 08/08/2019       | continuing        | 10000                | 01/05/2018                          |
#    



#    Then no data lock event is returned
