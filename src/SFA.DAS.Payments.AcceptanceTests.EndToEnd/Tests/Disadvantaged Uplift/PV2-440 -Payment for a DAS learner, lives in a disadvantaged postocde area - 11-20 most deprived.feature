@ignore

#Scenario: Payment for a DAS learner, lives in a disadvantaged postocde area - 11-20% most deprived, employer has sufficient levy funds in account, funding agreed within band maximum, UNDERTAKING APPRENTICESHIP FRAMEWORK
#    #The provider incentive for this postcode group is £150 split equally into 2 payments at 90 and 365 days. INELIGIBLE FOR APPRENITCESHIP STANDARDS
#    Given levy balance > agreed price for all months
#    And the following commitments exist:
#		| commitment Id | version Id | ULN       | start date  | end date   | framework code | programme type | pathway code | agreed price | status   |
#        | 1             | 1          | learner a | 01/08/2018  | 01/08/2019 | 593            | 20             | 1            | 15000        | active   |
#    When an ILR file is submitted with the following data:
#        | ULN       | learner type       | agreed price | start date | planned end date | actual end date | completion status | framework code | programme type | pathway code | home postcode deprivation |
#        | learner a | programme only DAS | 15000        | 06/08/2018 | 08/08/2019       |                 | continuing        | 593            | 20             | 1            | 11-20%                    |
#    Then the provider earnings and payments break down as follows:
#        | Type                                    | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 | ... | 07/19 | 08/19 | 09/19 |
#        | Provider Earned Total                   | 1000  | 1000  | 1000  | 1150  | 1000  | ... | 1000  | 150   | 0     |
#		| Provider Earned from SFA                | 1000  | 1000  | 1000  | 1150  | 1000  | ... | 1000  | 150   | 0     |        
#		| Provider Earned from Employer           | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | Provider Paid by SFA                    | 0     | 1000  | 1000  | 1000  | 1150  | ... | 1000  | 1000  | 150   |
#        | Payment due from Employer               | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | Levy account debited                    | 0     | 1000  | 1000  | 1000  | 1000  | ... | 1000  | 1000  | 0     |
#        | SFA Levy employer budget                | 1000  | 1000  | 1000  | 1000  | 1000  | ... | 1000  | 0     | 0     |
#		| SFA Levy co-funding budget              | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#		| SFA non-Levy co-funding budget          | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#		| SFA non-Levy additional payments budget | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#		| SFA Levy additional payments budget     | 0     | 0     | 0     | 150   | 0     | ... | 0     | 150   | 0     |
#    And the transaction types for the payments are:
#        | Payment type                 | 09/18 | 10/18 | 11/18 | 12/18 | ... | 08/19 | 09/19 |
#        | On-program                   | 1000  | 1000  | 1000  | 1000  | ... | 1000  | 0     |
#        | Completion                   | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | Balancing                    | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | Provider disadvantage uplift | 0     | 0     | 0     | 150   | ... | 0     | 150   |

Feature:
As a provider,
I want a levy learner living in a Disadvantaged Postcode area (11-20% most deprived) to undertake an Apprenticeship Framework course
So that I am accurately paid the Disadvantage Uplift amount of £300 in 2 installments of £150 each at 90 days and 365 days respectively.PV2-440
# For DCT Integration
#ILR entry: <PostcodePrior>OX17 1EZ</PostcodePrior>
Scenario Outline:Levy learner - on framework , Disadvantage Uplift 11-20% paid PV2-440
Given the following commitments exist	
	 | framework code | programme type | pathway code | agreed price | start date                | end date                     | status | effective from            |
	 | 593            | 20             | 1            | 10000        | 01/Aug/Last Academic Year | 01/Aug/Current Academic Year | active | 01/Aug/Last Academic Year |

And the provider previously submitted the following learner details
	| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Programme Type | Pathway Code | Funding Line Type                                  | SFA Contribution Percentage |
	| 06/Aug/Last Academic Year | 12 months        | 15000                | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             |                 | continuing        | Act1          | 1                   | ZPROG001      | 593            | 20             | 1            | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |

