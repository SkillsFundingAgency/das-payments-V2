Feature:Levy learner, price is changed and a negative amount is left to be paid resulting in a refund PV2-257
	Levy standard learner, where original commitment is withdrawn after payments made and new commitment is created with lower price and a negative amount is left to be paid - results in a refund
	As a provider,
	I want a levy learner, where original commitment is withdrawn after payments made and new commitment is created with lower price and a negative amount is left to be paid - results in a refund
	So that I am accurately paid my apprenticeship provision PV2-257

Scenario: Levy learner, price is changed and a negative amount is left to be paid resulting in a refund PV2-257

Given the employer levy account balance in collection period R03/Current Academic Year is 15000

And the following commitments exist
	| start date                   | end date                  | agreed price | status | effective from               |	Standard Code | Programme Type |
	| 01/Aug/Current Academic Year | 01/Aug/Next Academic Year | 11250        | active | 01/Aug/Current Academic Year |	51            | 25             |

And the provider previously submitted the following learner details
    | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assesment Price | Total Assesment Price Effective Date | Actual Duration | Completion Status | SFA Contribution Percentage | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  |
    | 04/Aug/Current Academic Year | 12 months        | 9000                 | 04/Aug/Current Academic Year        | 2250                  | 04/Aug/Current Academic Year         |                 | continuing        | 90%                         | Act1          | 1                   | ZPROG001      | 51            | 25             | 19-24 Apprenticeship (From May 2017) Levy Contract |

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
        
But the Commitment details are changed as follows

	| start date                   | end date                  | status  | agreed price | effective from               | stop effective from          |
	| 01/Aug/Current Academic Year | 01/Aug/Next Academic Year | stopped | 11250        | 01/Aug/Current Academic Year | 01/Aug/Current Academic Year |
	| 01/Aug/Current Academic Year | 01/Aug/Next Academic Year | active  | 1            | 01/Aug/Current Academic Year |                              |

And the Provider now changes the Learner details as follows
    | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assesment Price | Total Assesment Price Effective Date | Actual Duration | Completion Status | SFA Contribution Percentage | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  |
    | 04/Aug/Current Academic Year | 12 months        | 1                    | 04/Aug/Current Academic Year        | 0                     | 04/Aug/Current Academic Year         |                 | continuing        | 90%                         | Act1          | 1                   | ZPROG001      | 51            | 25             | 19-24 Apprenticeship (From May 2017) Levy Contract |

And price details as follows
	| Price Episode Id | Total Training Price | Total Training Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
	| pe-1             | 1                    | 04/Aug/Current Academic Year        | Act1          | 1                   | 90%                         |
When the amended ILR file is re-submitted for the learners in collection period R03/Current Academic Year

Then the following learner earnings should be generated
    | Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
    | Aug/Current Academic Year | 0.06667      | 0          | 0         | pe-1                     |
    | Sep/Current Academic Year | 0.06667      | 0          | 0         | pe-1                     |
    | Oct/Current Academic Year | 0.06667      | 0          | 0         | pe-1                     |
    | Nov/Current Academic Year | 0.06667      | 0          | 0         | pe-1                     |
    | Dec/Current Academic Year | 0.06667      | 0          | 0         | pe-1                     |
    | Jan/Current Academic Year | 0.06667      | 0          | 0         | pe-1                     |
    | Feb/Current Academic Year | 0.06667      | 0          | 0         | pe-1                     |
    | Mar/Current Academic Year | 0.06667      | 0          | 0         | pe-1                     |
    | Apr/Current Academic Year | 0.06667      | 0          | 0         | pe-1                     |
    | May/Current Academic Year | 0.06667      | 0          | 0         | pe-1                     |
    | Jun/Current Academic Year | 0.06667      | 0          | 0         | pe-1                     |
    | Jul/Current Academic Year | 0.06667      | 0          | 0         | pe-1                     |

And at month end only the following payments will be calculated
    | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
    | R03/Current Academic Year | Aug/Current Academic Year | -749.93333   | 0          | 0         |
    | R03/Current Academic Year | Sep/Current Academic Year | -749.93333   | 0          | 0         |
    | R03/Current Academic Year | Oct/Current Academic Year | 0.06667      | 0          | 0         |

