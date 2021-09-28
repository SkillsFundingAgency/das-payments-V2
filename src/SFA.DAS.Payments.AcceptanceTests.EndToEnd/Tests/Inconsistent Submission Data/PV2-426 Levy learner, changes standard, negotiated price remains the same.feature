    Feature: PV2-426 - Levy Learner changes course but price is the same
			As a provider,
			I want to ensure when my levy learner, the changes apprenticeship standard and negotiated price remains the same, to be paid the correct amount
			So that I am accurately paid my apprenticeship provision.

	Scenario Outline: PV2-426 - Levy learner change to standard at the end of a month no change in price

	Given the employer levy account balance in collection period <Collection_Period> is 15500

	And the following commitments exist
		| Identifier       | start date                   | end date                  | agreed price | status | effective from               | effective to                 | Programme Type | Standard Code |
		| Apprenticeship 1 | 01/Aug/Current Academic Year | 01/Aug/Next Academic Year | 15000        | active | 01/Aug/Current Academic Year | 31/Oct/Current Academic Year | 25             | 51            |
		| Apprenticeship 2 | 01/Aug/Current Academic Year | 01/Aug/Next Academic Year | 15000        | active | 03/Nov/Current Academic Year |                              | 25             | 52            |

	And the following aims
		| Aim Type         | Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Programme Type | Standard Code | Funding Line Type             | Completion Status |
		| Programme        | ZPROG001      | 03/Aug/Current Academic Year | 12 months        | 3 months        | 1                   | 25             | 51            | 16-18 Apprenticeship Non-Levy | withdrawn         |
		| Programme        | ZPROG001      | 03/Nov/Current Academic Year | 9 months         |                 | 2                   | 25             | 52            | 16-18 Apprenticeship Non-Levy | continuing        |

	And price details as follows		
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 12000                | 03/Aug/Current Academic Year        | 3000                   | 03/Aug/Current Academic Year          | Act1          | 1                   | 90%                         |
        | pe-2             | 12000                | 03/Nov/Current Academic Year        | 3000                   | 03/Nov/Current Academic Year          | Act1          | 2                   | 90%                         |

    When the ILR file is submitted for the learners for collection period <Collection_Period>
    Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing | Aim Sequence Number | Price Episode Identifier | Contract Type |
        | Aug/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     | Act1          |
        | Sep/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     | Act1          |
        | Oct/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     | Act1          |
        | Nov/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     | Act1          |
        | Dec/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     | Act1          |
        | Jan/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     | Act1          |
        | Feb/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     | Act1          |
        | Mar/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     | Act1          |
        | Apr/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     | Act1          |
        | May/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     | Act1          |
        | Jun/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     | Act1          |
        | Jul/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     | Act1          |

        | Aug/Current Academic Year | 0            | 0          | 0         | 2                   | pe-2                     | Act1          |
        | Sep/Current Academic Year | 0            | 0          | 0         | 2                   | pe-2                     | Act1          |
        | Oct/Current Academic Year | 0            | 0          | 0         | 2                   | pe-2                     | Act1          |
        | Nov/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-2                     | Act1          |
        | Dec/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-2                     | Act1          |
        | Jan/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-2                     | Act1          |
        | Feb/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-2                     | Act1          |
        | Mar/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-2                     | Act1          |
        | Apr/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-2                     | Act1          |
        | May/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-2                     | Act1          |
        | Jun/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-2                     | Act1          |
        | Jul/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-2                     | Act1          |
                                                                                                                               
	And at month end only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
		| R01/Current Academic Year | Aug/Current Academic Year | 1000         | 0          | 0         |
		| R02/Current Academic Year | Sep/Current Academic Year | 1000         | 0          | 0         |
		| R03/Current Academic Year | Oct/Current Academic Year | 1000         | 0          | 0         |
		| R04/Current Academic Year | Nov/Current Academic Year | 1000         | 0          | 0         |
		| R05/Current Academic Year | Dec/Current Academic Year | 1000         | 0          | 0         |
		| R06/Current Academic Year | Jan/Current Academic Year | 1000         | 0          | 0         |
		| R07/Current Academic Year | Feb/Current Academic Year | 1000         | 0          | 0         |
		| R08/Current Academic Year | Mar/Current Academic Year | 1000         | 0          | 0         |
		| R09/Current Academic Year | Apr/Current Academic Year | 1000         | 0          | 0         |
		| R10/Current Academic Year | May/Current Academic Year | 1000         | 0          | 0         |
		| R11/Current Academic Year | Jun/Current Academic Year | 1000         | 0          | 0         |
		| R12/Current Academic Year | Jul/Current Academic Year | 1000         | 0          | 0         |

    And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 1000          | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 1000          | Learning         |
        | R03/Current Academic Year | Oct/Current Academic Year | 1000          | Learning         |
        | R04/Current Academic Year | Nov/Current Academic Year | 1000          | Learning         |
        | R05/Current Academic Year | Dec/Current Academic Year | 1000          | Learning         |
        | R06/Current Academic Year | Jan/Current Academic Year | 1000          | Learning         |
        | R07/Current Academic Year | Feb/Current Academic Year | 1000          | Learning         |
        | R08/Current Academic Year | Mar/Current Academic Year | 1000          | Learning         |
        | R09/Current Academic Year | Apr/Current Academic Year | 1000          | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 1000          | Learning         |
        | R11/Current Academic Year | Jun/Current Academic Year | 1000          | Learning         |
        | R12/Current Academic Year | Jul/Current Academic Year | 1000          | Learning         |

    And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 1000          | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 1000          | Learning         |
        | R03/Current Academic Year | Oct/Current Academic Year | 1000          | Learning         |
        | R04/Current Academic Year | Nov/Current Academic Year | 1000          | Learning         |
        | R05/Current Academic Year | Dec/Current Academic Year | 1000          | Learning         |
        | R06/Current Academic Year | Jan/Current Academic Year | 1000          | Learning         |
        | R07/Current Academic Year | Feb/Current Academic Year | 1000          | Learning         |
        | R08/Current Academic Year | Mar/Current Academic Year | 1000          | Learning         |
        | R09/Current Academic Year | Apr/Current Academic Year | 1000          | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 1000          | Learning         |
        | R11/Current Academic Year | Jun/Current Academic Year | 1000          | Learning         |
        | R12/Current Academic Year | Jul/Current Academic Year | 1000          | Learning         |

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

