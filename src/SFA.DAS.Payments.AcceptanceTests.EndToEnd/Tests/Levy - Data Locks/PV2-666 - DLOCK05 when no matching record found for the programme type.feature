
Feature: DLOCK05 - When no matching record found in an employer digital account for for the programme type then datalock DLOCK_05 will be produced PV2-666
 		As a Provider,
 		I want to be notified with a DLOCK05 when no matching record found in an employer digital account for the Programme Type
 		So that I can correct the data mis-match between the Commitment and ILR PV2-666
 
Scenario: DLOCK05 - When no matching record found in an employer digital account for for the programme type then datalock DLOCK_05 will be produced PV2-666
 
	Given the employer levy account balance in collection period R12/Current Academic Year is 11000 
	And the following commitments exist
 		| Identifier       | framework code | programme type | pathway code | agreed price | start date                   | end date                  | status | effective from               |
		| Apprenticeship a | 593            | 20             | 1            | 10000        | 01/May/Current Academic Year | 01/May/Next Academic Year | active | 01/May/Current Academic Year |				
	And the provider is providing training for the following learners
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework code | Programme type | Pathway code | Funding Line Type                                  | SFA Contribution Percentage |
		| 01/May/Current Academic Year | 12 months        | 10000                | 01/May/Current Academic Year        | continuing        | Act1          | 1                   | ZPROG001      | 593            | 21             | 1            | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
    And price details as follows
		| Price Episode Id  | Total Training Price | Total Training Price Effective Date | Contract Type  | SFA Contribution Percentage |
		| pe-1              | 10000                | 01/May/Current Academic Year        | Act1           | 90%                         |
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
		| Jul/Current Academic Year | 666.66667    | 0          | 0         |
	And the following data lock failures were generated
        | Apprenticeship   | Delivery Period           | Framework Code | Programme Type | Pathway Code | Transaction Type | Error Code | Price Episode Identifier |
        | Apprenticeship a | May/Current Academic Year | 593            | 21             | 1            | Learning         | DLOCK_05   | pe-1                     |
        | Apprenticeship a | Jun/Current Academic Year | 593            | 21             | 1            | Learning         | DLOCK_05   | pe-1                     |
        | Apprenticeship a | Jul/Current Academic Year | 593            | 21             | 1            | Learning         | DLOCK_05   | pe-1                     |
	And Month end is triggered
	And no provider payments will be generated
	And no provider payments will be recorded


		#Scenario: DLOCK05 - When no matching record found in an employer digital account for the programme type then datalock DLOCK_05 will be produced
 #   
	#Given the following commitments exist:
	#
 #       | commitment Id | version Id | Provider   | ULN       | framework code | programme type | pathway code | agreed price | start date | end date   | status | effective from |
 #       | 73            | 73-125     | Provider a | learner a | 450            | 3              | 1            | 10000        | 01/May/Last Academic Year | 01/05/2019 | active | 01/May/Last Academic Year     |
 #       
 #   
	#When an ILR file is submitted with the following data:  
 #       | Provider   | ULN       | framework code | programme type | pathway code | start date | planned end date | completion status | Total training price | Total training price effective date |
 #       | Provider a | learner a | 450            | 2              | 1            | 01/May/Last Academic Year | 08/08/2019       | continuing        | 10000                | 01/May/Last Academic Year                          |
 #   
 #   
	#Then the following data lock event is returned:
 #       | Price Episode identifier  | Apprenticeship Id | ULN       | ILR Start Date | ILR Training Price | 
 #       | 21-593-1-01/May/Last Academic Year        | 73                | learner a | 01/May/Last Academic Year     | 10000              |
 #   
	#And the data lock event has the following errors:    
 #       | Price Episode identifier  | Error code | Error Description										                       |
 #       | 21-593-1-01/May/Last Academic Year        | DLOCK_05   | No matching record found in the employer digital account for the programme type |
 #   
	#And the data lock event has the following periods    
 #       | Price Episode identifier | Period   | Payable Flag | Transaction Type |
 #       | 21-593-1-01/May/Last Academic Year       | 1718-R10 | false        | Learning         |
 #       | 21-593-1-01/May/Last Academic Year       | 1718-R11 | false        | Learning         |
 #       | 21-593-1-01/May/Last Academic Year       | 1718-R12 | false        | Learning         |
 #   
	#And the data lock event used the following commitments   
 #       | Price Episode identifier | Apprentice Version | Start Date | framework code | programme type | pathway code | Negotiated Price | Effective Date |
 #       | 21-593-1-01/May/Last Academic Year       | 73-125             | 01/May/Last Academic Year | 450            | 3              | 1            | 10000            | 01/May/Last Academic Year     |
