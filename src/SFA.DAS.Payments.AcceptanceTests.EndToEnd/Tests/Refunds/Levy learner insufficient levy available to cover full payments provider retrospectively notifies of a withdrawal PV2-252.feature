@ignore
# Fails to generate levy refund as the earning for periods 4 and 5 are 0 and so the DataLock component adds the earning event period onto the PayableEarningEvent without assigning the correct AccountId
Feature: Levy learner, insufficient levy available to cover full payments, provider retrospectively notifies a withdrawal and previously paid monthly instalments need to be refunded PV2-252
	As a provider,
	I want a Levy learner, where there is insufficient levy available to cover full payments the provider retrospectively notifies a withdrawal and previously paid monthly instalments are refunded
	So that I am accurately paid my apprenticeship provision. PV2-252
 
Scenario:  A levy learner in co-funding and provider retrospectively notifies of a withdrawal after payments have already been made

Given the employer levy account balance in collection period R06/Current Academic Year is 2500

And the following commitments exist
	| Apprenticeship   | Employer   | start date                   | end date                     | agreed price | Standard Code | Programme Type |
	| Apprenticeship 1 | employer 1 | 01/Aug/Current Academic Year | 31/Jul/Current Academic Year | 11250        | 17            | 25             |

And the provider previously submitted the following learner details
    | Start Date             | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assesment Price Effective Date | Actual Duration | Completion Status | SFA Contribution Percentage | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  |
    | start of academic year | 12 months        | 11250                | 01/Aug/Current Academic Year        | 01/Aug/Current Academic Year         |                 | continuing        | 90%                         | Act1          | 1                   | ZPROG001      | 17            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract |

And the following earnings had been generated for the learner
    | Delivery Period           | On-Programme | Completion | Balancing |
    | Aug/Current Academic Year | 750          | 0          | 0         |
    | Sep/Current Academic Year | 750          | 0          | 0         |
    | Oct/Current Academic Year | 750          | 0          | 0         |
    | Nov/Current Academic Year | 750          | 0          | 0         |
    | Dec/Current Academic Year | 750          | 0          | 0         |
    | Jan/Current Academic Year | 750          | 0          | 0         |
    | Feb/Current Academic Year | 750          | 0          | 0         |
    | Mar/Current Academic Year | 750          | 0          | 0         |
    | Apr/Current Academic Year | 750          | 0          | 0         |
    | May/Current Academic Year | 750          | 0          | 0         |
    | Jun/Current Academic Year | 750          | 0          | 0         |
    | Jul/Current Academic Year | 750          | 0          | 0         |

And the following provider payments had been generated
    | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type | Employer   |
    | R01/Current Academic Year | Aug/Current Academic Year | 225                    | 25                          | 500           | Learning         | employer 1 |
    | R02/Current Academic Year | Sep/Current Academic Year | 225                    | 25                          | 500           | Learning         | employer 1 |
    | R03/Current Academic Year | Oct/Current Academic Year | 225                    | 25                          | 500           | Learning         | employer 1 |
    | R04/Current Academic Year | Nov/Current Academic Year | 225                    | 25                          | 500           | Learning         | employer 1 |
    | R05/Current Academic Year | Dec/Current Academic Year | 225                    | 25                          | 500           | Learning         | employer 1 |
        
But the Provider now changes the Learner details as follows
    | Start Date             | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assesment Price Effective Date | Actual Duration | Completion Status | SFA Contribution Percentage | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  |
    | start of academic year | 12 months        | 11250                | 01/Aug/Current Academic Year        | 01/Aug/Current Academic Year         | 3 months        | withdrawn         | 90%                         | Act1          | 1                   | ZPROG001      | 17            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract |
		 
And price details as follows
	| Price Episode Id | Total Training Price | Total Training Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
	| pe-1             | 11250                | 01/Aug/Current Academic Year        | Act1          | 1                   | 90%                         |
	
When the amended ILR file is re-submitted for the learners in collection period R06/Current Academic Year

Then the following learner earnings should be generated
    | Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
    | Aug/Current Academic Year | 750          | 0          | 0         | pe-1                     |
    | Sep/Current Academic Year | 750          | 0          | 0         | pe-1                     |
    | Oct/Current Academic Year | 750          | 0          | 0         | pe-1                     |
    | Nov/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | Dec/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | Jan/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | Feb/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | Mar/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | Apr/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | May/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | Jun/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | Jul/Current Academic Year | 0            | 0          | 0         | pe-1                     |

