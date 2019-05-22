#Feature: The ILR is submitted late (after the academic year boundary)
#
#Scenario: ILR submitted late for a DAS learner, levy available
#		
#Given the apprenticeship funding band maximum for each learner is 17000
#And levy balance > agreed price for all months
#When a provider submits an ILR several months after learning has started (but before the academic year boundary), 
#the earnings calculation is updated retrospectively and the provider gets paid for the preceding months.
#	
#
#		Given the following commitments exist:
#		
#            | ULN       | priority | start date | end date   | agreed price |
#            | learner a | 1        | 01/09/2018 | 08/09/2019 | 15000        |
#        
#		When an ILR file is submitted for the first time on 28/0 with the following data:
#		
#            | ULN       | learner type       | agreed price | start date | planned end date | completion status |
#            | learner a | programme only DAS | 15000        | 01/09/2018 | 08/09/2019       | continuing        |
#       
#	   Then the provider earnings and payments break down as follows:
#	   
#            | Type                       | 04/19 | 05/19 | 06/19 | 07/19 | 08/19 | 09/19 | ... |
#            | Provider Earned Total      | 1000  | 1000  | 1000  | 1000  | 1000  | 0     | ... |
#            | Provider Earned from SFA   | 1000  | 1000  | 1000  | 1000  | 1000  | 0     | ... |
#            | Provider Paid by SFA       | 0     | 0     | 0     | 0     | 11000 | 1000  | ... |
#            | Levy account debited       | 0     | 0     | 0     | 0     | 11000 | 1000  | ... |
#            | SFA Levy employer budget   | 1000  | 1000  | 1000  | 1000  | 1000  | 1000  | ... |
#            | SFA Levy co-funding budget | 0     | 0     | 0     | 0     | 0     | 0     | ... |	

Feature: ILR submitted for the first time for Levy Learner in R13/R14
	As a provider,
	I want to ensure that when a levy learner is submitted for the first time in R13/R14 that payments are calculated correctly.
	So that I am accurately paid my apprenticeship provision.PV2-428

Scenario: One levy learner, levy available, ILR submitted for the first time in R13/R14 PV2-428
# levy balance > agreed price for all months

Given the employer levy account balance in collection period R13/Current Academic Year is 15000
# New Commitment line
And the following commitments exist
    | start date                   | end date                  | agreed price | Standard Code | Programme Type | 
    | 01/Sep/Current Academic Year | 08/Sep/Next Academic Year | 15000        | 50            | 25             | 

And the provider is providing training for the following learners
    | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
    | 01/Sep/Current Academic Year | 12 months        | 12000                | 01/Sep/Current Academic Year        | 3000                   | 01/Sep/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 50            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         | 
	
When the ILR file is submitted for the learners for collection period R13/Current Academic Year 

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
    | R13/Current Academic Year | Sep/Current Academic Year | 1000         | 0          | 0         |
    | R13/Current Academic Year | Oct/Current Academic Year | 1000         | 0          | 0         |
    | R13/Current Academic Year | Nov/Current Academic Year | 1000         | 0          | 0         |
    | R13/Current Academic Year | Dec/Current Academic Year | 1000         | 0          | 0         |
    | R13/Current Academic Year | Jan/Current Academic Year | 1000         | 0          | 0         |
    | R13/Current Academic Year | Feb/Current Academic Year | 1000         | 0          | 0         |
    | R13/Current Academic Year | Mar/Current Academic Year | 1000         | 0          | 0         |
    | R13/Current Academic Year | Apr/Current Academic Year | 1000         | 0          | 0         |
    | R13/Current Academic Year | May/Current Academic Year | 1000         | 0          | 0         |
    | R13/Current Academic Year | Jun/Current Academic Year | 1000         | 0          | 0         |
    | R13/Current Academic Year | Jul/Current Academic Year | 1000         | 0          | 0         |
# Levy Payments
And only the following provider payments will be recorded
    | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | R13/Current Academic Year | Sep/Current Academic Year | 1000          | Learning         |
    | R13/Current Academic Year | Oct/Current Academic Year | 1000          | Learning         |
    | R13/Current Academic Year | Nov/Current Academic Year | 1000          | Learning         |
    | R13/Current Academic Year | Dec/Current Academic Year | 1000          | Learning         |
    | R13/Current Academic Year | Jan/Current Academic Year | 1000          | Learning         |
    | R13/Current Academic Year | Feb/Current Academic Year | 1000          | Learning         |
    | R13/Current Academic Year | Mar/Current Academic Year | 1000          | Learning         |
    | R13/Current Academic Year | Apr/Current Academic Year | 1000          | Learning         |
    | R13/Current Academic Year | May/Current Academic Year | 1000          | Learning         |
    | R13/Current Academic Year | Jun/Current Academic Year | 1000          | Learning         |
    | R13/Current Academic Year | Jul/Current Academic Year | 1000          | Learning         |

And only the following provider payments will be generated
    | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | R13/Current Academic Year | Sep/Current Academic Year | 1000          | Learning         |
    | R13/Current Academic Year | Oct/Current Academic Year | 1000          | Learning         |
    | R13/Current Academic Year | Nov/Current Academic Year | 1000          | Learning         |
    | R13/Current Academic Year | Dec/Current Academic Year | 1000          | Learning         |
    | R13/Current Academic Year | Jan/Current Academic Year | 1000          | Learning         |
    | R13/Current Academic Year | Feb/Current Academic Year | 1000          | Learning         |
    | R13/Current Academic Year | Mar/Current Academic Year | 1000          | Learning         |
    | R13/Current Academic Year | Apr/Current Academic Year | 1000          | Learning         |
    | R13/Current Academic Year | May/Current Academic Year | 1000          | Learning         |
    | R13/Current Academic Year | Jun/Current Academic Year | 1000          | Learning         |
    | R13/Current Academic Year | Jul/Current Academic Year | 1000          | Learning         |