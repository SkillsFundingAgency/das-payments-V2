Feature: Levy learner, levy available, 2 providers - one provider retrospectively notifies a withdrawal need to be refunded PV2-253
	As a provider,
	I want a levy learner, where levy is available the provider retrospectively notifies a withdrawal and previously paid monthly instalments are refunded
	So that I am accurately paid my apprenticeship provision.

Scenario: Levy learner, levy available, 2 providers - one provider retrospectively notifies a withdrawal need to be refunded
	
Given the employer levy account balance in collection period R06/Current Academic Year is 0

And the following commitments exist 
  | Identifier        | Learner ID | Provider   | start date                   | end date                     | status | agreed price |
  | Apprentiiship 1   | learner a  | provider a | 01/Aug/Current Academic Year | 31/Jul/Current Academic Year | active | 5625         |
  | Apprentiiship 2   | learner b  | provider b | 01/Aug/Current Academic Year | 31/Jul/Current Academic Year | active | 11250        |

And the "provider a" previously submitted the following learner details
	| Learner ID | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | SFA Contribution Percentage | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  |
	| learner a  | 01/Aug/Current Academic Year | 12 months        | 5625                 | 01/Aug/Current Academic Year        | 0                      | 01/Aug/Current Academic Year          |                 | continuing        | 90%                         | Act1          | 1                   | ZPROG001      | 25            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract |

And the "provider b" previously submitted the following learner details
	| Learner ID | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | SFA Contribution Percentage | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  |
	| learner b  | 01/Aug/Current Academic Year | 12 months        | 11250                | 01/Aug/Current Academic Year        | 0                      | 01/Aug/Current Academic Year          |                 | continuing        | 90%                         | Act1          | 1                   | ZPROG001      | 25            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract |

And the following earnings had been generated for the learner for "provider a"
    | Learner ID | Delivery Period           | On-Programme | Completion | Balancing |
    | learner a  | Aug/Current Academic Year | 375          | 0          | 0         |
    | learner a  | Sep/Current Academic Year | 375          | 0          | 0         |
    | learner a  | Oct/Current Academic Year | 375          | 0          | 0         |
    | learner a  | Nov/Current Academic Year | 375          | 0          | 0         |
    | learner a  | Dec/Current Academic Year | 375          | 0          | 0         |
    | learner a  | Jan/Current Academic Year | 375          | 0          | 0         |
    | learner a  | Feb/Current Academic Year | 375          | 0          | 0         |
    | learner a  | Mar/Current Academic Year | 375          | 0          | 0         |
    | learner a  | Apr/Current Academic Year | 375          | 0          | 0         |
    | learner a  | May/Current Academic Year | 375          | 0          | 0         |
    | learner a  | Jun/Current Academic Year | 375          | 0          | 0         |
    | learner a  | Jul/Current Academic Year | 375          | 0          | 0         |  

And the following earnings had been generated for the learner for "provider b"
	| Learner ID | Delivery Period           | On-Programme | Completion | Balancing |
	| learner b  | Aug/Current Academic Year | 750          | 0          | 0         |
	| learner b  | Sep/Current Academic Year | 750          | 0          | 0         |
	| learner b  | Oct/Current Academic Year | 750          | 0          | 0         |
	| learner b  | Nov/Current Academic Year | 750          | 0          | 0         |
	| learner b  | Dec/Current Academic Year | 750          | 0          | 0         |
	| learner b  | Jan/Current Academic Year | 750          | 0          | 0         |
	| learner b  | Feb/Current Academic Year | 750          | 0          | 0         |
	| learner b  | Mar/Current Academic Year | 750          | 0          | 0         |
	| learner b  | Apr/Current Academic Year | 750          | 0          | 0         |
	| learner b  | May/Current Academic Year | 750          | 0          | 0         |
	| learner b  | Jun/Current Academic Year | 750          | 0          | 0         |
	| learner b  | Jul/Current Academic Year | 750          | 0          | 0         |

And the following "provider a" payments had been generated
    | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | learner a  | R01/Current Academic Year | Aug/Current Academic Year | 375           | Learning         |
    | learner a  | R02/Current Academic Year | Sep/Current Academic Year | 375           | Learning         |
    | learner a  | R03/Current Academic Year | Oct/Current Academic Year | 375           | Learning         |
    | learner a  | R04/Current Academic Year | Nov/Current Academic Year | 375           | Learning         |
    | learner a  | R05/Current Academic Year | Dec/Current Academic Year | 375           | Learning         |
      
And the following "provider b" payments had been generated
    | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | learner b  | R01/Current Academic Year | Aug/Current Academic Year | 750           | Learning         |
    | learner b  | R02/Current Academic Year | Sep/Current Academic Year | 750           | Learning         |
    | learner b  | R03/Current Academic Year | Oct/Current Academic Year | 750           | Learning         |
    | learner b  | R04/Current Academic Year | Nov/Current Academic Year | 750           | Learning         |
    | learner b  | R05/Current Academic Year | Dec/Current Academic Year | 750           | Learning         |
          