And at month end only the following payments will be calculated
    | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | Employer   |
    | R06/Current Academic Year | Nov/Current Academic Year | -750         | 0          | 0         | employer 1 |
    | R06/Current Academic Year | Dec/Current Academic Year | -750         | 0          | 0         | employer 1 |

And only the following provider payments will be recorded
    | Collection Period         | Delivery Period           | Levy Payments | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type | Employer   |
    | R06/Current Academic Year | Nov/Current Academic Year | -500          | -225                   | -25                         | Learning         | employer 1 |
    | R06/Current Academic Year | Dec/Current Academic Year | -500          | -225                   | -25                         | Learning         | employer 1 |

And only the following provider payments will be generated
    | Collection Period         | Delivery Period           | Levy Payments | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type | Employer   |
    | R06/Current Academic Year | Nov/Current Academic Year | -500          | -225                   | -25                         | Learning         | employer 1 |
    | R06/Current Academic Year | Dec/Current Academic Year | -500          | -225                   | -25                         | Learning         | employer 1 |


#Scenario:673-AC03 DAS learner, insufficient levy available to cover full payments, provider retrospectively notifies a withdrawal and previously-paid monthly instalments need to be refunded.	Given The learner is programme only DAS
#    Given The learner is programme only DAS
#	And the apprenticeship funding band maximum is 17000
#    And levy balance > agreed price for all months
#	And the following commitments exist:
#		  | commitment Id | version Id | ULN       | start date | end date   | status | agreed price | effective from | effective to | programme type	|
#		  | 1             | 1          | learner a | 01/08/2018 | 01/08/2019 | active | 11250        | 01/08/2018     |              | 25				|
#	And following learning has been recorded for previous payments:
#		| ULN       | start date | aim sequence number |  completion status | programme type	|
#		| learner a | 04/08/2018 | 1                   |  continuing        | 25				|
#  
#	And the following earnings and payments have been made to the provider A for learner a:
#		| Type                          | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 | 01/19 |
#		| Provider Earned Total         | 750   | 750   | 750   | 750   | 750   | 0     |       
#		| Provider Earned from SFA      | 725   | 725   | 725   | 725   | 725   | 0     |       
#		| Provider Earned from Employer | 25    | 25    | 25    | 25    | 25    | 0     |       
#		| Provider Paid by SFA          | 0     | 725   | 725   | 725   | 725   | 725   |        
#		| Payment due from Employer     | 0     | 25    | 25    | 25    | 25    | 25    |       
#		| Levy account debited          | 0     | 500   | 500   | 500   | 500   | 500   |         
#		| SFA Levy employer budget      | 500   | 500   | 500   | 500   | 500   | 0     |        
#		| SFA Levy co-funding budget    | 225   | 225   | 225   | 225   | 225   | 0     |       
#    When an ILR file is submitted for the first time on 10/01/19 with the following data:
#        | ULN       | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date | programme type	|
#        | learner a | 04/08/2018 | 20/08/2019       | 12/11/2018      | withdrawn         | 9000                 | 04/08/2018                          | 2250                   | 04/08/2018                            | 25				|
#	Then the provider earnings and payments break down as follows:
#        | Type                          | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 | 01/19 | 02/19 |
#        | Provider Earned Total         | 750   | 750   | 750   | 0     | 0     | 0     | 0     |
#        | Provider Earned from SFA      | 725   | 725   | 725   | 0     | 0     | 0     | 0     |
#        | Provider Earned from Employer | 25    | 25    | 25    | 0     | 0     | 0     | 0     |
#        | Provider Paid by SFA          | 0     | 725   | 725   | 725   | 725   | 725   | 0     |
#        | Refund taken by SFA           | 0     | 0     | 0     | 0     | 0     | 0     | -1450 |
#        | Payment due from Employer     | 0     | 25    | 25    | 25    | 25    | 25    | 0     |
#        | Levy account debited          | 0     | 500   | 500   | 500   | 500   | 500   | 0     |
#        | Levy account credited         | 0     | 0     | 0     | 0     | 0     | 0     | 1000  |
#        | SFA Levy employer budget      | 500   | 500   | 500   | 0     | 0     | 0     | 0     |
#        | SFA Levy co-funding budget    | 225   | 225   | 225   | 0     | 0     | 0     | 0     |

