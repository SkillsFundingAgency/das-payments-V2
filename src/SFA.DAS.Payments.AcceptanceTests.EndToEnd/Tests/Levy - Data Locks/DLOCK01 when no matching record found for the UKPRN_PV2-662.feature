@ignore
Feature: DLOCK01 - When no matching record found in an employer digital account for the UKPRN then datalock DLOCK_01 will be produced PV2-662
		As a provider,
		I want to be notified with a DLOCK01 When no matching record is found in an employer digital account for the UKPRN
		So that I can correct the data mis-match between the Commitment and ILR

Scenario: DLOCK01 - When no matching record found in an employer digital account for the UKPRN then datalock DLOCK_01 will be produced PV2-662
	Given the employer levy account balance in collection period "R12/Current Academic Year" is 10000
	And the following apprenticeships exist
		| Apprenticeship | Provider   | Learner ID | framework code | programme type | pathway code | agreed price | start date                   | end date                  | status | effective from               |
		| Apprentice a   | Provider b | learner a  | 593            | 20             | 1            | 10000        | 01/May/Current Academic Year | 01/May/Next Academic Year | active | 01/May/Current Academic Year |	
	And the "provider a" is providing training for the following learners
		| Learner ID | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| learner a  | 01/May/Current Academic Year | 12 months        | 10000                | 01/May/Current Academic Year        | 0                      | 01/May/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
	When the ILR file is submitted for the learners for the collection period "R12/Current Academic Year" by "provider a"
	Then the following learner earnings should be generated for "provider a"	
		| Delivery Period           | On-Programme | Completion | Balancing |
		| Aug/Current Academic Year | 0            | 0          | 0         |
		| Sep/Current Academic Year | 0            | 0          | 0         |
		| Oct/Current Academic Year | 0            | 0          | 0         |
		| Nov/Current Academic Year | 0            | 0          | 0         |
		| Dec/Current Academic Year | 0            | 0          | 0         |
		| Jan/Current Academic Year | 0            | 0          | 0         |
		| Feb/Current Academic Year | 0            | 0          | 0         |
		| Mar/Current Academic Year | 0            | 0          | 0         |
		| Apr/Current Academic Year | 0            | 0          | 0         |
		| May/Current Academic Year | 666.66667    | 0          | 0         |
		| Jun/Current Academic Year | 666.66667    | 0          | 0         |
		| Jul/Current Academic Year | 666.66667    | 0          | 0         |
	# New step
    And the following non-payable earnings was generated
        | Learner ID | ILR Start Date               | ILR Training Price | framework code | programme type | pathway code |
        | learner a  | 01/May/Current Academic Year | 10000              | 593            | 20             | 1            |

    And the following data lock failures were generated
        | Apprenticeship | Learner ID | ILR Start Date               | Delivery Period           | Transaction Type | Error Code |
        |                | learner a  | 01/May/Current Academic Year | May/Current Academic Year | Learning         | DLOCK 01   |
        |                | learner a  | 01/May/Current Academic Year | Jun/Current Academic Year | Learning         | DLOCK 01   |
        |                | learner a  | 01/May/Current Academic Year | Jul/Current Academic Year | Learning         | DLOCK 01   |
  And Month end is triggered
	And no provider payments will be generated
	And no provider payments will be recorded





#Scenario: DLOCK01 - When no matching record found in an employer digital account for the UKPRN then datalock DLOCK_01 will be produced
#
#    Given the following commitments exist:
#        | commitment Id | version Id | Provider   | ULN       | framework code | programme type | pathway code | agreed price | start date | end date   | status | effective from |
#        | 73            | 73-125     | Provider b | learner a | 450            | 2              | 1            | 10000        | 01/05/2018 | 01/05/2019 | active | 01/05/2018     |
#        
#    When an ILR file is submitted with the following data:  
#        | Provider   | ULN       | framework code | programme type | pathway code | start date | planned end date | completion status | Total training price | Total training price effective date |
#        | Provider a | learner a | 450            | 2              | 1            | 01/05/2018 | 08/08/2019       | continuing        | 10000                | 01/05/2018                          |
#    
#    Then the following data lock event is returned:
#	
#        | Price Episode identifier  | Apprenticeship Id | ULN       | ILR Start Date | ILR Training Price | 
#        | 2-450-1-01/05/2018        | 73                | learner a | 01/05/2018     | 10000              |
#    
#	And the data lock event has the following errors:    
#        | Price Episode identifier  | Error code | Error Description										                                    |
#        | 2-450-1-01/05/2018        | DLOCK_01   | No matching record found in an employer digital account for the UKPRN                     	|
#    
#	And the data lock event has the following periods    
#        | Price Episode identifier | Period   | Payable Flag | Transaction Type |
#        | 2-450-1-01/05/2018       | 1718-R10 | false        | Learning         |
#        | 2-450-1-01/05/2018       | 1718-R11 | false        | Learning         |
#        | 2-450-1-01/05/2018       | 1718-R12 | false        | Learning         |
#    
#	And the data lock event used the following commitments 
#	
#        | Price Episode identifier | Apprentice Version | Start Date | framework code | programme type | pathway code | Negotiated Price | Effective Date |
#        | 2-450-1-01/05/2018       | 73-125             | 01/05/2018 | 450            | 2              | 1            | 10000            | 01/05/2018     |