But the "provider b" now changes the Learner details as follows
	| Learner ID | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | SFA Contribution Percentage | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  |
	| learner b  | 01/Aug/Current Academic Year | 12 months        | 11250                | 01/Aug/Current Academic Year        | 0                      | 01/Aug/Current Academic Year          | 3 months        | withdrawn         | 90%                         | Act1          | 1                   | ZPROG001      | 25            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract |

And the "provider a" now changes the Learner details as follows
	| Learner ID | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | SFA Contribution Percentage | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  |
	| learner a  | 01/Aug/Current Academic Year | 12 months        | 5625                 | 01/Aug/Current Academic Year        | 0                      | 01/Aug/Current Academic Year          |                 | continuing        | 90%                         | Act1          | 1                   | ZPROG001      | 25            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract |
	
When the amended ILR file is re-submitted for the learners in the collection period R06/Current Academic Year by "provider b"
And the ILR file is submitted for the learners for the collection period R06/Current Academic Year by "provider a"
	
Then the following learner earnings should be generated for "provider a"
    | Learner ID | Delivery Period           | On-Programme | Completion | Balancing |
    | learner a  | Aug/Current Academic Year | 375          | 0          | 0         |
    | learner a  | Sep/Current Academic Year | 375          | 0          | 0         |
    | learner a  | Oct/Current Academic Year | 375          | 0          | 0         |
    | learner a  | Nov/Current Academic Year | 375          | 0          | 0         |
    | learner a  | Dec/Current Academic Year | 375          | 0          | 0         |
    | learner a  | Jan/Current Academic Year | 375          | 0          | 0         |
    | learner a  | Feb/Current Academic Year | 375          | 0          | 0         |
    | learner a  | Mar/Current Academic Year | 375          | 0          | 0         |
    | learner a  | Apr/Current Academic Year | 375          | 0          | 0         |
    | learner a  | May/Current Academic Year | 375          | 0          | 0         |
    | learner a  | Jun/Current Academic Year | 375          | 0          | 0         |
    | learner a  | Jul/Current Academic Year | 375          | 0          | 0         |

And the following learner earnings should be generated for "provider b"
	| Learner ID | Delivery Period           | On-Programme | Completion | Balancing |
	| learner b  | Aug/Current Academic Year | 750          | 0          | 0         |
	| learner b  | Sep/Current Academic Year | 750          | 0          | 0         |
	| learner b  | Oct/Current Academic Year | 750          | 0          | 0         |
	| learner b  | Nov/Current Academic Year | 0            | 0          | 0         |
	| learner b  | Dec/Current Academic Year | 0            | 0          | 0         |
	| learner b  | Jan/Current Academic Year | 0            | 0          | 0         |
	| learner b  | Feb/Current Academic Year | 0            | 0          | 0         |
	| learner b  | Mar/Current Academic Year | 0            | 0          | 0         |
	| learner b  | Apr/Current Academic Year | 0            | 0          | 0         |
	| learner b  | May/Current Academic Year | 0            | 0          | 0         |
	| learner b  | Jun/Current Academic Year | 0            | 0          | 0         |
	| learner b  | Jul/Current Academic Year | 0            | 0          | 0         |

And at month end only the following payments will be calculated for "provider a"
    | Learner ID | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
    | learner a  | R06/Current Academic Year | Jan/Current Academic Year | 375          | 0          | 0         |
 
And at month end only the following payments will be calculated for "provider b"
    | Learner ID | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
    | learner b  | R06/Current Academic Year | Nov/Current Academic Year | -750         | 0          | 0         |
    | learner b  | R06/Current Academic Year | Dec/Current Academic Year | -750         | 0          | 0         |
    | learner b  | R06/Current Academic Year | Jan/Current Academic Year | 0            | 0          | 0         |

And Month end is triggered

And only the following "provider a" payments will be recorded
    | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | learner a  | R06/Current Academic Year | Jan/Current Academic Year | 375           | Learning         |

And only the following "provider b" payments will be recorded
    | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | learner b  | R06/Current Academic Year | Nov/Current Academic Year | -750          | Learning         |
    | learner b  | R06/Current Academic Year | Dec/Current Academic Year | -750          | Learning         |
    | learner b  | R06/Current Academic Year | Jan/Current Academic Year | 0             | Learning         |
        
And only the following "provider a" payments will be generated
    | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | learner a  | R06/Current Academic Year | Jan/Current Academic Year | 375           | Learning         |

And only the following "provider b" payments will be generated
    | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | learner b  | R06/Current Academic Year | Nov/Current Academic Year | -750          | Learning         |
    | learner b  | R06/Current Academic Year | Dec/Current Academic Year | -750          | Learning         |
    | learner b  | R06/Current Academic Year | Jan/Current Academic Year | 0             | Learning         |
       
