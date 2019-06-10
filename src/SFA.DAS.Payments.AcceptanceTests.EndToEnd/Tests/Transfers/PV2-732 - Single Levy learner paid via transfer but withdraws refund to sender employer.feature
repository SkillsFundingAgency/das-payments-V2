Feature: Transfers - PV2-732 Single Levy learner paid via transfer but withdraws, refund to sender employer
	As a provider,
	I want a Levy learner, where the employer receives a transfer from another employer to fund the learner, and the learner then withdraws
	So that I am not paid for the learner by SFA via a transfer - PV2-732

Scenario Outline: Transfers - Single Levy learner paid via transfer but withdraws, refund to sender employer - PV2-732

	Given the "employer 1" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 1>
	And the "employer 2" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 2>
	And the remaining transfer allowance for "employer 2" is <Employer 2 Remaining Transfer Allowance>
	#And a transfer agreement has been set up between employer 1 and employer 2

	And the following apprenticeships exist 
		| Employer   | Sending Employer | Start Date                   | End Date                  | Agreed Price | Standard Code | Programme Type | Status | Effective From               |
		| employer 1 | employer 2       | 01/May/Current Academic Year | 01/May/Next Academic Year | 15000        | 50            | 25             | active | 01/May/Current Academic Year |

    And the provider previously submitted the following learner details
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 06/May/Current Academic Year | 12 months        | 12000                | 06/May/Current Academic Year        | 3000                   | 06/May/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 50            | 25             | 19-24 Apprenticeship (From May 2017) Levy Contract | 90%                         |

	And the following earnings had been generated for the learner
		| Delivery Period           | On-Programme | Completion | Balancing |
		| Aug/Current Academic Year | 0            | 0          | 0         |
		| Sep/Current Academic Year | 0            | 0          | 0         |
		| Oct/Current Academic Year | 0            | 0          | 0         |
		| Nov/Current Academic Year | 0            | 0          | 0         |
		| Dec/Current Academic Year | 0            | 0          | 0         |
		| Jan/Current Academic Year | 0            | 0          | 0         |
		| Feb/Current Academic Year | 0            | 0          | 0         |
		| Mar/Current Academic Year | 0            | 0          | 0         |
		| Apr/Current Academic Year | 0            | 0          | 0         |
		| May/Current Academic Year | 1000         | 0          | 0         |
		| Jun/Current Academic Year | 1000         | 0          | 0         |
		| Jul/Current Academic Year | 1000         | 0          | 0         |

	And the following provider payments had been generated
		| Collection Period         | Delivery Period           | Levy Payments | Transfer Payments | Transaction Type | Employer   | Sending Employer |
		| R10/Current Academic Year | May/Current Academic Year | 0             | 1000              | Learning         | employer 1 | employer 2       |
		| R11/Current Academic Year | Jun/Current Academic Year | 0             | 1000              | Learning         | employer 1 | employer 2       |
       
	But the Provider now changes the Learner details as follows
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 06/May/Current Academic Year | 12 months        | 12000                | 06/May/Current Academic Year        | 3000                   | 06/May/Current Academic Year          | 0 months        | withdrawn         | Act1          | 1                   | ZPROG001      | 50            | 25             | 19-24 Apprenticeship (From May 2017) Levy Contract | 90%                         |

	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then no learner earnings should be generated
    And at month end only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
        | R12/Current Academic Year | May/Current Academic Year | -1000        | 0          | 0         |
        | R12/Current Academic Year | Jun/Current Academic Year | -1000        | 0          | 0         |
	And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | Levy Payments | Transfer Payments | Transaction Type | Employer   | Sending Employer |
        | R12/Current Academic Year | May/Current Academic Year | 0             | -1000             | Learning         | employer 1 | employer 2       |
        | R12/Current Academic Year | Jun/Current Academic Year | 0             | -1000             | Learning         | employer 1 | employer 2       |
	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | Levy Payments | Transfer Payments | Transaction Type | Employer   | Sending Employer |
        | R12/Current Academic Year | May/Current Academic Year | 0             | -1000             | Learning         | employer 1 | employer 2       |
        | R12/Current Academic Year | Jun/Current Academic Year | 0             | -1000             | Learning         | employer 1 | employer 2       |

Examples: 
        | Collection_Period         | Levy Balance for employer 1 | Levy Balance for employer 2 | Employer 2 Remaining Transfer Allowance |
        | R12/Current Academic Year | 0                           | 60000                       | 0                                       |