And only the following provider payments will be recorded
    | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | R03/Current Academic Year | Aug/Current Academic Year | -749.93333    | Learning         |
    | R03/Current Academic Year | Sep/Current Academic Year | -749.93333    | Learning         |
    | R03/Current Academic Year | Oct/Current Academic Year | 0.06667       | Learning         |

And only the following provider payments will be generated
    | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | R03/Current Academic Year | Aug/Current Academic Year | -749.93333    | Learning         |
    | R03/Current Academic Year | Sep/Current Academic Year | -749.93333    | Learning         |
    | R03/Current Academic Year | Oct/Current Academic Year | 0.06667       | Learning         |


#Scenario:Levy standard learner, original commitment is withdrawn after payments made and new commitment is created with lower price and a negative amount is left to be paid - results in a refund
#	Given  the apprenticeship funding band maximum is 15000
#    And levy balance > agreed price for all months
#    And the following commitments exist:    
#        | commitment Id | version Id | ULN       | start date | end date   | status    | agreed price | effective from | effective to | programme type | stop effective from |
#        | 1             | 1          | learner a | 01/08/2018 | 01/08/2019 | cancelled | 11250        | 01/08/2018     |              | 25             | 01/08/2018   |
#        | 2             | 1          | learner a | 01/08/2018 | 01/08/2019 | active    | 1            | 01/08/2018     |              | 25             |              |
#    
#	And following learning has been recorded for previous payments:
#		| ULN       | start date | aim sequence number | completion status | programme type |
#		| Learner a | 04/08/2018 | 1                   | continuing        | 25				|
#	And the following earnings and payments have been made to the provider A for learner a:
#		| Type                           | 08/18 | 09/18 | 10/18 | 11/18 |
#		| Provider Earned Total          | 750   | 750   | 0     | 0     |
#		| Provider Earned from SFA       | 750   | 750   | 0     | 0     |
#		| Provider Earned from Employer  | 0     | 0     | 0     | 0     |
#		| Provider Paid by SFA           | 0     | 750   | 750   | 0     |
#		| Payment due from Employer      | 0     | 0     | 0     | 0     |
#		| Levy account debited           | 0     | 750   | 750   | 0     |
#		| SFA Levy employer budget       | 750   | 750   | 0     | 0     |
#		| SFA Levy co-funding budget     | 0     | 0     | 0     | 0     |
#		| SFA non-Levy co-funding budget | 0     | 0     | 0     | 0     | 
#        
#    When an ILR file is submitted for the first time on 10/10/18 with the following data:
#        | ULN       | learner type       | start date | planned end date | agreed price | completion status | programme type	|
#        | learner a | programme only DAS | 04/08/2018 | 20/08/2019       | 1            | continuing        | 25				|
#	
#    Then the provider earnings and payments break down as follows:
#        | Type                           | 08/18   | 09/18   | 10/18   | 11/18       | 12/18   |
#        | Provider Earned Total          | 0.06667 | 0.06667 | 0.06667 | 0.06667     | 0.06667 |
#        | Provider Earned from SFA       | 0.06667 | 0.06667 | 0.06667 | 0.06667     | 0.06667 |
#        | Provider Earned from Employer  | 0       | 0       | 0       | 0           | 0       |
#        | Provider Paid by SFA           | 0       | 750     | 750     | 0.06667     | 0.06667 |
#        | Refund taken by SFA            | 0       | 0       | 0       | -1499.86666 | 0       |
#        | Payment due from Employer      | 0       | 0       | 0       | 0           | 0       |
#        | Refund due to employer         | 0       | 0       | 0       | 0           | 0       |
#        | Levy account debited           | 0       | 750     | 750     | 0.06667     | 0.06667 |
#        | Levy account credited          | 0       | 0       | 0       | 1499.87     | 00      |
#        | SFA Levy employer budget       | 0.06667 | 0.06667 | 0.06667 | 0.06667     | 0.06667 |
#        | SFA Levy co-funding budget     | 0       | 0       | 0       | 0           | 0       |
#        | SFA non-Levy co-funding budget | 0       | 0       | 0       | 0           | 0       |