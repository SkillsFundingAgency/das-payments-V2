Feature: Transfers - PV2-733 Two Levy learners paid via transfer but later one deleted, sender gets refund
	As a provider,
	I want a Levy learner, where the employer receives a transfer from another employer to fund the learner, and 1 learner is then deleted
	So that I am not paid for the deleted learner by SFA via a transfer - PV2-733

Scenario Outline: Transfers - Two Levy learners paid via transfer but later one deleted, sender gets refund - PV2-733

	Given the "employer 1" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 1>
	And the "employer 2" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 2>
	And the remaining transfer allowance for "employer 2" is <Employer 2 Remaining Transfer Allowance>
	#And a transfer agreement has been set up between employer 1 and employer 2

	And the following apprenticeships exist 
		| Learner ID | Identifier      | Employer   | Sending Employer | Start Date                   | End Date                  | Agreed Price | Standard Code | Programme Type | Status | Effective From               |
		| learner a  | Apprentiiship a | employer 1 | employer 2       | 01/May/Current Academic Year | 01/May/Next Academic Year | 9000         | 50            | 25             | active | 01/May/Current Academic Year |
		| learner b  | Apprentiiship b | employer 1 | employer 2       | 01/May/Current Academic Year | 01/May/Next Academic Year | 9000         | 50            | 25             | active | 01/May/Current Academic Year |

    And the provider previously submitted the following learner details
		| Learner ID | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| learner a  | 01/May/Current Academic Year | 12 months        | 9000                 | 01/May/Current Academic Year        | 0                      | 01/May/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 50            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
		| learner b  | 01/May/Current Academic Year | 12 months        | 9000                 | 01/May/Current Academic Year        | 0                      | 01/May/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 50            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |

	And the following earnings had been generated for the learner
		| Learner ID | Delivery Period           | On-Programme | Completion | Balancing |
		| learner a  | Aug/Current Academic Year | 0            | 0          | 0         |
		| learner a  | Sep/Current Academic Year | 0            | 0          | 0         |
		| learner a  | Oct/Current Academic Year | 0            | 0          | 0         |
		| learner a  | Nov/Current Academic Year | 0            | 0          | 0         |
		| learner a  | Dec/Current Academic Year | 0            | 0          | 0         |
		| learner a  | Jan/Current Academic Year | 0            | 0          | 0         |
		| learner a  | Feb/Current Academic Year | 0            | 0          | 0         |
		| learner a  | Mar/Current Academic Year | 0            | 0          | 0         |
		| learner a  | Apr/Current Academic Year | 0            | 0          | 0         |
		| learner a  | May/Current Academic Year | 600          | 0          | 0         |
		| learner a  | Jun/Current Academic Year | 600          | 0          | 0         |
		| learner a  | Jul/Current Academic Year | 600          | 0          | 0         |
		| learner b  | Aug/Current Academic Year | 0            | 0          | 0         |
		| learner b  | Sep/Current Academic Year | 0            | 0          | 0         |
		| learner b  | Oct/Current Academic Year | 0            | 0          | 0         |
		| learner b  | Nov/Current Academic Year | 0            | 0          | 0         |
		| learner b  | Dec/Current Academic Year | 0            | 0          | 0         |
		| learner b  | Jan/Current Academic Year | 0            | 0          | 0         |
		| learner b  | Feb/Current Academic Year | 0            | 0          | 0         |
		| learner b  | Mar/Current Academic Year | 0            | 0          | 0         |
		| learner b  | Apr/Current Academic Year | 0            | 0          | 0         |
		| learner b  | May/Current Academic Year | 600          | 0          | 0         |
		| learner b  | Jun/Current Academic Year | 600          | 0          | 0         |
		| learner b  | Jul/Current Academic Year | 600          | 0          | 0         |
	And the following provider payments had been generated
		| Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transfer Payments | Transaction Type | Employer   | Sending Employer |
		| learner a  | R10/Current Academic Year | May/Current Academic Year | 0             | 600               | Learning         | employer 1 | employer 2       |
		| learner a  | R11/Current Academic Year | Jun/Current Academic Year | 0             | 600               | Learning         | employer 1 | employer 2       |
		| learner b  | R10/Current Academic Year | May/Current Academic Year | 0             | 600               | Learning         | employer 1 | employer 2       |
		| learner b  | R11/Current Academic Year | Jun/Current Academic Year | 0             | 600               | Learning         | employer 1 | employer 2       |

	But the Provider now changes the Learner details as follows
		| Learner ID | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| learner a  | 01/May/Current Academic Year | 12 months        | 9000                 | 01/May/Current Academic Year        | 0                      | 01/May/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 50            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
	And price details as follows
		| Learner ID | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Contract Type | Aim Sequence Number |
		| learner a  | pe-1             | 9000                | 01/May/Current Academic Year        | 0                      | 01/May/Current Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         | Act1          | 1                   |
		| learner b  | pe-2             | 9000                | 01/MAy/Current Academic Year        | 0                      | 01/May/Current Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         | Act1          | 1                   |
	
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Learner ID | Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| learner a  | Aug/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| learner a  | Sep/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| learner a  | Oct/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| learner a  | Nov/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| learner a  | Dec/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| learner a  | Jan/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| learner a  | Feb/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| learner a  | Mar/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| learner a  | Apr/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| learner a  | May/Current Academic Year | 600          | 0          | 0         | pe-1                     |
		| learner a  | Jun/Current Academic Year | 600          | 0          | 0         | pe-1                     |
		| learner a  | Jul/Current Academic Year | 600          | 0          | 0         | pe-1                     |
	And levy month end is ran
	And only the following provider payments will be recorded
        | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transfer Payments | Transaction Type | Employer   | Sending Employer | Standard Code |
        | learner a  | R12/Current Academic Year | Jul/Current Academic Year | 0             | 600               | Learning         | employer 1 | employer 2       | 50            |
        | learner b  | R12/Current Academic Year | May/Current Academic Year | 0             | -600              | Learning         | employer 1 | employer 2       | 50            |
        | learner b  | R12/Current Academic Year | Jun/Current Academic Year | 0             | -600              | Learning         | employer 1 | employer 2       | 50            |
	And only the following provider payments will be generated
        | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transfer Payments | Transaction Type | Employer   | Sending Employer | Standard Code |
        | learner a  | R12/Current Academic Year | Jul/Current Academic Year | 0             | 600               | Learning         | employer 1 | employer 2       | 50            |
        | learner b  | R12/Current Academic Year | May/Current Academic Year | 0             | -600              | Learning         | employer 1 | employer 2       | 50            |
        | learner b  | R12/Current Academic Year | Jun/Current Academic Year | 0             | -600              | Learning         | employer 1 | employer 2       | 50            |

