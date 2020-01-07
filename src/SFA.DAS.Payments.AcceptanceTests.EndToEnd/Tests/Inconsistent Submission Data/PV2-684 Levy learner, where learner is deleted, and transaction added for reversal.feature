Feature: Inconsistent Submissions Data PV2-684

Scenario: Levy learner a is deleted from ILR in 07/18, but Levy learner b is added to the 07/18 ILR PV2-684	
	Given the employer levy account balance in collection period R12/Current Academic Year is 9000
	And the following commitments exist
        | Identifier       | Learner ID | start date                   | end date                  | agreed price | Framework Code | Pathway Code | Programme Type |
        | Apprenticeship 1 | learner a  | 01/May/Current Academic Year | 01/May/Next Academic Year | 9000         | 593            | 1            | 20             |
	And the provider previously submitted the following learner details
		| Learner ID | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| learner a  | 06/May/Current Academic Year | 12 months        | 9000                 | 06/May/Current Academic Year        | 0                      | 06/May/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
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
    And the following provider payments had been generated
        | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | learner a  | R10/Current Academic Year | May/Current Academic Year | 600           | Learning         |
        | learner a  | R11/Current Academic Year | Jun/Current Academic Year | 600           | Learning         |
    But the Commitment details are changed as follows
        | Identifier       | Learner ID | start date                   | end date                  | agreed price | Framework Code | Pathway Code | Programme Type |
        | Apprenticeship 2 | learner b  | 01/May/Current Academic Year | 01/May/Next Academic Year | 9000         | 593            | 1            | 20             |
    And the Provider now changes the Learner details as follows
		| Learner ID | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| learner b  | 06/May/Current Academic Year | 12 months        | 9000                 | 06/May/Current Academic Year        | 0                      | 06/May/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
		| pe-1             | 9000                 | 06/May/Current Academic Year        | 0                      | 06/May/Current Academic Year          | 0                       |                                        | 0                         |                                          | Act1          | 1                   | 90%                         |

	When the amended ILR file is re-submitted for the learners in collection period R12/Current Academic Year
	Then the following learner earnings should be generated
		| Learner ID | Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| learner b  | Aug/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| learner b  | Sep/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| learner b  | Oct/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| learner b  | Nov/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| learner b  | Dec/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| learner b  | Jan/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| learner b  | Feb/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| learner b  | Mar/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| learner b  | Apr/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| learner b  | May/Current Academic Year | 600          | 0          | 0         | pe-1                     |
		| learner b  | Jun/Current Academic Year | 600          | 0          | 0         | pe-1                     |
		| learner b  | Jul/Current Academic Year | 600          | 0          | 0         | pe-1                     |
    And at month end only the following payments will be calculated
        | Learner ID | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
        | learner a  | R12/Current Academic Year | May/Current Academic Year | -600         | 0          | 0         |
        | learner a  | R12/Current Academic Year | Jun/Current Academic Year | -600         | 0          | 0         |
        | learner b  | R12/Current Academic Year | May/Current Academic Year | 600          | 0          | 0         |
        | learner b  | R12/Current Academic Year | Jun/Current Academic Year | 600          | 0          | 0         |
        | learner b  | R12/Current Academic Year | Jul/Current Academic Year | 600          | 0          | 0         |

	And only the following provider payments will be recorded
        | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | learner a  | R12/Current Academic Year | May/Current Academic Year | -600          | Learning         |
        | learner a  | R12/Current Academic Year | Jun/Current Academic Year | -600          | Learning         |
        | learner b  | R12/Current Academic Year | May/Current Academic Year | 600           | Learning         |
        | learner b  | R12/Current Academic Year | Jun/Current Academic Year | 600           | Learning         |
        | learner b  | R12/Current Academic Year | Jul/Current Academic Year | 600           | Learning         |

	And only the following provider payments will be generated
        | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | learner a  | R12/Current Academic Year | May/Current Academic Year | -600          | Learning         |
        | learner a  | R12/Current Academic Year | Jun/Current Academic Year | -600          | Learning         |
        | learner b  | R12/Current Academic Year | May/Current Academic Year | 600           | Learning         |
        | learner b  | R12/Current Academic Year | Jun/Current Academic Year | 600           | Learning         |
        | learner b  | R12/Current Academic Year | Jul/Current Academic Year | 600           | Learning         |



