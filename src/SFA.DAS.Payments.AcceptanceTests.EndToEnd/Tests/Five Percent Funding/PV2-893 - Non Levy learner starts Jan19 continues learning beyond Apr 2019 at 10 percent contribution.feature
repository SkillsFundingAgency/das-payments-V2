Feature: 5% Contribution from April 2019 PV2-893
As a provider,
I want a Non Levy learner, starting in Jan 2019, at 10% contribution
So that I am paid the correct apprenticeship funding by SFA

Scenario Outline: Non Levy Learner, starts learning in Jan19, 10% contribution 

	And the provider is providing training for the following learners
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 06/Jan/Current Academic Year | 12 months        | 15000                | 06/Jan/Current Academic Year        |                        |                                       |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |

   	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
		| pe-1             | 15000                | 06/Jan/Current Academic Year        | 0                      |                                       |                         |                                        | 0                         |                                          | Act2          | 1                   | 90%                         |

	When the ILR file is submitted for the learners for collection period <Collection_Period>

    Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| Aug/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Sep/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Oct/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Nov/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Dec/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Jan/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Feb/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Mar/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Apr/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| May/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Jun/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Jul/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		
    And at month end only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
		| R06/Current Academic Year | Jan/Current Academic Year | 1000         | 0          | 0         |
		| R07/Current Academic Year | Feb/Current Academic Year | 1000         | 0          | 0         |
		| R08/Current Academic Year | Mar/Current Academic Year | 1000         | 0          | 0         |
		| R09/Current Academic Year | Apr/Current Academic Year | 1000         | 0          | 0         |
		| R10/Current Academic Year | May/Current Academic Year | 1000         | 0          | 0         |
		| R11/Current Academic Year | Jun/Current Academic Year | 1000         | 0          | 0         |
		| R12/Current Academic Year | Jul/Current Academic Year | 1000         | 0          | 0         |
			
	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| R06/Current Academic Year | Jan/Current Academic Year | 900                    | 100                         | Learning         |
		| R07/Current Academic Year | Feb/Current Academic Year | 900                    | 100                         | Learning         |
		| R08/Current Academic Year | Mar/Current Academic Year | 900                    | 100                         | Learning         |
		| R09/Current Academic Year | Apr/Current Academic Year | 900                    | 100                         | Learning         |
		| R10/Current Academic Year | May/Current Academic Year | 900                    | 100                         | Learning         |
		| R11/Current Academic Year | Jun/Current Academic Year | 900                    | 100                         | Learning         |
		| R12/Current Academic Year | Jul/Current Academic Year | 900                    | 100                         | Learning         |
														
	And only the following provider payments will be generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| R06/Current Academic Year | Jan/Current Academic Year | 900                    | 100                         | Learning         |
		| R07/Current Academic Year | Feb/Current Academic Year | 900                    | 100                         | Learning         |
		| R08/Current Academic Year | Mar/Current Academic Year | 900                    | 100                         | Learning         |
		| R09/Current Academic Year | Apr/Current Academic Year | 900                    | 100                         | Learning         |
		| R10/Current Academic Year | May/Current Academic Year | 900                    | 100                         | Learning         |
		| R11/Current Academic Year | Jun/Current Academic Year | 900                    | 100                         | Learning         |
		| R12/Current Academic Year | Jul/Current Academic Year | 900                    | 100                         | Learning         |
   Examples: 
        | Collection_Period         |
		| R06/Current Academic Year | 
		| R07/Current Academic Year |
		| R08/Current Academic Year |
		| R09/Current Academic Year | 
		| R10/Current Academic Year |
		| R11/Current Academic Year |
		| R12/Current Academic Year |
        