And the following earnings had been generated for the learner
    | Delivery Period        | On-Programme | Completion | Balancing | FirstDisadvantagePayment |
    | Aug/Last Academic Year | 1000         | 0          | 0         | 0                        |
    | Sep/Last Academic Year | 1000         | 0          | 0         | 0                        |
    | Oct/Last Academic Year | 1000         | 0          | 0         | 0                        |
    | Nov/Last Academic Year | 1000         | 0          | 0         | 150                      |
    | Dec/Last Academic Year | 1000         | 0          | 0         | 0                        |
    | Jan/Last Academic Year | 1000         | 0          | 0         | 0                        |
    | Feb/Last Academic Year | 1000         | 0          | 0         | 0                        |
    | Mar/Last Academic Year | 1000         | 0          | 0         | 0                        |
    | Apr/Last Academic Year | 1000         | 0          | 0         | 0                        |
    | May/Last Academic Year | 1000         | 0          | 0         | 0                        |
    | Jun/Last Academic Year | 1000         | 0          | 0         | 0                        |
    | Jul/Last Academic Year | 1000         | 0          | 0         | 0                        |
And the following provider payments had been generated
    | Collection Period      | Delivery Period        | Levy Payments | SFA Fully-Funded Payments | Transaction Type         |
    | R01/Last Academic Year | Aug/Last Academic Year | 900           | 0                         | Learning                 |
    | R02/Last Academic Year | Sep/Last Academic Year | 900           | 0                         | Learning                 |
    | R03/Last Academic Year | Oct/Last Academic Year | 900           | 0                         | Learning                 |
    | R04/Last Academic Year | Nov/Last Academic Year | 900           | 0                         | Learning                 |
    | R05/Last Academic Year | Dec/Last Academic Year | 900           | 0                         | Learning                 |
    | R06/Last Academic Year | Jan/Last Academic Year | 900           | 0                         | Learning                 |
    | R07/Last Academic Year | Feb/Last Academic Year | 900           | 0                         | Learning                 |
    | R08/Last Academic Year | Mar/Last Academic Year | 900           | 0                         | Learning                 |
    | R09/Last Academic Year | Apr/Last Academic Year | 900           | 0                         | Learning                 |
    | R10/Last Academic Year | May/Last Academic Year | 900           | 0                         | Learning                 |
    | R11/Last Academic Year | Jun/Last Academic Year | 900           | 0                         | Learning                 |
    | R12/Last Academic Year | Jul/Last Academic Year | 900           | 0                         | Learning                 |
    | R04/Last Academic Year | Nov/Last Academic Year | 0             | 150                       | FirstDisadvantagePayment |

But the Provider now changes the Learner details as follows
	| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Programme Type | Pathway Code | Funding Line Type                                  | SFA Contribution Percentage |
	| 06/Aug/Last Academic Year | 12 months        | 15000                | 06/Aug/Last Academic Year           | 0                      |                                       |                 | continuing        | Act1          | 1                   | ZPROG001      | 593            | 20             | 1            | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
Then the following learner earnings should be generated
	| Delivery Period           | On-Programme | Completion | Balancing | SecondDisadvantagePayment |
	| Aug/Current Academic Year | 0            | 0          | 0         | 150                       |
	| Sep/Current Academic Year | 0            | 0          | 0         | 0                         |
	| Oct/Current Academic Year | 0            | 0          | 0         | 0                         |
	| Nov/Current Academic Year | 0            | 0          | 0         | 0                         |
	| Dec/Current Academic Year | 0            | 0          | 0         | 0                         |
	| Jan/Current Academic Year | 0            | 0          | 0         | 0                         |
	| Feb/Current Academic Year | 0            | 0          | 0         | 0                         |
	| Mar/Current Academic Year | 0            | 0          | 0         | 0                         |
	| Apr/Current Academic Year | 0            | 0          | 0         | 0                         |
	| May/Current Academic Year | 0            | 0          | 0         | 0                         |
	| Jun/Current Academic Year | 0            | 0          | 0         | 0                         |
	| Jul/Current Academic Year | 0            | 0          | 0         | 0                         |

And at month end only the following payments will be calculated
	| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | SecondDisadvantagePayment |
	| R01/Current Academic Year | Aug/Current Academic Year | 0            | 0          | 0         | 150                       |

And only the following provider payments will be recorded
	| Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type          |
	| R01/Current Academic Year | Aug/Current Academic Year | 0             | 150                       | SecondDisadvantagePayment |

And  only the following provider payments will be generated
	| Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type          |
	| R01/Current Academic Year | Aug/Current Academic Year | 0             | 150                       | SecondDisadvantagePayment |

Examples:
    | Collection_Period         |
	| R01/Current Academic Year |
	| R02/Current Academic Year |