Examples: 
        | Collection_Period         | Levy Balance for employer 1 | Levy Balance for employer 2 | Employer 2 Remaining Transfer Allowance |
        | R12/Current Academic Year | 0                           | 72000                       | 600                                     |

#Feature: Transfers
#
#Scenario: 2 learners, paid for via transfer and 1 deleted from ILR â€“ refund due to sending employer
#
#Given The learner is programme only DAS
#	And a transfer agreement has been set up between employer a and employer b 
#	And employer b's levy balance > agreed price for all months
#	And the apprenticeship funding band maximum is 9000
#
#
#And the following commitments exist:
#		| employer of apprentice | employer paying for training | commitment Id | version Id | ULN       | start date | end date   | standard code | agreed price | status    | effective from | effective to |
#		| employer a             | employer b                   | 1             | 1          | learner a | 01/05/2018 |01/05/2019  | 50            | 9000         | Active    | 01/05/2018     |              |
#		| employer a             | employer b                   | 2             | 1          | learner b | 01/05/2018 |01/05/2019  | 50            | 9000         | Active    | 01/05/2018     |              |
#	   		
#When an ILR file is submitted for period R10 with the following data:
#
#        | ULN       | learner type           | agreed price | start date | planned end date | actual end date | completion status | aim type         | aim sequence number | aim rate | standard code | contract type | contract type date from |
#        | learner a | programme only DAS     | 9000         | 01/05/2018 | 20/05/2019       |                 | Continuing        | programme        | 1                   |    9000  | 50            | DAS           | 06/05/2018              |
#		| learner b | programme only DAS     | 9000         | 01/05/2018 | 20/05/2019       |                 | Continuing        | programme        | 1                   |    9000  | 50            | DAS           | 06/05/2018              |
#        
#And following learning has been recorded for previous payments:
#
#            | ULN       | start date | standard code | 
#            | learner a | 01/05/2017 | 50            | 
#            | learner b | 01/05/2017 | 50            |
#
#And the following earnings and payments have been made to the provider for learner a and learner b:
#
#        | Type                                    | 05/18  | 06/18  | 07/18  | 08/18    |
#        | Provider Earned Total                   | 1200   | 1200   | 0      | 0        |
#        | Provider Earned from SFA                | 1200   | 1200   | 0      | 0        |
#        | Provider Earned from employer a         | 0      | 0      | 0      | 0        |
#		| Provider Earned from employer b         | 0      | 0      | 0      | 0        |
#        | Provider Paid by SFA                    | 0      | 1200   | 1200   | 0        |
#        | Refund taken by SFA                     | 0      | 0      | 0      | 0        |
#        | Payment due from employer a             | 0      | 0      | 0      | 0        |
#		| Payment due from employer b             | 0      | 0      | 0      | 0        |
#        | Refund due to employer a                | 0      | 0      | 0      | 0        |
#		| Refund due to employer b                | 0      | 0      | 0      | 0        |
#        | Levy account for employer a debited     | 0      | 0      | 0      | 0        |
#		| Levy account for employer b debited     | 0      | 1200   | 1200   | 0        |
#        | Levy account for employer a credited    | 0      | 0      | 0      | 0        |
#		| Levy account for employer b credited    | 0      | 0      | 0      | 0        |
#        | SFA Levy employer budget                | 1200   | 1200   | 1200   | 0        |
#        | SFA Levy co-funding budget              | 0      | 0      | 0      | 0        |
#        | SFA Levy additional payments budget     | 0      | 0      | 0      | 0        |
#        | SFA non-Levy co-funding budget          | 0      | 0      | 0      | 0        |
#        | SFA non-Levy additional payments budget | 0      | 0      | 0      | 0        |	
#		          
#          
#When an ILR file is submitted in period R12 (July) with the following data:
#		
#        | ULN       | learner type           | agreed price | start date | planned end date | actual end date | completion status | aim type         | aim sequence number | aim rate | standard code | contract type | contract type date from |
#        | learner a | programme only DAS     | 9000         | 01/05/2018 | 20/05/2019       |                 | Continuing        | programme        | 1                   |    9000  | 50            | DAS           | 06/05/2018              |
#
#		Then the provider earnings and payments for provider a break down as follows:
#	
#        | Type                                    | 05/18  | 06/18  | 07/18  | 08/18    |
#        | Provider Earned Total                   | 1200   | 1200   | 600    | 600      |
#        | Provider Earned from SFA                | 1200   | 1200   | 600    | 600      |
#        | Provider Earned from employer a         | 0      | 0      | 0      | 0        |
#		| Provider Earned from employer b         | 0      | 0      | 0      | 0        |
#        | Provider Paid by SFA                    | 0      | 1200   | 1200   | 600      |
#        | Refund taken by SFA                     | 0      | 0      | 0      | -1200    |
#        | Payment due from employer a             | 0      | 0      | 0      | 0        |
#		| Payment due from employer b             | 0      | 0      | 0      | 0        |
#        | Refund due to employer a                | 0      | 0      | 0      | 0        |
#		| Refund due to employer b                | 0      | 0      | 0      | -1200    |
#        | Levy account for employer a debited     | 0      | 0      | 0      | 0        |
#		| Levy account for employer b debited     | 0      | 1200   | 1200   | 600      |
#        | Levy account for employer a credited    | 0      | 0      | 0      | 0        |
#		| Levy account for employer b credited    | 0      | 0      | 0      | 1200     |
#        | SFA Levy employer budget                | 1200   | 1200   | 1200   | 600      |
#        | SFA Levy co-funding budget              | 0      | 0      | 0      | 0        |
#        | SFA Levy additional payments budget     | 0      | 0      | 0      | 0        |
#        | SFA non-Levy co-funding budget          | 0      | 0      | 0      | 0        |
#        | SFA non-Levy additional payments budget | 0      | 0      | 0      | 0        |			
		

