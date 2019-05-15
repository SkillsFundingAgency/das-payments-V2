@ignore
#Feature: 5% Contribution from April 2019
#
#Scenario: Non Levy Learner, started learning before Apr19, takes a planned break and resumes from Apr19, moves into co-funding Levy on 10% contribution
#
#Background: The example is demonstrating a learner flagged as 'Non Levy' starts learning pre Apr 2019, takes a planned break and resumes in Apr19
#			and moves into co-funding Levy with 10% contribution from Apr19 as the learning is existing course
#	
#    Given The learner is programme only Levy 
#	And the apprenticeship funding band maximum is 15000
#	
#	And the Contract type in the ILR is:
#        | contract type | date from  | date to    |
#        | Non-DAS       | 03/01/2019 | 28/02/2019 |
#        | DAS 	 	    | 03/04/2019 | 01/04/2020 |
#	
#	And the following commitments exist on 03/03/2019:
#        | commitment Id | version Id | ULN       | priority | start date | planned end date | actual end date | completion status | effective from | effective to |
#        | 1             | 1          | learner a | 1        | 01/04/2019 | 01/04/2020       | 			      | continuing	      | 01/04/2019     | 		      |
#	
#	
#	When an ILR file is submitted on 03/03/2019 with the following data:
#		| ULN       | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date |
#        | learner a | 03/01/2019 | 04/01/2020       | 28/02/2019      | planned break     | 12000                | 03/01/2019                          | 3000                   | 03/01/2019                            |
#        | learner a | 03/04/2019 | 03/04/2020       |                 | continuing        | 12000                | 03/04/2019                          | 3000                   | 03/04/2019                            |
#        
#
#   		
#	Then the provider earnings and payments break down as follows:		
#        | Type                       		| 01/19 | 02/18 | 03/19 | 04/19 | 05/19 | ... | 03/20 | 04/20 |
#        | Provider Earned Total      		| 1000  | 1000  | 0     | 1000  | 1000  | ... | 1000  | 0     |
#        | Provider Earned from SFA   		| 900   | 900   | 0     | 900   | 900   | ... | 900   | 0     |
#        | Provider Earned from Employer 	| 100   | 100   | 0     | 100   | 100   | ... | 100   | 0     |            
#		| Provider Paid by SFA       		| 0     | 900   | 900   | 0     | 900   | ... | 900   | 900   |
#        | Payment due from Employer         | 0     | 100   | 100   | 0     | 100   | ... | 100   | 100   | 
#		| Levy account debited       		| 0     | 0     | 0     | 0     | 0     |  ...| 0     | 0     |
#        | SFA Levy employer budget   		| 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | SFA Levy co-funding budget 		| 0     | 0     | 0     | 900   | 900   | ... | 900   | 0     |
#		| SFA non-Levy co-funding budget	| 900   | 900   | 0     | 0     | 0     | ... | 0     | 0     | 
#
#	And the transaction types for the payments are:
#        | Payment type                 | 02/19 | 03/19 | 04/19 | 05/19 | ... | 03/20 | 04/20 |
#        | On-program                   | 900   | 900   | 0     | 900   | ... | 900   | 900   |
#        | Completion                   | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | Balancing                    | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | Employer 16-18 incentive     | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | Provider 16-18 incentive     | 0     | 0     | 0     | 0     | ... | 0     | 0     |

Feature: 5% Contribution from April 2019 PV2-909