#Feature: Provider earnings and payments where learner changes apprenticeship standard 
#
#Scenario: Levy learner, changes standard, negotiated price remains the same	
#
#	Given The learner is programme only DAS
#	And the apprenticeship funding band maximum is 17000
#	And levy balance > agreed price for all months
#		
#	And the following commitments exist:
#		| ULN       | priority | start date | end date   | agreed price |
#        | learner a | 1        | 03/08/2018 | 01/08/2019 | 15000        |
#
# 
#	When an ILR file is submitted with the following data:
#        | ULN       | standard code | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date |
#        | learner a | 51            | 03/08/2018 | 01/08/2019       | 01/11/2018      | withdrawn         | 12000                | 03/08/2018                          | 3000                   | 01/08/2018                            |
#        | learner a | 52            | 03/11/2018 | 01/11/2019       |                 | continuing        | 12000                | 03/11/2018                          | 3000                   | 03/11/2018                            |
#       		
#	Then the provider earnings and payments break down as follows:
#		
#        | Type                       		| 08/18   | 09/18   | 10/18   | 11/18   | 12/18   |
#        | Provider Earned Total      		| 1000    | 1000    | 1000    | 1000    | 1000    | 
#        | Provider Earned from SFA   		| 1000    | 1000    | 1000    | 1000    | 1000    |
#        | Provider Earned from Employer 	| 0       |    0    |    0    |    0    |    0    |            
#		| Provider Paid by SFA       		| 0       | 1000    | 1000    | 1000    | 1000    |
#        | Payment due from Employer         | 0       |    0    |    0    |    0    |    0    | 
#		| Levy account debited       		| 0       | 1000    | 1000    | 1000    | 1000    |
#        | SFA Levy employer budget   		| 1000    | 1000    | 1000    | 1000    | 1000    |
#        | SFA Levy co-funding budget 		| 0       |    0    |    0    |    0    |    0    |
#		| SFA non-Levy co-funding budget	| 0       |    0    |    0    |    0    |    0    | 			
#
#	And the transaction types for the payments are:
#		| Payment type                   | 09/18 | 10/18 | 11/18 | 12/18  |
#		| On-program                     | 1000  | 1000  | 1000  | 1000   |
#		| Completion                     | 0     | 0     | 0     | 0      |
#		| Balancing                      | 0     | 0     | 0     | 0      |
#		| English and maths on programme | 0     | 0     | 0     | 0      |
#		| English and maths Balancing    | 0     | 0     | 0     | 0      |