#Feature: Transfers
#
#Scenario: 1 learner, paid for via transfer and withdraws in ILR â€“ refund to sender
#	
#	Given The learner is programme only DAS
#	And a transfer agreement has been set up between employer a and employer b 
#	And employer b's levy balance > agreed price for all months
#	And the apprenticeship funding band maximum is 15000
#
#
#And the following commitments exist:
#		| employer of apprentice | employer paying for training | commitment Id | version Id | ULN       | start date | end date   | standard code | agreed price | status    | effective from | effective to |
#		| employer a             | employer b                   | 1             | 1          | learner a | 01/05/2018 | 01/05/2019 | 50            | 15000        | Active    | 01/05/2018     |              |
#
#When an ILR file is submitted for period R10 with the following data:
#        | ULN       | learner type           | agreed price | start date | planned end date | actual end date | completion status | aim type         | aim sequence number | standard code | contract type | contract type date from |
#        | learner a | programme only DAS     | 15000        | 06/05/2018 | 20/05/2019       |                 | Continuing        | programme        | 1                   | 50            | DAS           | 06/05/2018              |
#
#Then the provider earnings and payments break down as follows:
#	
#        | Type                                    | 05/18  | 06/18  | 
#        | Provider Earned Total                   | 1000   | 1000   | 
#        | Provider Earned from SFA                | 1000   | 1000   | 
#        | Provider Earned from employer a         | 0      | 0      | 
#		| Provider Earned from employer b         | 0      | 0      | 
#        | Provider Paid by SFA                    | 0      | 1000   | 
#        | Refund taken by SFA                     | 0      | 0      | 
#        | Payment due from employer a             | 0      | 0      |
#		| Payment due from employer b             | 0      | 0      | 
#        | Refund due to employer a                | 0      | 0      | 
#		| Refund due to employer b                | 0      | 0      | 
#        | Levy account for employer a debited     | 0      | 0      | 
#		| Levy account for employer b debited     | 0      | 1000   | 
#        | Levy account for employer a credited    | 0      | 0      | 
#		| Levy account for employer b credited    | 0      | 0      | 
#        | SFA Levy employer budget                | 1000   | 1000   |
#        | SFA Levy co-funding budget              | 0      | 0      | 
#        | SFA Levy additional payments budget     | 0      | 0      | 
#        | SFA non-Levy co-funding budget          | 0      | 0      | 
#        | SFA non-Levy additional payments budget | 0      | 0      |
#
#When an ILR file is submitted for period R12 with the following data:
#
#        | ULN       | learner type           | agreed price | start date | planned end date | actual end date | completion status | aim type         | aim sequence number | standard code | contract type | contract type date from |
#        | learner a | programme only DAS     | 15000        | 06/05/2018 | 20/05/2019       |    06/05/2018   | withdrawn         | programme        | 1                   | 50            | DAS           | 06/05/2018              |
#
#Then the provider earnings and payments break down as follows:
#        | Type                                    | 05/18  | 06/18  | 07/18  | 08/18    |
#        | Provider Earned Total                   | 0      | 0      | 0      | 0        |
#        | Provider Earned from SFA                | 0      | 0      | 0      | 0        |
#        | Provider Earned from employer a         | 0      | 0      | 0      | 0        |
#		| Provider Earned from employer b         | 0      | 0      | 0      | 0        |
#        | Provider Paid by SFA                    | 0      | 1000   | 1000   | 0        |
#        | Refund taken by SFA                     | 0      | 0      | 0      | -2000    |
#        | Payment due from employer a             | 0      | 0      | 0      | 0        |
#		| Payment due from employer b             | 0      | 0      | 0      | 0        |
#        | Refund due to employer a                | 0      | 0      | 0      | 0        |
#		| Refund due to employer b                | 0      | 0      | 0      | -2000    |
#        | Levy account for employer a debited     | 0      | 0      | 0      | 0        |
#		| Levy account for employer b debited     | 0      | 1000   | 1000   | 0        |
#        | Levy account for employer a credited    | 0      | 0      | 0      | 0        |
#		| Levy account for employer b credited    | 0      | 0      | 0      | 2000     |
#        | SFA Levy employer budget                | 0      | 0      | 0      | 0        |
#        | SFA Levy co-funding budget              | 0      | 0      | 0      | 0        |
#        | SFA Levy additional payments budget     | 0      | 0      | 0      | 0        |
#        | SFA non-Levy co-funding budget          | 0      | 0      | 0      | 0        |
#        | SFA non-Levy additional payments budget | 0      | 0      | 0      | 0        |	
		
