Feature: One Non-levy learner - change to start date within calendar month, backwards in month PV2-283

As a provider,
I want a non-levy learner, where change to start date within calendar month, backwards in month is paid the correct amount
So that I am accurately paid my apprenticeship provision.

@EndToEnd

Scenario Outline: Change to start date within calendar month, backwards in month PV2-283
Given the provider previously submitted the following learner details
	| ULN       | Priority | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
	| learner a | 1        | 05/Aug/Current Academic Year | 12 months        | 9000                 | 05/Aug/Current Academic Year        | 0                      | 05/Aug/Current Academic Year          |                 | continuing        | Act2          | 1                   | ZPROG001      | 403            | 1            | 2              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |
And the following earnings had been generated for the learner
    | Delivery Period           | On-Programme | Completion | Balancing |
    | Aug/Current Academic Year | 600          | 0          | 0         |
    | Sep/Current Academic Year | 600          | 0          | 0         |
    | Oct/Current Academic Year | 600          | 0          | 0         |
    | Nov/Current Academic Year | 600          | 0          | 0         |
    | Dec/Current Academic Year | 600          | 0          | 0         |
    | Jan/Current Academic Year | 600          | 0          | 0         |
    | Feb/Current Academic Year | 600          | 0          | 0         |
    | Mar/Current Academic Year | 600          | 0          | 0         |
    | Apr/Current Academic Year | 600          | 0          | 0         |
    | May/Current Academic Year | 600          | 0          | 0         |
    | Jun/Current Academic Year | 600          | 0          | 0         |
    | Jul/Current Academic Year | 600          | 0          | 0         |
And the following provider payments had been generated
    | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
    | R01/Current Academic Year | Aug/Current Academic Year | 540                    | 60                          | Learning         |
    | R02/Current Academic Year | Sep/Current Academic Year | 540                    | 60                          | Learning         |
But the Provider now changes the Learner details as follows
	| ULN       | Priority | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
	| learner a | 1        | 04/Aug/Current Academic Year | 12 months        | 9000                 | 04/Aug/Current Academic Year        | 0                      | 04/Aug/Current Academic Year          |                 | continuing        | Act2          | 1                   | ZPROG001      | 403            | 1            | 2              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |
When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
Then the following learner earnings should be generated
	| Delivery Period           | On-Programme | Completion | Balancing |
	| Aug/Current Academic Year | 600          | 0          | 0         |
	| Sep/Current Academic Year | 600          | 0          | 0         |
	| Oct/Current Academic Year | 600          | 0          | 0         |
	| Nov/Current Academic Year | 600          | 0          | 0         |
	| Dec/Current Academic Year | 600          | 0          | 0         |
	| Jan/Current Academic Year | 600          | 0          | 0         |
	| Feb/Current Academic Year | 600          | 0          | 0         |
	| Mar/Current Academic Year | 600          | 0          | 0         |
	| Apr/Current Academic Year | 600          | 0          | 0         |
	| May/Current Academic Year | 600          | 0          | 0         |
	| Jun/Current Academic Year | 600          | 0          | 0         |
	| Jul/Current Academic Year | 600          | 0          | 0         |
And only the following payments will be calculated
    | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
    | R03/Current Academic Year | Oct/Current Academic Year | 600          | 0          | 0         |
    | R04/Current Academic Year | Nov/Current Academic Year | 600          | 0          | 0         |
    | R05/Current Academic Year | Dec/Current Academic Year | 600          | 0          | 0         |
    | R06/Current Academic Year | Jan/Current Academic Year | 600          | 0          | 0         |
    | R07/Current Academic Year | Feb/Current Academic Year | 600          | 0          | 0         |
    | R08/Current Academic Year | Mar/Current Academic Year | 600          | 0          | 0         |
    | R09/Current Academic Year | Apr/Current Academic Year | 600          | 0          | 0         |
    | R10/Current Academic Year | May/Current Academic Year | 600          | 0          | 0         |
    | R11/Current Academic Year | Jun/Current Academic Year | 600          | 0          | 0         |
    | R12/Current Academic Year | Jul/Current Academic Year | 600          | 0          | 0         |
And only the following provider payments will be recorded
    | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
    | R03/Current Academic Year | Oct/Current Academic Year | 540                    | 60                          | Learning         |
    | R04/Current Academic Year | Nov/Current Academic Year | 540                    | 60                          | Learning         |
    | R05/Current Academic Year | Dec/Current Academic Year | 540                    | 60                          | Learning         |
    | R06/Current Academic Year | Jan/Current Academic Year | 540                    | 60                          | Learning         |
    | R07/Current Academic Year | Feb/Current Academic Year | 540                    | 60                          | Learning         |
    | R08/Current Academic Year | Mar/Current Academic Year | 540                    | 60                          | Learning         |
    | R09/Current Academic Year | Apr/Current Academic Year | 540                    | 60                          | Learning         |
    | R10/Current Academic Year | May/Current Academic Year | 540                    | 60                          | Learning         |
    | R11/Current Academic Year | Jun/Current Academic Year | 540                    | 60                          | Learning         |
    | R12/Current Academic Year | Jul/Current Academic Year | 540                    | 60                          | Learning         |
And at month end only the following provider payments will be generated
    | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
    | R03/Current Academic Year | Oct/Current Academic Year | 540                    | 60                          | Learning         |
    | R04/Current Academic Year | Nov/Current Academic Year | 540                    | 60                          | Learning         |
    | R05/Current Academic Year | Dec/Current Academic Year | 540                    | 60                          | Learning         |
    | R06/Current Academic Year | Jan/Current Academic Year | 540                    | 60                          | Learning         |
    | R07/Current Academic Year | Feb/Current Academic Year | 540                    | 60                          | Learning         |
    | R08/Current Academic Year | Mar/Current Academic Year | 540                    | 60                          | Learning         |
    | R09/Current Academic Year | Apr/Current Academic Year | 540                    | 60                          | Learning         |
    | R10/Current Academic Year | May/Current Academic Year | 540                    | 60                          | Learning         |
    | R11/Current Academic Year | Jun/Current Academic Year | 540                    | 60                          | Learning         |
    | R12/Current Academic Year | Jul/Current Academic Year | 540                    | 60                          | Learning         |
Examples: 
    | Collection_Period         |
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