#Scenario: 683-AC01- DAS learner, levy available, 2 providers - provider A and provider B, Provider B retrospectively notifies a withdrawal and previously-paid monthly instalments need to be refunded, 
#and this refund must be credited to Provider A where refunded Levy amount will be in excess of the available Levy for that period.
#
#	Given The learner is programme only DAS
#	And the apprenticeship funding band maximum is 17000
#	And the employer's levy balance is:
#        | 01/19 | 02/19 | 03/19 | 04/19 | 05/19 | 06/19 | 07/19 |
#        | 0     | 1125  | 1125  | 1125  | 1125  | 1125  | 1125  |
#	And the following commitments exist:
#		| commitment Id | Provider   | version Id | ULN       | start date | end date   | status | standard code | programme type	| agreed price | effective from | effective to |
#		| 1             | Provider A | 1          | Learner A | 01/08/2018 | 01/08/2019 | active | 25            | 25				| 5625         | 01/08/2018     |              |
#		| 2             | Provider B | 1          | Learner B | 01/08/2018 | 01/08/2019 | active | 25            | 25				| 11250        | 01/08/2018     |              |
#	
#	And following learning has been recorded for previous payments:
#		| ULN       | start date | aim sequence number | completion status | standard code | programme type |
#		| Learner A | 04/08/2018 | 1                   | continuing        | 25            | 25				|
#		| Learner B | 04/08/2018 | 1                   | continuing        | 25            | 25				|
#
#
	#And the following earnings and payments have been made to the Provider A for Learner A:
	#	| Type                                | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 |01/19|     
	#	| Provider Earned Total               | 375   | 375   | 375   | 375   | 375   | 0   |
	#	| Provider Paid by SFA                | 375   | 375   | 375   | 375   | 375   | 0   | 
	#	| Provider Earned from Employer       | 0     | 0     | 0     | 0     |  0    | 0   |
	#	| Provider Paid by SFA                | 0     | 375   | 375   | 375   | 375   | 375 |
	#	| Payment due from Employer           | 0     | 0     | 0     | 0     |   0   | 0   |
	#	| Levy account debited                | 0     | 375   | 375   | 375   | 375   | 0   | 
	#	| SFA Levy employer budget            | 375   | 375   | 375   | 375   | 375   | 0   | 
	#	| SFA Levy co-funding budget          | 0     | 0     | 0     | 0     | 0     | 0   | 
#	And the following earnings and payments have been made to the Provider B for Learner B:
#		| Type                          | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 | 01/19 |
#		| Provider Earned Total         | 750   | 750   | 750   | 750   | 750   | 0     |
#		| Provider Earned from SFA      | 750   | 750   | 750   | 750   | 750   | 0     |
#		| Provider Earned from Employer | 0     | 0     | 0     | 0     | 0     | 0     |
#		| Provider Paid by SFA          | 0     | 750   | 750   | 750   | 750   | 750   |
#		| Payment due from Employer     | 0     | 0     | 0     | 0     | 0     | 0     |
#		| Levy account debited          | 0     | 750   | 750   | 750   | 750   | 750   |
#		| SFA Levy employer budget      | 750   | 750   | 750   | 750   | 750   | 0     |
#		| SFA Levy co-funding budget    | 0     | 0     | 0     | 0     | 0     | 0     |  
#		
#    When an ILR file is submitted for the first time on 10/01/19 with the following data:
#		| ULN       | Provider   | start date | planned end date | actual end date | completion status | standard code | programme type		| Total training price | Total training price effective date | Total assessment price | Total assessment price effective date |
#		| Learner A | Provider A | 04/08/2018 | 20/08/2019       |                 | Continuing        | 25            | 25					| 4500                 | 04/08/2018                          | 1125                   | 04/08/2018                            |
#		| Learner B | Provider B | 04/08/2018 | 20/08/2019       | 12/11/2018      | withdrawn         | 25            | 25					| 9000                 | 04/08/2018                          | 2250                   | 04/08/2018                            |
#
#    Then OBSOLETE - the earnings and payments break down for provider A is as follows:
#		| Type                          | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 | 01/19 | 02/19 |
#		| Provider Earned Total         | 375   | 375   | 375   | 375   | 375   | 375   | 375   |
#		| Provider Earned from SFA      | 375   | 375   | 375   | 375   | 375   | 375   | 375   |
#		| Provider Earned from Employer | 0     | 0     | 0     | 0     | 0     | 0     | 0     |
#		| Provider Paid by SFA          | 0     | 375   | 375   | 375   | 375   | 375   | 375   |
#		| Refund taken by SFA           | 0     | 0     | 0     | 0     | 0     | 0     | 0     |
#		| Payment due from Employer     | 0     | 0     | 0     | 0     | 0     | 0     | 0     |
#		| Levy account debited          | 0     | 375   | 375   | 375   | 375   | 375   | 375   |
#		| Levy account credited         | 0     | 0     | 0     | 0     | 0     | 0     | 0     |
#		| SFA Levy employer budget      | 375   | 375   | 375   | 375   | 375   | 375   | 375   |
#		| SFA Levy co-funding budget    | 0     | 0     | 0     | 0     | 0     | 0     | 0     |
#
#    Then OBSOLETE - the earnings and payments break down for provider B is as follows:
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
#	 And the net effect on employer's levy balance after each period end is:
#        | 01/19 | 02/19 | 03/19 | 04/19 | 05/19 | 06/19 | 07/19 |
#        | -1125 | 750   | 750   | 750   | 750   | 750   | 750   |