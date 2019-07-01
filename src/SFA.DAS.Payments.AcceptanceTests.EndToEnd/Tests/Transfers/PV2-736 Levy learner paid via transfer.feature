Feature: Transfers - PV2-736 Single Levy learner paid via transfer
	As a provider,
	I want a Levy learner, where the employer receives a transfer from another employer to fund the learner
	So that I am accurately paid the apprenticeship amount by SFA via a transfer - PV2-736

Scenario Outline: Transfers - PV2-736 - Single Levy learner paid via transfer

	Given the "employer 1" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 1>
	And  the "employer 2" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 2> 
	And the remaining transfer allowance for "employer 2" is <Employer 2 Remaining Transfer Allowance>

	#And a transfer agreement has been set up between employer 1 and employer 2

	And the following apprenticeships exist
		| Employer   | Sending Employer | Start Date                   | End Date                  | Agreed price | Standard Code | Status | effective from               | programme type |
		| employer 1 | employer 2       | 01/Aug/Current Academic Year | 06/Aug/Next Academic Year | 15000        | 50            | active | 01/Aug/Current Academic Year | 25             | 

    And the provider is providing training for the following learners
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 06/Aug/Current Academic Year | 12 months        | 15000                | 06/Aug/Current Academic Year        | 0                      | 06/Aug/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 50            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |

	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
		| pe-1             | 15000                | 06/Aug/Current Academic Year        | 0                      | 06/Aug/Current Academic Year          | 0                       |                                        | 0                         |                                          | Act1          | 1                   | 90%                         |
	
	When the ILR file is submitted for the learners for collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| Aug/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Sep/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Oct/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Nov/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Dec/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Jan/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Feb/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Mar/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Apr/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| May/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Jun/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Jul/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
    And at month end only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
		| R01/Current Academic Year | Aug/Current Academic Year | 1000         | 0          | 0         |
		| R02/Current Academic Year | Sep/Current Academic Year | 1000         | 0          | 0         |
		| R03/Current Academic Year | Oct/Current Academic Year | 1000         | 0          | 0         |
	# New columns - Transfer Payments and Employer
	And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | Levy Payments | Transfer Payments | Transaction Type | Employer   | Sending Employer |
        | R01/Current Academic Year | Aug/Current Academic Year | 0             | 1000              | Learning         | employer 1 | employer 2       |
        | R02/Current Academic Year | Sep/Current Academic Year | 0             | 1000              | Learning         | employer 1 | employer 2       |
        | R03/Current Academic Year | Oct/Current Academic Year | 0             | 1000              | Learning         | employer 1 | employer 2       |
	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | Levy Payments | Transfer Payments | Transaction Type | Employer   | Sending Employer |
        | R01/Current Academic Year | Aug/Current Academic Year | 0             | 1000              | Learning         | employer 1 | employer 2       |
        | R02/Current Academic Year | Sep/Current Academic Year | 0             | 1000              | Learning         | employer 1 | employer 2       |
        | R03/Current Academic Year | Oct/Current Academic Year | 0             | 1000              | Learning         | employer 1 | employer 2       |

Examples: 
        | Collection_Period         | Levy Balance for employer 1 | Levy Balance for employer 2 | Employer 2 Remaining Transfer Allowance |
        | R01/Current Academic Year | 0                           | 50000                       | 12000                                   |
        | R02/Current Academic Year | 0                           | 49000                       | 11000                                   |
        | R03/Current Academic Year | 0                           | 48000                       | 10000                                   |

#Feature: Transfers
#
#Scenario: 1 learner, paid for via transfer, basic
#	
#    Given The learner is programme only DAS
#	And a transfer agreement has been set up between employer a and employer b 
#	And employer b's levy balance > agreed price for all months
#	And the apprenticeship funding band maximum is 15000
#	
#	And the following commitments exist:
#	
#		| employer of apprentice | employer paying for training | ULN       | start date | end date   | standard code | agreed price | status     | effective from | effective to |
#		| employer a             | employer b                   | learner a | 01/05/2018 | 06/05/2019 | 50            | 15000        | continuing | 01/05/2018     |   		      |
#	
#	When an ILR file is submitted with the following data:
#        | ULN       | learner type           | agreed price | price effective from | start date | planned end date | actual end date | completion status | aim type         | aim sequence number | standard code |
#        | learner a | programme only DAS     | 15000        | 06/05/2018           | 06/05/2018 | 20/05/2019       |    		     | continuing        | programme        | 1                   | 50            |
#
#	Then the provider earnings and payments break down as follows:
#	      	
#		| Type                                    | 05/18  | 06/18  | 07/18  | 08/18   |
#        | Provider Earned Total                   | 1000   | 1000   | 1000   | 1000    |
#        | Provider Earned from SFA                | 1000   | 1000   | 1000   | 1000    |
#        | Provider Earned from employer a         | 0      | 0      | 0      | 0       |
#		| Provider Earned from employer b         | 0      | 0      | 0      | 0       |
#        | Provider Paid by SFA                    | 0      | 1000   | 1000   | 1000    |
#        | Refund taken by SFA                     | 0      | 0      | 0      | 0       |
#        | Payment due from employer a             | 0      | 0      | 0      | 0       |
#		| Payment due from employer b             | 0      | 0      | 0      | 0       |
#        | Refund due to employer a                | 0      | 0      | 0      | 0       |
#		| Refund due to employer b                | 0      | 0      | 0      | 0       |
#        | Levy account for employer a debited     | 0      | 0      | 0      | 0       |
#		| Levy account for employer b debited     | 0      | 1000   | 1000   | 1000    |
#        | Levy account for employer a credited    | 0      | 0      | 0      | 0       |
#		| Levy account for employer b credited    | 0      | 0      | 0      | 0       |
#        | SFA Levy employer budget                | 1000   | 1000   | 1000   | 1000    |
#        | SFA Levy co-funding budget              | 0      | 0      | 0      | 0       |
#        | SFA Levy additional payments budget     | 0      | 0      | 0      | 0       |
#        | SFA non-Levy co-funding budget          | 0      | 0      | 0      | 0       |
#        | SFA non-Levy additional payments budget | 0      | 0      | 0      | 0       |
		

