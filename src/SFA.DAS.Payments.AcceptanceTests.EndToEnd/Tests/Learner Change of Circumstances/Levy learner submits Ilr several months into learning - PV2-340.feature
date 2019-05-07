@ignore
Feature: One levy learner, levy available, ILR submitted several months later but before AY end PV2-340
	As a provider,
	I want a levy learner, where ILR is submitted several months after learning has started, to be paid the correct amount
	So that I am accurately paid my apprenticeship provision. PV2-340

Scenario Outline: One levy learner, levy available, ILR submitted several months later but before AY end PV2-340
# levy balance > agreed price for all months
Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
# New Commitment line
And the following commitments exist
    | start date                | end date                     | agreed price |
    | 01/Sep/Last Academic Year | 08/Sep/Current Academic Year | 15000        |

And the provider is providing training for the following learners
    | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
    | 01/Sep/Current Academic Year | 12 months        | 12000                | 01/Sep/Current Academic Year        | 3000                   | 01/Sep/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 50            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |  
	
When the ILR file is submitted for the learners for collection period <Collection_Period>

Then the following learner earnings should be generated
    | Delivery Period           | On-Programme | Completion | Balancing |
    | Aug/Current Academic Year | 0            | 0          | 0         |
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

And at month end only the following payments will be calculated
    | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
    | R05/Current Academic Year | Sep/Current Academic Year | 1000         | 0          | 0         |
    | R05/Current Academic Year | Oct/Current Academic Year | 1000         | 0          | 0         |
    | R05/Current Academic Year | Nov/Current Academic Year | 1000         | 0          | 0         |
    | R05/Current Academic Year | Dec/Current Academic Year | 1000         | 0          | 0         |
    | R06/Current Academic Year | Jan/Current Academic Year | 1000         | 0          | 0         |

# Levy Payments
And only the following provider payments will be recorded
    | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | R05/Current Academic Year | Sep/Current Academic Year | 1000          | Learning         |
    | R05/Current Academic Year | Oct/Current Academic Year | 1000          | Learning         |
    | R05/Current Academic Year | Nov/Current Academic Year | 1000          | Learning         |
    | R05/Current Academic Year | Dec/Current Academic Year | 1000          | Learning         |
    | R06/Current Academic Year | Jan/Current Academic Year | 1000          | Learning         |

And only the following provider payments will be generated
    | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | R05/Current Academic Year | Sep/Current Academic Year | 1000          | Learning         |
    | R05/Current Academic Year | Oct/Current Academic Year | 1000          | Learning         |
    | R05/Current Academic Year | Nov/Current Academic Year | 1000          | Learning         |
    | R05/Current Academic Year | Dec/Current Academic Year | 1000          | Learning         |
    | R06/Current Academic Year | Jan/Current Academic Year | 1000          | Learning         |
# Levy Balance
Examples: 
        | Collection_Period         | Levy Balance |
        | R05/Current Academic Year | 9000         |
        | R06/Current Academic Year | 3500         |

# When a provider submits an ILR several months after learning has started (but before the academic year boundary), the earnings calculation is updated retrospectively and the provider gets paid for the preceding months.
#
#    Background:
#        Given the apprenticeship funding band maximum for each learner is 17000
#        And levy balance > agreed price for all months
#
#    Scenario: ILR submitted late for a DAS learner, levy available, learner finishes on time
#		Given the following commitments exist:
#            | ULN       | priority | start date | end date   | agreed price |
#            | learner a | 1        | 01/09/2018 | 08/09/2019 | 15000        |
#        When an ILR file is submitted for the first time on 28/12/18 with the following data:
#            | ULN       | learner type       | agreed price | start date | planned end date | completion status |
#            | learner a | programme only DAS | 15000        | 01/09/2018 | 08/09/2019       | continuing        |
#        Then the provider earnings and payments break down as follows:
#            | Type                       | 09/18 | 10/18 | 11/18 | 12/18 | 01/19 | 02/19 | ... |
#            | Provider Earned Total      | 1000  | 1000  | 1000  | 1000  | 1000  | 1000  | ... |
#            | Provider Earned from SFA   | 1000  | 1000  | 1000  | 1000  | 1000  | 1000  | ... |
#            | Provider Paid by SFA       | 0     | 0     | 0     | 0     | 4000  | 1000  | ... |
#            | Levy account debited       | 0     | 0     | 0     | 0     | 4000  | 1000  | ... |
#            | SFA Levy employer budget   | 1000  | 1000  | 1000  | 1000  | 1000  | 1000  | ... |
#            | SFA Levy co-funding budget | 0     | 0     | 0     | 0     | 0     | 0     | ... |