Scenario Outline: Non Levy Learner, started learning before Apr19, has planned break and resumes learning from Apr19, remains on 10% contribution PV2-909

    Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>

	And the following apprenticeships exist
		| Apprenticeship | Provider   | Learner ID | standard code | programme type | agreed price | start date                   | end date                  | status | effective from               |
		| Apprentice a   | Provider a | learner a  | 17            | 25             | 15000        | 01/Apr/Current Academic Year | 01/Apr/Next Academic Year | active | 01/Apr/Current Academic Year |	

	And the provider previously submitted the following learner details
        | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
        | 01/Jan/Current Academic Year | 12 months        | 12000                | 01/Jan/Current Academic Year        | 3000                   | 01/Jan/Current Academic Year          | 2 months        | planned break     | Act2          | 1                   | ZPROG001      | 17            | 25             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |

    And the following earnings had been generated for the learner
        | Delivery Period           | On-Programme | Completion | Balancing |
        | Aug/Current Academic Year | 0            | 0          | 0         |
        | Sep/Current Academic Year | 0            | 0          | 0         |
        | Oct/Current Academic Year | 0            | 0          | 0         |
        | Nov/Current Academic Year | 0            | 0          | 0         |
        | Dec/Current Academic Year | 0            | 0          | 0         |
        | Jan/Current Academic Year | 1000         | 0          | 0         |
        | Feb/Current Academic Year | 1000         | 0          | 0         |
        | Mar/Current Academic Year | 0            | 0          | 0         |
        | Apr/Current Academic Year | 0            | 0          | 0         |
        | May/Current Academic Year | 0            | 0          | 0         |
        | Jun/Current Academic Year | 0            | 0          | 0         |
        | Jul/Current Academic Year | 0            | 0          | 0         |
    And the following provider payments had been generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | R06/Current Academic Year | Jan/Current Academic Year | 900                    | 100                         | Learning         |
		| R07/Current Academic Year | Feb/Current Academic Year | 900                    | 100                         | Learning         |
        		
    But the Provider now changes the Learner details as follows
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status |  Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 01/Apr/Current Academic Year | 10 months        | 12000                | 01/Apr/Current Academic Year        | 3000                   | 03/Apr/Current Academic Year          |                 | continuing        |  1                   | ZPROG001      | 17            | 25             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |

	And the Contract Type details as follows 
		| Contract Type | Effective From               | Effective To                 |
		| Act2          | 01/Jan/Current Academic Year | 28/Feb/Current Academic Year |
		| Act1          | 01/Apr/Current Academic Year |                              |

	And price details as follows
        | Price details     | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | SFA Contribution Percentage |
        | 1st price details | 12000                | 01/Jan/Current Academic Year        | 3000                   | 01/Jan/Current Academic Year          | 90%                         |
        | 2nd price details | 12000                | 01/Apr/Current Academic Year        | 3000                   | 01/Apr/Current Academic Year          | 90%                         |

	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing |
        | Aug/Current Academic Year | 0            | 0          | 0         |
        | Sep/Current Academic Year | 0            | 0          | 0         |
        | Oct/Current Academic Year | 0            | 0          | 0         |
        | Nov/Current Academic Year | 0            | 0          | 0         |
        | Dec/Current Academic Year | 0            | 0          | 0         |
        | Jan/Current Academic Year | 1000         | 0          | 0         |
        | Feb/Current Academic Year | 1000         | 0          | 0         |
        | Mar/Current Academic Year | 0            | 0          | 0         |
        | Apr/Current Academic Year | 1000         | 0          | 0         |
        | May/Current Academic Year | 1000         | 0          | 0         |
        | Jun/Current Academic Year | 1000         | 0          | 0         |
        | Jul/Current Academic Year | 1000         | 0          | 0         |
    And at month end only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |       
        | R09/Current Academic Year | Apr/Current Academic Year | 1000         | 0          | 0         |
		| R10/Current Academic Year | May/Current Academic Year | 1000         | 0          | 0         |
		| R11/Current Academic Year | Jun/Current Academic Year | 1000         | 0          | 0         |
		| R12/Current Academic Year | Jul/Current Academic Year | 1000         | 0          | 0         |
         
	And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |       
        | R09/Current Academic Year | Apr/Current Academic Year | 900                    | 100                         | Learning         |
		| R10/Current Academic Year | May/Current Academic Year | 900                    | 100                         | Learning         |
		| R11/Current Academic Year | Jun/Current Academic Year | 900                    | 100                         | Learning         |
		| R12/Current Academic Year | Jul/Current Academic Year | 900                    | 100                         | Learning         |
        
	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |        
        | R09/Current Academic Year | Apr/Current Academic Year | 900                    | 100                         | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 900                    | 100                         | Learning         |
        | R11/Current Academic Year | Jun/Current Academic Year | 900                    | 100                         | Learning         |
        | R12/Current Academic Year | Jul/Current Academic Year | 900                    | 100                         | Learning         |
        
	Examples: 
        | Collection_Period         | Levy Balance |
		| R09/Current Academic Year | 0            |
		| R10/Current Academic Year | 0            |
		| R11/Current Academic Year | 0            |
		| R12/Current Academic Year | 0            |
