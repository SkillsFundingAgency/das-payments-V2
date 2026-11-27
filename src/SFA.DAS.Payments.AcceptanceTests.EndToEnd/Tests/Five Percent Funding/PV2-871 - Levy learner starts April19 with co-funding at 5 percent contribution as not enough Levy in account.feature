Feature: 5% Contribution from April 2019 PV2-871
As a provider,
I want a Levy learner, starting in April 2019, where learner is co-funded at 5% contribution, as not enough levy in the account
So that I am paid the correct apprenticeship funding by SFA

Scenario Outline: Levy Learner, starts new learning April 2019, 5% contribution, learner is co-funded as not enough levy in the account PV2-871

	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
		
	And the following apprenticeships exist
		| framework code | programme type | pathway code | agreed price | start date                   | end date                  | status | effective from               |
		| 593            | 20             | 1            | 15000        | 01/Apr/2025-26 Academic Year | 01/Apr/2026-27 Academic Year | active | 01/Apr/2025-26 Academic Year |	
    
	And the provider is providing training for the following learners
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                  | SFA Contribution Percentage |
		| 01/Apr/2025-26 Academic Year | 12 months        | 15000                | 01/Apr/2025-26 Academic Year        |                        |                                       |                 | continuing        | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship Levy Contract | 95%                         |

	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
		| pe-1             | 15000                | 01/Apr/2025-26 Academic Year        | 0                      | 01/Apr/2025-26 Academic Year          | 0                       |                                        | 0                         |                                          | Act1          | 1                   | 95%                         |

	When the ILR file is submitted for the learners for collection period <Collection_Period>

	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| Aug/2025-26 Academic Year | 0            | 0          | 0         | pe-1                     |
		| Sep/2025-26 Academic Year | 0            | 0          | 0         | pe-1                     |
		| Oct/2025-26 Academic Year | 0            | 0          | 0         | pe-1                     |
		| Nov/2025-26 Academic Year | 0            | 0          | 0         | pe-1                     |
		| Dec/2025-26 Academic Year | 0            | 0          | 0         | pe-1                     |
		| Jan/2025-26 Academic Year | 0            | 0          | 0         | pe-1                     |
		| Feb/2025-26 Academic Year | 0            | 0          | 0         | pe-1                     |
		| Mar/2025-26 Academic Year | 0            | 0          | 0         | pe-1                     |
		| Apr/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| May/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Jun/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Jul/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |


    And at month end only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
        | R09/2025-26 Academic Year | Apr/2025-26 Academic Year | 1000         | 0          | 0         |
        | R10/2025-26 Academic Year | May/2025-26 Academic Year | 1000         | 0          | 0         |
		| R11/2025-26 Academic Year | Jun/2025-26 Academic Year | 1000         | 0          | 0         |
		| R12/2025-26 Academic Year | Jul/2025-26 Academic Year | 1000         | 0          | 0         |

	And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
        | R09/2025-26 Academic Year | Apr/2025-26 Academic Year | 950                    | 50                          | 0             | Learning         |
        | R10/2025-26 Academic Year | May/2025-26 Academic Year | 950                    | 50                          | 0             | Learning         |
        | R11/2025-26 Academic Year | Jun/2025-26 Academic Year | 950                    | 50                          | 0             | Learning         |
        | R12/2025-26 Academic Year | Jul/2025-26 Academic Year | 950                    | 50                          | 0             | Learning         |

	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
        | R09/2025-26 Academic Year | Apr/2025-26 Academic Year | 950                    | 50                          | 0             | Learning         |
        | R10/2025-26 Academic Year | May/2025-26 Academic Year | 950                    | 50                          | 0             | Learning         |
        | R11/2025-26 Academic Year | Jun/2025-26 Academic Year | 950                    | 50                          | 0             | Learning         |
        | R12/2025-26 Academic Year | Jul/2025-26 Academic Year | 950                    | 50                          | 0             | Learning         |
   
   Examples: 
        | Collection_Period         | Levy Balance |
        | R09/2025-26 Academic Year | 0            |
        | R10/2025-26 Academic Year | 0            |
        | R11/2025-26 Academic Year | 0            |
        | R12/2025-26 Academic Year | 0            |