#Feature: Inconsistent Submissions Data
#
#Backgorund: Levy learner a is deleted from ILR in 07/18, but Levy learner b is added to the 07/18 ILR
#
#  
#Scenario: Levy learner, where learner is deleted, and transaction added for reversal
#
#		Given The learner is programme only DAS
#        
#		And levy balance > agreed price for all months
#        
#		And the apprenticeship funding band maximum is 9000
#
#        And the following commitments exist:
#			| commitment Id | version Id | ULN       | start date | end date   | framework code | programme type | pathway code | agreed price | status    | effective from | effective to |
#			| 1             | 1          | learner a | 01/05/2018 | 01/05/2019 | 403            | 2              | 1            | 9000         | Active    | 01/05/2018     |              |
#        
#        And following learning has been recorded for previous payments:
#            | ULN       | start date | aim sequence number | aim type         | aim reference | framework code | programme type | pathway code | completion status |
#            | learner a | 06/05/2018 | 1                   | programme        | ZPROG001      | 403            | 2              | 1            | continuing        |
#  
#        And the following programme earnings and payments have been made to the provider A for learner a:
#            | Type                                | 05/18 | 06/18 | 07/18 | 08/18 |
#            | Provider Earned Total               | 600   | 600   | 0     | 0     |
#            | Provider Earned from SFA            | 600   | 600   | 0     | 0     |
#            | Provider Earned from Employer       | 0     | 0     | 0     | 0     |
#            | Provider Paid by SFA                | 0     | 600   | 600   | 0     |
#            | Payment due from Employer           | 0     | 0     | 0     | 0     |
#            | Levy account debited                | 0     | 600   | 600   | 0     |
#            | SFA Levy employer budget            | 600   | 600   | 0     | 0     |
#            | SFA Levy co-funding budget          | 0     | 0     | 0     | 0     |
#            | SFA Levy additional payments budget | 0     | 0     | 0     | 0     |
#            | SFA non-Levy co-funding budget      | 0     | 0     | 0     | 0     | 
#
#		And the following payments will be added for reversal:
#			| Payment type                 | 06/18 | 07/18 |
#			| On-program                   | 600   | 600   |
#			| Completion                   | 0     | 0     |
#			| Balancing                    | 0     | 0     |
#			| Employer 16-18 incentive     | 0     | 0     |
#			| Provider 16-18 incentive     | 0     | 0     |
#			| Framework uplift on-program  | 0     | 0     |
#			| Framework uplift completion  | 0     | 0     |
#			| Framework uplift balancing   | 0     | 0     |
#			| Provider disadvantage uplift | 0     | 0     |
#		
#
#        When the following commitment exists:
#			| commitment Id | version Id | ULN       | start date | end date   | framework code | programme type | pathway code | agreed price | status    | effective from | effective to |
#			| 1             | 1          | learner b | 01/05/2018 | 01/05/2019 | 403            | 2              | 1            | 9000         | Active    | 01/05/2018     |              |
#
#		And an ILR file is submitted for learner b for the first time, but its for a different learner, on 31/07/18 with the following data:
#            | ULN       | learner type       | aim sequence number | aim type         | aim reference | aim rate | agreed price | start date | planned end date | actual end date | completion status | framework code | programme type | pathway code |
#            | learner b | programme only DAS | 1                   | programme        | ZPROG001      |          | 9000         | 06/05/2018 | 20/05/2019       |                 | continuing        | 401            | 2              | 1            |
#  
#        Then the provider earnings and payments break down as follows:
#            | Type                                    | 05/18 | 06/18 | 07/18 | 08/18 | 09/18 | 10/18 |
#            | Provider Earned Total                   | 600   | 600   | 600   | 600   | 600   | 600   |
#            | Provider Earned from SFA                | 600   | 600   | 600   | 600   | 600   | 600   |
#            | Provider Earned from Employer           | 0     | 0     | 0     | 0     | 0     | 0     |
#            | Provider Paid by SFA                    | 0     | 600   | 600   | 1800  | 600   | 600   |
#            | Refund taken by SFA                     | 0     | 0     | 0     | -1200 | 0     | 0     |
#            | Payment due from Employer               | 0     | 0     | 0     | 0     | 0     | 0     |
#            | Refund due to employer                  | 0     | 0     | 0     | 0     | 0     | 0     |
#            | Levy account debited                    | 0     | 0     | 0     | 1800  | 600   | 600   |
#            | Levy account credited                   | 0     | 0     | 0     | 1200  | 0     | 0     |
#            | SFA Levy employer budget                | 600   | 600   | 600   | 600   | 600   | 600   |
#            | SFA Levy co-funding budget              | 0     | 0     | 0     | 0     | 0     | 0     |
#            | SFA Levy additional payments budget     | 0     | 0     | 0     | 0     | 0     | 0     |
#            | SFA non-Levy co-funding budget          | 0     | 0     | 0     | 0     | 0     | 0     |
#            | SFA non-Levy additional payments budget | 0     | 0     | 0     | 0     | 0     | 0     |    

