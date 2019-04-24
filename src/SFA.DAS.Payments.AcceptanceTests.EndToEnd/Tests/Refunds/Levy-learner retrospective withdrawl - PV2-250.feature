
Feature: Levy Learner is withdrawn retrospectively PV2-250
	As a provider,
	I want a levy learner, where levy is available the provider retrospectively notifies a withdrawal and previously paid monthly instalments are refunded
	So that I am accurately paid my apprenticeship provision.

Scenario:  Provider retrospectively notifies of a withdrawal for a levy learner after payments have already been made PV2-250

Given the employer levy account balance in collection period R06/Current Academic Year is 15000

And the following commitments exist

	| start date                   | end date                     | agreed price |
	| 01/Aug/Current Academic Year | 31/Jul/Current Academic Year | 11250        |

And the provider previously submitted the following learner details
    | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assesment Price | Total Assesment Price Effective Date | Actual Duration | Completion Status | SFA Contribution Percentage | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  |
    | 04/Aug/Current Academic Year | 12 months        | 9000                 | 01/Aug/Current Academic Year        | 2250                  | 01/Aug/Current Academic Year         |                 | continuing        | 90%                         | Act1          | 1                   | ZPROG001      | 17            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract |

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
    | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | R01/Current Academic Year | Aug/Current Academic Year | 750           | Learning         |
    | R02/Current Academic Year | Sep/Current Academic Year | 750           | Learning         |
    | R03/Current Academic Year | Oct/Current Academic Year | 750           | Learning         |
    | R04/Current Academic Year | Nov/Current Academic Year | 750           | Learning         |
    | R05/Current Academic Year | Dec/Current Academic Year | 750           | Learning         |
        
But the Provider now changes the Learner details as follows
    | Start Date                   | Planned Duration | Total Training Price | Total Assessment Price | Total Training Price Effective Date | Total Assesment Price Effective Date | Actual Duration | Completion Status | SFA Contribution Percentage | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  |
    | 04/Aug/Current Academic Year | 12 months        | 9000                 | 2250                   | 01/Aug/Current Academic Year        | 01/Aug/Current Academic Year         | 3 months        | withdrawn         | 90%                         | Act1          | 1                   | ZPROG001      | 17            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract |
		 
When the amended ILR file is re-submitted for the learners in collection period R06/Current Academic Year

Then the following learner earnings should be generated
    | Delivery Period           | On-Programme | Completion | Balancing |
    | Aug/Current Academic Year | 750          | 0          | 0         |
    | Sep/Current Academic Year | 750          | 0          | 0         |
    | Oct/Current Academic Year | 750          | 0          | 0         |
    | Nov/Current Academic Year | 0            | 0          | 0         |
    | Dec/Current Academic Year | 0            | 0          | 0         |
    | Jan/Current Academic Year | 0            | 0          | 0         |
    | Feb/Current Academic Year | 0            | 0          | 0         |
    | Mar/Current Academic Year | 0            | 0          | 0         |
    | Apr/Current Academic Year | 0            | 0          | 0         |
    | May/Current Academic Year | 0            | 0          | 0         |
    | Jun/Current Academic Year | 0            | 0          | 0         |
    | Jul/Current Academic Year | 0            | 0          | 0         |

And at month end only the following payments will be calculated
    | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
    | R06/Current Academic Year | Nov/Current Academic Year | -750         | 0          | 0         |
    | R06/Current Academic Year | Dec/Current Academic Year | -750         | 0          | 0         |

And only the following provider payments will be recorded
    | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | R06/Current Academic Year | Nov/Current Academic Year | -750          | Learning         |
    | R06/Current Academic Year | Dec/Current Academic Year | -750          | Learning         |

And only the following provider payments will be generated
    | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | R06/Current Academic Year | Nov/Current Academic Year | -750          | Learning         |
    | R06/Current Academic Year | Dec/Current Academic Year | -750          | Learning         |




#Scenario: Levy learner, levy available, provider retrospectively notifies a withdrawal and previously-paid monthly instalments need to be refunded.
#
#  Given The learner is programme only DAS
#    And the apprenticeship funding band maximum is 17000
#    And levy balance > agreed price for all months
# And the following commitments exist:
#		| commitment Id | version Id | ULN       | start date | end date   | status | agreed price | effective from | effective to | programme type |
#		| 1             | 1          | learner a | 04/08/2018 | 01/08/2019 | active | 11250        | 01/08/2018     |              | 25				|
#
#	And following learning has been recorded for previous payments:
#		| ULN       | start date | aim sequence number |  completion status | programme type	|
#		| learner a | 04/08/2018 | 1                   |  continuing        | 25				|
#  
# And the following earnings and payments have been made to the provider A for learner a:
#	    | Type                          | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 | 01/19 |
#        | Provider Earned Total         | 750   | 750   | 750   | 750   | 750   | 0     |       
#        | Provider Earned from SFA      | 750   | 750   | 750   | 750   | 750   | 0     |       
#        | Provider Earned from Employer | 0     | 0     | 0     | 0     | 0     | 0     |       
#        | Provider Paid by SFA          | 0     | 750   | 750   | 750   | 750   | 750   |        
#        | Payment due from Employer     | 0     | 0     | 0     | 0     | 0     | 0     |       
#        | Levy account debited          | 0     | 750   | 750   | 750   | 750   | 750   |         
#        | SFA Levy employer budget      | 750   | 750   | 750   | 750   | 750   | 0     |        
#        | SFA Levy co-funding budget    | 0     | 0     | 0     | 0     | 0     | 0     |       
#    When an ILR file is submitted for the first time on 10/01/18 with the following data:
#        | ULN       | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date | programme type	|
#        | learner a | 04/08/2018 | 20/08/2019       | 12/11/2018      | withdrawn         | 9000                 | 04/08/2018                          | 2250                   | 04/08/2018                            | 25				|
#	Then the provider earnings and payments break down as follows:
#        | Type                          | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 | 01/19 | 02/19 |
#        | Provider Earned Total         | 750   | 750   | 750   | 0     | 0     | 0     | 0     |
#        | Provider Earned from SFA      | 750   | 750   | 750   | 0     | 0     | 0     | 0     |
#        | Provider Earned from Employer | 0     | 0     | 0     | 0     | 0     | 0     | 0     |
#        | Provider Paid by SFA          | 0     | 750   | 750   | 750   | 750   | 750   | 0     |
#        | Refund taken by SFA           | 0     | 0     | 0     | 0     | 0     | 0     | -1500 |
#        | Payment due from Employer     | 0     | 0     | 0     | 0     | 0     | 0     | 0     |
#        | Levy account debited          | 0     | 750   | 750   | 750   | 750   | 750   | 0     |
#        | Levy account credited         | 0     | 0     | 0     | 0     | 0     | 0     | 1500  |
#        | SFA Levy employer budget      | 750   | 750   | 750   | 0     | 0     | 0     | 0     |
#        | SFA Levy co-funding budget    | 0     | 0     | 0     | 0     | 0     | 0     | 0     |


