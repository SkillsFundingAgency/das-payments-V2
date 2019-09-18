﻿@basic_refund
Feature: Levy learner changes start date forwards in month - PV2-280
	As a provider,
	I want a levy learner, where change to start date within calendar month, forward in month is paid the correct amount
	So that I am accurately paid my apprenticeship provision.

#Scenario: DAS Learner - Change to start date within calendar month, forward in month
#    Given The learner is programme only DAS
#       
#	And levy balance > agreed price for all months
#    And the apprenticeship funding band maximum is 9000
#
#	And the following commitments exist:
#		| commitment Id | version Id | ULN       | start date | end date   | framework code | programme type | pathway code | agreed price | status    | effective from | effective to |
#		| 1             | 1          | learner a | 01/08/2017 | 01/08/2018 | 403            | 2              | 1            | 9000         | Active    | 01/08/2017     |              |
#        
#	When an ILR file is submitted for period R01 with the following data:
#        | ULN       | learner type       | agreed price | start date | planned end date | actual end date | completion status | aim type  | aim sequence number | framework code | programme type | pathway code |
#        | learner a | programme only DAS | 9000         | 05/08/2017 | 20/08/2018       |                 | continuing        | programme | 1                   | 403            | 2              | 1            |
#
#	And an ILR file is submitted for period R03 with the following data:
#         | ULN       | learner type       | agreed price | start date | planned end date | actual end date | completion status | aim type  | aim sequence number | framework code | programme type | pathway code |
#         | learner a | programme only DAS | 9000         | 15/08/2017 | 20/08/2018       |                 | continuing        | programme | 1                   | 403            | 2              | 1            |
#
#	Then the provider earnings and payments break down as follows:
#		| Type                                | 08/17 | 09/17 | 10/17 | 11/17 |
#        | Provider Earned Total               | 600   | 600   | 600   | 600   |
#        | Provider Earned from SFA            | 0     | 600   | 600   | 600   |
#        | Provider Earned from Employer       | 0     | 0     | 0     | 0     |
#        | Provider Paid by SFA                | 0     | 600   | 600   | 600   |
#        | Payment due from Employer           | 0     | 0     | 0     | 0     |
#        | Levy account debited                | 0     | 600   | 600   | 600   |
#        | SFA Levy employer budget            | 600   | 600   | 600   | 600   |
#        | SFA Levy co-funding budget          | 0     | 0     | 0     | 0     |
#        | SFA Levy additional payments budget | 0     | 0     | 0     | 0     |
#        | SFA non-Levy co-funding budget      | 0     | 0     | 0     | 0     | 

    #Feature: Levy learner - Change to start date within calendar month, forward in month
	
Scenario:  For a DAS-Learner, the start date of apprenticeship is moved forward within a calendar month PV2-280

Given the employer levy account balance in collection period R06/Current Academic Year is 15000

And the following commitments exist
	| start date                   | end date                     | agreed price | framework code | programme type | pathway code |
	| 01/Aug/Current Academic Year | 31/Jul/Current Academic Year | 9000         | 403            | 2              | 1            |

And the provider previously submitted the following learner details
    | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Completion Status | SFA Contribution Percentage | Contract Type | Aim Sequence Number | Aim Reference | framework code | programme type | pathway code | Funding Line Type                                  |
    | 05/Aug/Current Academic Year | 12 months        | 9000                 | Aug/Current Academic Year           | continuing        | 90%                         | Act1          | 1                   | ZPROG001      | 403            | 2              | 1            | 16-18 Apprenticeship (From May 2017) Levy Contract |
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
    | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | R01/Current Academic Year | Aug/Current Academic Year | 600           | Learning         |
    | R02/Current Academic Year | Sep/Current Academic Year | 600           | Learning         |
    | R03/Current Academic Year | Oct/Current Academic Year | 600           | Learning         |
    | R04/Current Academic Year | Nov/Current Academic Year | 600           | Learning         |
    | R05/Current Academic Year | Dec/Current Academic Year | 600           | Learning         |
        
But the Provider now changes the Learner details as follows
    | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Completion Status | SFA Contribution Percentage | Contract Type | Aim Sequence Number | Aim Reference | framework code | programme type | pathway code | Funding Line Type                                  |
    | 15/Aug/Current Academic Year | 12 months        | 9000                 | Aug/Current Academic Year           | continuing        | 90%                         | Act1          | 1                   | ZPROG001      | 403            | 2              | 1            | 16-18 Apprenticeship (From May 2017) Levy Contract |

And price details as follows
	| Price Episode Id | Total Training Price | Total Training Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
	| pe-1             | 9000                 | 15/Aug/Current Academic Year        | Act1          | 1                   | 90%                         |
		 
When the amended ILR file is re-submitted for the learners in collection period R06/Current Academic Year

Then the following learner earnings should be generated
    | Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
    | Aug/Current Academic Year | 600          | 0          | 0         | pe-1                     |
    | Sep/Current Academic Year | 600          | 0          | 0         | pe-1                     |
    | Oct/Current Academic Year | 600          | 0          | 0         | pe-1                     |
    | Nov/Current Academic Year | 600          | 0          | 0         | pe-1                     |
    | Dec/Current Academic Year | 600          | 0          | 0         | pe-1                     |
    | Jan/Current Academic Year | 600          | 0          | 0         | pe-1                     |
    | Feb/Current Academic Year | 600          | 0          | 0         | pe-1                     |
    | Mar/Current Academic Year | 600          | 0          | 0         | pe-1                     |
    | Apr/Current Academic Year | 600          | 0          | 0         | pe-1                     |
    | May/Current Academic Year | 600          | 0          | 0         | pe-1                     |
    | Jun/Current Academic Year | 600          | 0          | 0         | pe-1                     |
    | Jul/Current Academic Year | 600          | 0          | 0         | pe-1                     |

And at month end only the following payments will be calculated
    | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
    | R06/Current Academic Year | Jan/Current Academic Year | 600          | 0          | 0         |

And only the following provider payments will be recorded
    | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | R06/Current Academic Year | Jan/Current Academic Year | 600           | Learning         |
		
And only the following provider payments will be generated
    | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | R06/Current Academic Year | Jan/Current Academic Year | 600           | Learning         |

