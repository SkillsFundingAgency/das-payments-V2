
Feature: DLOCK07(a) - When price is changed, then effective to is set on previous price episode PV2-677
		I want to be notified with a DLOCK07(a) when the price is changed, and the effective to is set on previous price episode
		So that I can correct the data mis-match between the Commitment and ILR - PV2-677

Scenario: DLOCK07(a) - When price is changed, then effective to is set on previous price episode PV2-677
	Given the employer levy account balance in collection period R12/Current Academic Year is 14000 
	And the following commitments exist
 		| Identifier       | framework code | programme type | pathway code | agreed price | start date                   | end date                  | status | effective from               | effective to                 |
 		| Apprenticeship a | 593            | 20             | 1            | 10000        | 01/May/Current Academic Year | 01/May/Next Academic Year | active | 01/May/Current Academic Year | 30/Jun/Current Academic Year |
 		| Apprenticeship a | 593            | 20             | 1            | 15000        | 01/May/Current Academic Year | 01/May/Next Academic Year | active | 01/Jul/Current Academic Year |                              |		
	And the provider is providing training for the following learners
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework code | Programme type | Pathway code | Funding Line Type                                  | SFA Contribution Percentage |
		| 01/May/Current Academic Year | 12 months        | 10000                | 01/May/Current Academic Year        | continuing        | Act1          | 1                   | ZPROG001      | 593            | 20             | 1            | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
	And price details as follows
		| Price Episode Id  | Total Training Price | Total Training Price Effective Date | Contract Type  | SFA Contribution Percentage |
		| pe-1              | 10000                | 01/May/Current Academic Year        | Act1           | 90%                         |
		| pe-2              | 14000                | 01/Jul/Current Academic Year        | Act1           | 90%                         |
	When the ILR file is submitted for the learners for collection period R12/Current Academic Year
	Then the following learner earnings should be generated
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
		| Jul/Current Academic Year | 1120         | 0          | 0         |
	And the following data lock failures were generated
		| Apprenticeship   | Delivery Period           | Framework Code | Programme Type | Pathway Code | Transaction Type | Error Code | Price Episode Identifier |
		| Apprenticeship a | Jul/Current Academic Year | 593            | 20             | 1            | Learning         | DLOCK_07   | pe-2                     |
	And at month end only the following payments will be calculated
		| Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
		| R10/Current Academic Year | May/Current Academic Year | 666.66667     | Learning         |
		| R11/Current Academic Year | Jun/Current Academic Year | 666.66667     | Learning         |
	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
		| R10/Current Academic Year | May/Current Academic Year | 666.66667     | Learning         |
		| R11/Current Academic Year | Jun/Current Academic Year | 666.66667     | Learning         |
	And only the following provider payments will be generated
		| Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
		| R10/Current Academic Year | May/Current Academic Year | 666.66667     | Learning         |
		| R11/Current Academic Year | Jun/Current Academic Year | 666.66667     | Learning         |
	


#Scenario: DLOCK07(a) - When price is changed, then effective to is set on previous price episode
#
#    Given the following commitments exist:
#        | commitment Id | version Id | Provider   | ULN       | framework code | programme type | pathway code | agreed price | start date | end date   | status | effective from | effective to |
#        | 73            | 73-125     | Provider a | learner a | 450            | 2              | 1            | 10000        | 01/05/2018 | 01/05/2019 | active | 01/05/2018     | 30/06/2018   |
#        | 73            | 73-200     | Provider a | learner a | 450            | 2              | 1            | 15000        | 01/05/2018 | 01/05/2019 | active | 01/07/2018     |              |
#        
#    When an ILR file is submitted with the following data:  
#        | Provider   | ULN       | framework code | programme type | pathway code | start date | planned end date | completion status | Total training price 1 | Total training price 1 effective date | Total training price 2 | Total training price 2 effective date |
#        | Provider a | learner a | 450            | 2              | 1            | 01/05/2018 | 08/08/2019       | continuing        | 10000                  | 01/05/2018                            | 14000                  | 01/07/2018                            |
#    
#    Then the following data lock event is returned:
#        | Price Episode identifier | Apprenticeship Id | ULN       | ILR Start Date | ILR Training Price | ILR Effective from | ILR Effective to |
#        | 2-450-1-01/05/2018       | 73                | learner a | 01/05/2018     | 10000              | 01/05/2018         | 30/06/2018       |
#        | 2-450-1-01/07/2018       | 73                | learner a | 01/05/2018     | 14000              | 01/07/2018         |                  |
#        
#    And the data lock event has the following errors:    
#        | Price Episode identifier  | Error code | Error Description										|
#        | 2-450-1-01/07/2018        | DLOCK_07   | No matching record found in the employer digital account for the negotiated cost of training	|
#        
#    And the data lock event has the following periods    
#        | Price Episode identifier | Period   | Payable Flag | Transaction Type |
#        | 2-450-1-01/05/2018       | 1718-R10 | true         | Learning         |
#        | 2-450-1-01/05/2018       | 1718-R11 | true         | Learning         |
#        | 2-450-1-01/07/2018       | 1718-R12 | false        | Learning         |
#        
#    And the data lock event used the following commitments   
#        | Price Episode identifier | Apprentice Version | Start Date | framework code | programme type | pathway code | Negotiated Price | Effective Date |
#        | 2-450-1-01/05/2018       | 73-125             | 01/05/2018 | 450            | 2              | 1            | 10000            | 01/05/2018     |
#        | 2-450-1-01/07/2018       | 73-200             | 01/05/2018 | 450            | 2              | 1            | 15000            | 01/07/2018     |

