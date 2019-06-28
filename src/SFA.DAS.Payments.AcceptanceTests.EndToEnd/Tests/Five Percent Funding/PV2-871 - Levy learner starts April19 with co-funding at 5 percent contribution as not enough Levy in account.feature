Feature: 5% Contribution from April 2019 PV2-871
As a provider,
I want a Levy learner, starting in April 2019, where learner is co-funded at 5% contribution, as not enough levy in the account
So that I am paid the correct apprenticeship funding by SFA

Scenario Outline: Levy Learner, starts new learning April 2019, 5% contribution, learner is co-funded as not enough levy in the account PV2-871

	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
		
	And the following apprenticeships exist
		| framework code | programme type | pathway code | agreed price | start date                   | end date                  | status | effective from               |
		| 593            | 20             | 1            | 15000        | 01/Apr/Current Academic Year | 01/Apr/Next Academic Year | active | 01/Apr/Current Academic Year |	
    
	And the provider is providing training for the following learners
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                  | SFA Contribution Percentage |
		| 01/Apr/Current Academic Year | 12 months        | 15000                | 01/Apr/Current Academic Year        |                        |                                       |                 | continuing        | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship Levy Contract | 95%                         |

	When the ILR file is submitted for the learners for collection period <Collection_Period>

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
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
        | R09/Current Academic Year | Apr/Current Academic Year | 950                    | 50                          | 0             | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 950                    | 50                          | 0             | Learning         |
        | R11/Current Academic Year | Jun/Current Academic Year | 950                    | 50                          | 0             | Learning         |
        | R12/Current Academic Year | Jul/Current Academic Year | 950                    | 50                          | 0             | Learning         |

	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
        | R09/Current Academic Year | Apr/Current Academic Year | 950                    | 50                          | 0             | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 950                    | 50                          | 0             | Learning         |
        | R11/Current Academic Year | Jun/Current Academic Year | 950                    | 50                          | 0             | Learning         |
        | R12/Current Academic Year | Jul/Current Academic Year | 950                    | 50                          | 0             | Learning         |
   
   Examples: 
        | Collection_Period         | Levy Balance |
        | R09/Current Academic Year | 0            |
        | R10/Current Academic Year | 0            |
        | R11/Current Academic Year | 0            |
        | R12/Current Academic Year | 0            |