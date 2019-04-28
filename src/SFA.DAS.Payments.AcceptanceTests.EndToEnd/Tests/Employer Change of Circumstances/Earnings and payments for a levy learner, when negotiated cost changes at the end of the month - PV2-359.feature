Feature: Earnings and payments for a levy learner when the negotiated price changes at the end of the month PV2-359
As a provider,
I want earnings and payments for a levy learner, levy available, and there is a change to the Negotiated Cost which happens at the end of the month to be paid the correct amount
So that I am accurately paid my apprenticeship provision.

Scenario: Earnings and payments for a levy learner when the negotiated price changes at the end of the month PV2-359
Given the employer levy account balance in collection period R04/Current Academic Year is 1500
And the following commitments exist 
	 | start date                   | end date                     | agreed price | effective from               | effective to                 |
	 | 01/Aug/Current Academic Year | 31/Jul/Current Academic Year | 15000        | 01/Aug/Current Academic Year | 31/Oct/Current Academic Year |
	 | 01/Aug/Current Academic Year | 31/Jul/Current Academic Year | 5625         | 03/Nov/Current Academic Year |                              |
And the provider previously submitted the following learner details
    | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assesment Price Effective Date | Completion Status | SFA Contribution Percentage | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  |
    | 01/Aug/Current Academic Year | 12 months        | 12000                | 01/Aug/Current Academic Year        | 3000                   | 01/Aug/Current Academic Year         | continuing        | 90%                         | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract |

And the following earnings had been generated for the learner
    | Delivery Period           | On-Programme | Completion | Balancing |
    | Aug/Current Academic Year | 1000         | 0          | 0         |
    | Sep/Current Academic Year | 1000         | 0          | 0         |
    | Oct/Current Academic Year | 1000         | 0          | 0         |
    | Nov/Current Academic Year | 1000         | 0          | 0         |
    | Dec/Current Academic Year | 1000         | 0          | 0         |
    | Jan/Current Academic Year | 1000         | 0          | 0         |
    | Feb/Current Academic Year | 1000         | 0          | 0         |
    | Mar/Current Academic Year | 1000         | 0          | 0         |
    | Apr/Current Academic Year | 1000         | 0          | 0         |
    | May/Current Academic Year | 1000         | 0          | 0         |
    | Jun/Current Academic Year | 1000         | 0          | 0         |
    | Jul/Current Academic Year | 1000         | 0          | 0         |

And the following provider payments had been generated
    | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | R01/Current Academic Year | Aug/Current Academic Year | 1000          | Learning         |
    | R02/Current Academic Year | Sep/Current Academic Year | 1000          | Learning         |
    | R03/Current Academic Year | Oct/Current Academic Year | 1000          | Learning         |
       
But the Provider now changes the Learner details as follows
    | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assesment Price Effective Date | Completion Status | SFA Contribution Percentage | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  |
    | 01/Aug/Current Academic Year | 12 months        | 5000                 | 03/Nov/Current Academic Year        | 625                    | 03/Nov/Current Academic Year         | continuing        | 90%                         | Act1          | 1                   | ZPROG001      | 52            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract |
		 
When the amended ILR file is re-submitted for the learners in collection period R04/Current Academic Year

Then the following learner earnings should be generated
    | Delivery Period           | On-Programme | Completion | Balancing |
	| Aug/Current Academic Year | 1000         | 0          | 0         |
	| Sep/Current Academic Year | 1000         | 0          | 0         |
	| Oct/Current Academic Year | 1000         | 0          | 0         |
    | Nov/Current Academic Year | 500          | 0          | 0         |
    | Dec/Current Academic Year | 500          | 0          | 0         |
    | Jan/Current Academic Year | 500          | 0          | 0         |
    | Feb/Current Academic Year | 500          | 0          | 0         |
    | Mar/Current Academic Year | 500          | 0          | 0         |
    | Apr/Current Academic Year | 500          | 0          | 0         |
    | May/Current Academic Year | 500          | 0          | 0         |
    | Jun/Current Academic Year | 500          | 0          | 0         |
    | Jul/Current Academic Year | 500          | 0          | 0         |

And at month end only the following payments will be calculated
    | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | Standard Code |
    | R04/Current Academic Year | Nov/Current Academic Year | 500          | 0          | 0         | 52            |

And only the following provider payments will be recorded
    | Collection Period         | Delivery Period           | Levy Payments | Transaction Type | Standard Code |
    | R04/Current Academic Year | Nov/Current Academic Year | 500           | Learning         | 52            |

And only the following provider payments will be generated
    | Collection Period         | Delivery Period           | Levy Payments | Transaction Type | Standard Code |
    | R04/Current Academic Year | Nov/Current Academic Year | 500           | Learning         | 52            |
     
#@ChangeInCircumstances
#Feature: Provider earnings and payments where learner changes apprenticeship standard or there is a change to the negotiated price at the end of a month, remaining with the same employer and provider
#
#    Background:
#        Given The learner is programme only levy
#        And the apprenticeship funding band maximum is 17000
#        And levy balance > agreed price for all months
#
#    Scenario: Earnings and payments for a levy learner, levy available, where the apprenticeship standard changes and data lock is passed in both instances
#        Given the following commitments exist on 03/12/2018:
#            | commitment Id | version Id | ULN       | standard code | start date | end date   | agreed price | effective from | effective to |
#            | 1             | 1-001      | learner a | 51            | 01/08/2018 | 01/08/2019 | 15000        | 01/08/2018     | 31/10/2018   |
#            | 1             | 1-002      | learner a | 52            | 01/08/2018 | 01/08/2019 | 5625         | 03/11/2018     |              |
#        
#        When an ILR file is submitted with the following data:
#            | ULN       | standard code | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date |
#            | learner a | 51            | 03/08/2018 | 01/08/2019       | 31/10/2018      | withdrawn         | 12000                | 03/08/2018                          | 3000                   | 03/08/2018                            |
#            | learner a | 52            | 03/11/2018 | 01/08/2019       |                 | continuing        | 4500                 | 03/11/2018                          | 1125                   | 03/11/2018                            |
#        
#        #Then the data lock status of the ILR in 03/12/2018 is:
#        #    | Payment type | 08/18               | 09/18               | 10/18               | 11/18               | 12/18               |
#        #    | On-program   | commitment 1 v1-001 | commitment 1 v1-001 | commitment 1 v1-001 | commitment 1 v1-002 | commitment 1 v1-002 |
#        
#        Then the provider earnings and payments break down as follows:
#            | Type                       | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 |
#            | Provider Earned Total      | 1000  | 1000  | 1000  | 500   | 500   |
#            | Provider Earned from SFA   | 1000  | 1000  | 1000  | 500   | 500   |
#            | Provider Paid by SFA       | 0     | 1000  | 1000  | 1000  | 500   |
#            | Levy account debited       | 0     | 1000  | 1000  | 1000  | 500   |
#            | SFA Levy employer budget   | 1000  | 1000  | 1000  | 500   | 500   |
#            | SFA Levy co-funding budget | 0     | 0     | 0     | 0     | 0     |