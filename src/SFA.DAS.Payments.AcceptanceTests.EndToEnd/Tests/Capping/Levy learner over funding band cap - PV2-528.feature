#@supports_dc_e2e
Feature: Levy learner over funding band cap - PV2-528
	As a provider,
	I want a levy learner, where the negotiated total price for the learner is more than the maximum allowed for that funding band
	So that I am only paid up to the maximum cap within that funding band by SFA

#Scenario: Payment for a DAS learner with a negotiated price above the funding band cap
#
#    Given levy balance > agreed price for all months
#    And the apprenticeship funding band maximum is 15000
#    And the following commitments exist:
#	
#        | commitment Id | version Id | Provider   | ULN       | start date | end date   | agreed price | standard code | status | effective from |
#        | 1             | 1          | provider a | learner a | 01/08/2018 | 01/08/2019 | 18000        | 50            | active | 01/08/2018     |
#		
#    When an ILR file is submitted with the following data:
#        | Provider   | ULN       | learner type       | agreed price | start date | planned end date | actual end date | completion status | standard code |
#        | provider a | learner a | programme only DAS | 18000        | 06/08/2018 | 08/08/2019       |                 | continuing        | 50            |
#		
#    Then the following capping will apply to the price episodes:
#        | Provider   | price episode | negotiated price | funding cap | previous funding paid | price above cap | effective price for SFA payments |
#        | provider a | 08/18 onwards | 18000            | 15000       | 0                     | 3000            | 15000                            |		
#    And the provider earnings and payments break down as follows:
#        | Type                                | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 |
#        | Provider Earned Total               | 1000  | 1000  | 1000  | 1000  | 1000  |
#        | Provider Earned from SFA            | 1000  | 1000  | 1000  | 1000  | 1000  |
#        | Provider Earned from Employer       | 0     | 0     | 0     | 0     | 0     |
#        | Provider Paid by SFA                | 0     | 1000  | 1000  | 1000  | 1000  |
#        | Payment due from Employer           | 0     | 0     | 0     | 0     | 0     |
#        | Levy account debited                | 0     | 1000  | 1000  | 1000  | 1000  |
#        | SFA Levy employer budget            | 1000  | 1000  | 1000  | 1000  | 1000  |
#        | SFA Levy co-funding budget          | 0     | 0     | 0     | 0     | 0     |
#        | SFA Levy additional payments budget | 0     | 0     | 0     | 0     | 0     |
#        | SFA non-Levy co-funding budget      | 0     | 0     | 0     | 0     | 0     |		
#    And the transaction types for the payments are:
#        | Payment type                 | 09/18 | 10/18 | 11/18 | 12/18 |
#        | On-program                   | 1000  | 1000  | 1000  | 1000  |
#        | Completion                   | 0     | 0     | 0     | 0     |
#        | Balancing                    | 0     | 0     | 0     | 0     |
#        | Employer 16-18 incentive     | 0     | 0     | 0     | 0     |
#        | Provider 16-18 incentive     | 0     | 0     | 0     | 0     |
#        | Provider disadvantage uplift | 0     | 0     | 0     | 0     |

Scenario Outline: Capping - Payment for Levy learner with a negotiated price above funding cap PV2-528	
	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
	And the following commitments exist
		| version Id | start date                   | end date                  | agreed price | standard code | status | effective from               | Programme Type |
		| 1          | 01/Aug/Current Academic Year | 01/Aug/Next Academic Year | 18000        | 50            | active | 01/Aug/Current Academic Year | 25             |
    And the provider is providing training for the following learners
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 06/Aug/Current Academic Year | 12 months        | 18000                | 06/Aug/Current Academic Year        | 0                      | 06/Aug/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 50            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
	# New capping section
	And the following capping will apply to the price episodes
        | negotiated price | funding cap | previous funding paid | price above cap | effective price for SFA payments |
        | 18000            | 15000       | 0                     | 3000            | 15000                            |
	When the ILR file is submitted for the learners for collection period <Collection_Period>
	Then the following learner earnings should be generated
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
    And at month end only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
		| R01/Current Academic Year | Aug/Current Academic Year | 1000         | 0          | 0         |
		| R02/Current Academic Year | Sep/Current Academic Year | 1000         | 0          | 0         |
		| R03/Current Academic Year | Oct/Current Academic Year | 1000         | 0          | 0         |
        | R04/Current Academic Year | Nov/Current Academic Year | 1000         | 0          | 0         |
        | R05/Current Academic Year | Dec/Current Academic Year | 1000         | 0          | 0         |
		| R06/Current Academic Year | Jan/Current Academic Year | 1000         | 0          | 0         |
		| R07/Current Academic Year | Feb/Current Academic Year | 1000         | 0          | 0         |
		| R08/Current Academic Year | Mar/Current Academic Year | 1000         | 0          | 0         |
		| R09/Current Academic Year | Apr/Current Academic Year | 1000         | 0          | 0         |
		| R10/Current Academic Year | May/Current Academic Year | 1000         | 0          | 0         |
		| R11/Current Academic Year | Jun/Current Academic Year | 1000         | 0          | 0         |
		| R12/Current Academic Year | Jul/Current Academic Year | 1000         | 0          | 0         |
	And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 1000          | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 1000          | Learning         |
        | R03/Current Academic Year | Oct/Current Academic Year | 1000          | Learning         |
        | R04/Current Academic Year | Nov/Current Academic Year | 1000          | Learning         |
        | R05/Current Academic Year | Dec/Current Academic Year | 1000          | Learning         |
        | R06/Current Academic Year | Jan/Current Academic Year | 1000          | Learning         |
        | R07/Current Academic Year | Feb/Current Academic Year | 1000          | Learning         |
        | R08/Current Academic Year | Mar/Current Academic Year | 1000          | Learning         |
        | R09/Current Academic Year | Apr/Current Academic Year | 1000          | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 1000          | Learning         |
        | R11/Current Academic Year | Jun/Current Academic Year | 1000          | Learning         |
        | R12/Current Academic Year | Jul/Current Academic Year | 1000          | Learning         |
	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 1000          | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 1000          | Learning         |
        | R03/Current Academic Year | Oct/Current Academic Year | 1000          | Learning         |
        | R04/Current Academic Year | Nov/Current Academic Year | 1000          | Learning         |
        | R05/Current Academic Year | Dec/Current Academic Year | 1000          | Learning         |
        | R06/Current Academic Year | Jan/Current Academic Year | 1000          | Learning         |
        | R07/Current Academic Year | Feb/Current Academic Year | 1000          | Learning         |
        | R08/Current Academic Year | Mar/Current Academic Year | 1000          | Learning         |
        | R09/Current Academic Year | Apr/Current Academic Year | 1000          | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 1000          | Learning         |
        | R11/Current Academic Year | Jun/Current Academic Year | 1000          | Learning         |
        | R12/Current Academic Year | Jul/Current Academic Year | 1000          | Learning         |

Examples: 
        | Collection_Period         | Levy Balance |
        | R01/Current Academic Year | 18500        |
        | R02/Current Academic Year | 17500        |
        | R03/Current Academic Year | 16500        |
        | R04/Current Academic Year | 15500        |
        | R05/Current Academic Year | 14500        |
        | R06/Current Academic Year | 13500        |
        | R07/Current Academic Year | 12500        |
        | R08/Current Academic Year | 11500        |
        | R09/Current Academic Year | 10500        |
        | R10/Current Academic Year | 9500         |
        | R11/Current Academic Year | 8500         |
        | R12/Current Academic Year | 7500         |
