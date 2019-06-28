Feature: Transfers - PV2-740 Single Levy learner paid partialy via transfer, own levy and co-funding
	As a provider,
	I want a Levy learner, where the employer receives a transfer from another employer to fund the learner, but not enough transfer funds are available, and receiving employer pays some with their own Levy, and some is co-funded
	So that I am accurately paid the apprenticeship amount by SFA
	Acceptance Criteria
	Agreed total price is £15000
	Funding band maximum is £15000
	Levy learner
	Transfer agreement has been set up between employer a and employer b
	Only £400 per month transfer funds available from employer b, employer has £100 per month Levy, remaining is co-funded - PV2-740

Scenario Outline: Transfers - PV2-740 - Single Levy learner paid partialy via transfer, own levy and co-funding

	Given the "employer 1" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 1>
	And the "employer 2" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 2>
	And the remaining transfer allowance for "employer 2" is <Employer 2 Remaining Transfer Allowance>
	#And a transfer agreement has been set up between employer 1 and employer 2

	And the following apprenticeships exist 
		| Employer   | Sending Employer | Start Date                   | End Date                  | Agreed Price | Standard Code | Programme Type | Status | Effective From               |
		| employer 1 | employer 2       | 01/May/Current Academic Year | 06/May/Next Academic Year | 15000        | 50            | 25             | active | 01/May/Current Academic Year |

    And the provider is providing training for the following learners
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 06/May/Current Academic Year | 12 months        | 15000                | 06/May/Current Academic Year        | 0                      | 06/May/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 50            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |

	When the ILR file is submitted for the learners for collection period <Collection_Period>
	Then the following learner earnings should be generated
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
    And at month end only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
		| R10/Current Academic Year | May/Current Academic Year | 1000         | 0          | 0         |
		| R11/Current Academic Year | Jun/Current Academic Year | 1000         | 0          | 0         |
		| R12/Current Academic Year | Jul/Current Academic Year | 1000         | 0          | 0         |
	And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | Levy Payments | Transfer Payments | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type | Employer   | Sending Employer |
        | R10/Current Academic Year | May/Current Academic Year | 0             | 400               | 0                      | 0                           | Learning         | employer 1 | employer 2       |
        | R10/Current Academic Year | May/Current Academic Year | 100           | 0                 | 450                    | 50                          | Learning         | employer 1 |                  |
        | R11/Current Academic Year | Jun/Current Academic Year | 0             | 400               | 0                      | 0                           | Learning         | employer 1 | employer 2       |
        | R11/Current Academic Year | Jun/Current Academic Year | 100           | 0                 | 450                    | 50                          | Learning         | employer 1 |                  |
        | R12/Current Academic Year | Jul/Current Academic Year | 0             | 400               | 0                      | 0                           | Learning         | employer 1 | employer 2       |
        | R12/Current Academic Year | Jul/Current Academic Year | 100           | 0                 | 450                    | 50                          | Learning         | employer 1 |                  |
	And only the following provider payments will be generated
		| Collection Period         | Delivery Period           | Levy Payments | Transfer Payments | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type | Employer   | Sending Employer |
		| R10/Current Academic Year | May/Current Academic Year | 100           | 0                 | 450                    | 50                          | Learning         | employer 1 |                  |
		| R11/Current Academic Year | Jun/Current Academic Year | 100           | 0                 | 450                    | 50                          | Learning         | employer 1 |                  |
		| R12/Current Academic Year | Jul/Current Academic Year | 100           | 0                 | 450                    | 50                          | Learning         | employer 1 |                  |
		| R10/Current Academic Year | May/Current Academic Year | 0             | 400               | 0                      | 0                           | Learning         | employer 1 | employer 2       |
		| R11/Current Academic Year | Jun/Current Academic Year | 0             | 400               | 0                      | 0                           | Learning         | employer 1 | employer 2       |
		| R12/Current Academic Year | Jul/Current Academic Year | 0             | 400               | 0                      | 0                           | Learning         | employer 1 | employer 2       |
Examples: 
        | Collection_Period         | Levy Balance for employer 1 | Levy Balance for employer 2 | Employer 2 Remaining Transfer Allowance |
        | R10/Current Academic Year | 100                         | 60000                       | 400                                     |
        | R11/Current Academic Year | 100                         | 59600                       | 400                                     |
        | R12/Current Academic Year | 100                         | 59200                       | 400                                     |


#Feature: Transfers
#
#Scenario: 1 learner, not enough transfer funds to cover the learner, receiver pays some with own Levy funds and some co-funded
#	
#    Given The learner is programme only DAS
#	And a transfer agreement has been set up between employer a and employer b 
#	And employer b's transfer allowance = 400 for all months
#	And employer a's levy balance = 100 for all months
#	And the apprenticeship funding band maximum is 15000
#	
#	And the following commitments exist:
#	
#		| employer of apprentice | employer paying for training | ULN       | start date | end date   | standard code | agreed price | status     | effective from | effective to |
#		| employer a             | employer b                   | learner a | 01/05/2018 | 06/05/2019 | 50            | 15000        | continuing | 01/05/2018     |   		      |
#	
#	When an ILR file is submitted with the following data:
#        | ULN       | learner type           | agreed price | price effective from | start date | planned end date | actual end date | completion status | aim type   | aim sequence number | standard code |
#        | learner a | programme only DAS     | 15000        | 06/05/2018           | 06/05/2018 | 20/05/2019       |    		     | continuing        | programme  | 2                   | 50            |
#
#	Then the provider earnings and payments break down as follows:
#	      	
#		| Type                                    | 05/18  | 06/18  | 07/18  | 08/18  |
#        | Provider Earned Total                   | 1000   | 1000   | 1000   | 1000   |
#        | Provider Earned from SFA                | 950    | 950    | 950    | 950    |
#        | Provider Earned from employer a         | 50     | 50     | 50     | 50     |
#		| Provider Earned from employer b         | 0      | 0      | 0      | 0      |
#        | Provider Paid by SFA                    | 0      | 950    | 950    | 950    |
#        | Refund taken by SFA                     | 0      | 0      | 0      | 0      |
#        | Payment due from employer a             | 0      | 50     | 50     | 50     |
#		| Payment due from employer b             | 0      | 0      | 0      | 0      |
#        | Refund due to employer a                | 0      | 0      | 0      | 0      |
#		| Refund due to employer b                | 0      | 0      | 0      | 0      |
#        | Levy account for employer a debited     | 0      | 100    | 100    | 100    |
#		| Levy account for employer b debited     | 0      | 400    | 400    | 400    |
#        | Levy account for employer a credited    | 0      | 0      | 0      | 0      |
#		| Levy account for employer b credited    | 0      | 0      | 0      | 0      |
#        | SFA Levy employer budget                | 500    | 500    | 500    | 500    |
#        | SFA Levy co-funding budget              | 450    | 450    | 450    | 450    |
#        | SFA Levy additional payments budget     | 0      | 0      | 0      | 0      |
#        | SFA non-Levy co-funding budget          | 0      | 0      | 0      | 0      |
#        | SFA non-Levy additional payments budget | 0      | 0      | 0      | 0      |
#
#	And the following transfers from employer b exist:
#	
#		| recipient  | 05/18  | 06/18  | 07/18  | 08/18  | 
#		| employer a | 400    | 400    | 400    | 400    |