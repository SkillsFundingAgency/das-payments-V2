@ignore
Feature:Two levy learners, full levy available for one learner, partial levy available for the other PV2-604
As a provider,
I want 2 Levy learners, where levy is spent in priority order and there is only enough levy available for one and half learners
So that I am accurately paid the apprenticeship amount by SFA  PV2-604

Scenario Outline: Two levy learners, full levy available for one learner, partial levy available for the other PV2-604
Given the employer levy account balance in collection period  <collection_Period> is <Levy Balance>
And the following commitments exist
	| Identifier         | Learner ID | priority | start date                   | end date                  | agreed price |
	| Apprenticeship 1    | learner a  | 1        | 01/Aug/Current Academic Year | 08/Aug/Next Academic Year | 15000        |
	| Apprenticeship 2    | learner b  | 2        | 01/Aug/Current Academic Year | 08/Aug/Next Academic Year | 15000        |
And the provider is providing training for the following learners
	| Learner ID | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                               | SFA Contribution Percentage |
	| learner a  | 01/Aug/Current Academic Year | 12 months        | 15000                | 01/Aug/Current Academic Year        | 0                      | 01/Aug/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship Non-Levy Contract (procured) | 90%                         |
	| learner b  | 01/Aug/Current Academic Year | 12 months        | 15000                | 01/Aug/Current Academic Year        | 0                      | 01/Aug/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship Non-Levy Contract (procured) | 90%                         |
When the ILR file is submitted for the learners for collection period <collection_Period>
Then the following learner earnings should be generated
	| Learner ID | Delivery Period           | On-Programme | Completion | Balancing |
	| learner a  | Aug/Current Academic Year | 1000         | 0          | 0         |
	| learner a  | Sep/Current Academic Year | 1000         | 0          | 0         |
	| learner a  | Oct/Current Academic Year | 1000         | 0          | 0         |
	| learner a  | Nov/Current Academic Year | 1000         | 0          | 0         |
	| learner a  | Dec/Current Academic Year | 1000         | 0          | 0         |
	| learner a  | Jan/Current Academic Year | 1000         | 0          | 0         |
	| learner a  | Feb/Current Academic Year | 1000         | 0          | 0         |
	| learner a  | Mar/Current Academic Year | 1000         | 0          | 0         |
	| learner a  | Apr/Current Academic Year | 1000         | 0          | 0         |
	| learner a  | May/Current Academic Year | 1000         | 0          | 0         |
	| learner a  | Jun/Current Academic Year | 1000         | 0          | 0         |
	| learner a  | Jul/Current Academic Year | 1000         | 0          | 0         |
	| learner b  | Aug/Current Academic Year | 1000         | 0          | 0         |
	| learner b  | Sep/Current Academic Year | 1000         | 0          | 0         |
	| learner b  | Oct/Current Academic Year | 1000         | 0          | 0         |
	| learner b  | Nov/Current Academic Year | 1000         | 0          | 0         |
	| learner b  | Dec/Current Academic Year | 1000         | 0          | 0         |
	| learner b  | Jan/Current Academic Year | 1000         | 0          | 0         |
	| learner b  | Feb/Current Academic Year | 1000         | 0          | 0         |
	| learner b  | Mar/Current Academic Year | 1000         | 0          | 0         |
	| learner b  | Apr/Current Academic Year | 1000         | 0          | 0         |
	| learner b  | May/Current Academic Year | 1000         | 0          | 0         |
	| learner b  | Jun/Current Academic Year | 1000         | 0          | 0         |
	| learner b  | Jul/Current Academic Year | 1000         | 0          | 0         |
And at month end only the following payments will be calculated
    | Learner ID | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
    | learner a  | R01/Current Academic Year | Aug/Current Academic Year | 1000         | 0          | 0         |
    | learner a  | R02/Current Academic Year | Sep/Current Academic Year | 1000         | 0          | 0         |
    | learner a  | R03/Current Academic Year | Oct/Current Academic Year | 1000         | 0          | 0         |
    | learner a  | R04/Current Academic Year | Nov/Current Academic Year | 1000         | 0          | 0         |
    | learner a  | R05/Current Academic Year | Dec/Current Academic Year | 1000         | 0          | 0         |
    | learner a  | R06/Current Academic Year | Jan/Current Academic Year | 1000         | 0          | 0         |
    | learner a  | R07/Current Academic Year | Feb/Current Academic Year | 1000         | 0          | 0         |
    | learner a  | R08/Current Academic Year | Mar/Current Academic Year | 1000         | 0          | 0         |
    | learner a  | R09/Current Academic Year | Apr/Current Academic Year | 1000         | 0          | 0         |
    | learner a  | R10/Current Academic Year | May/Current Academic Year | 1000         | 0          | 0         |
    | learner a  | R11/Current Academic Year | Jun/Current Academic Year | 1000         | 0          | 0         |
    | learner a  | R12/Current Academic Year | Jul/Current Academic Year | 1000         | 0          | 0         |
    | learner b  | R01/Current Academic Year | Aug/Current Academic Year | 1000         | 0          | 0         |
    | learner b  | R02/Current Academic Year | Sep/Current Academic Year | 1000         | 0          | 0         |
    | learner b  | R03/Current Academic Year | Oct/Current Academic Year | 1000         | 0          | 0         |
    | learner b  | R04/Current Academic Year | Nov/Current Academic Year | 1000         | 0          | 0         |
    | learner b  | R05/Current Academic Year | Dec/Current Academic Year | 1000         | 0          | 0         |
    | learner b  | R06/Current Academic Year | Jan/Current Academic Year | 1000         | 0          | 0         |
    | learner b  | R07/Current Academic Year | Feb/Current Academic Year | 1000         | 0          | 0         |
    | learner b  | R08/Current Academic Year | Mar/Current Academic Year | 1000         | 0          | 0         |
    | learner b  | R09/Current Academic Year | Apr/Current Academic Year | 1000         | 0          | 0         |
    | learner b  | R10/Current Academic Year | May/Current Academic Year | 1000         | 0          | 0         |
    | learner b  | R11/Current Academic Year | Jun/Current Academic Year | 1000         | 0          | 0         |
    | learner b  | R12/Current Academic Year | Jul/Current Academic Year | 1000         | 0          | 0         |
And only the following provider payments will be recorded
    | Learner ID | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
    | learner a  | R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
    | learner a  | R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
    | learner a  | R03/Current Academic Year | Oct/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
    | learner a  | R04/Current Academic Year | Nov/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
    | learner a  | R05/Current Academic Year | Dec/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
    | learner a  | R06/Current Academic Year | Jan/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
    | learner a  | R07/Current Academic Year | Feb/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
    | learner a  | R08/Current Academic Year | Mar/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
    | learner a  | R09/Current Academic Year | Apr/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
    | learner a  | R10/Current Academic Year | May/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
    | learner a  | R11/Current Academic Year | Jun/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
    | learner a  | R12/Current Academic Year | Jul/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
	| learner b  | R01/Current Academic Year | Aug/Current Academic Year | 450                    | 50                          | 500           | Learning         |
	| learner b  | R02/Current Academic Year | Sep/Current Academic Year | 450                    | 50                          | 500           | Learning         |
	| learner b  | R03/Current Academic Year | Oct/Current Academic Year | 450                    | 50                          | 500           | Learning         |
	| learner b  | R04/Current Academic Year | Nov/Current Academic Year | 450                    | 50                          | 500           | Learning         |
	| learner b  | R05/Current Academic Year | Dec/Current Academic Year | 450                    | 50                          | 500           | Learning         |
	| learner b  | R06/Current Academic Year | Jan/Current Academic Year | 450                    | 50                          | 500           | Learning         |
	| learner b  | R07/Current Academic Year | Feb/Current Academic Year | 450                    | 50                          | 500           | Learning         |
	| learner b  | R08/Current Academic Year | Mar/Current Academic Year | 450                    | 50                          | 500           | Learning         |
	| learner b  | R09/Current Academic Year | Apr/Current Academic Year | 450                    | 50                          | 500           | Learning         |
	| learner b  | R10/Current Academic Year | May/Current Academic Year | 450                    | 50                          | 500           | Learning         |
	| learner b  | R11/Current Academic Year | Jun/Current Academic Year | 450                    | 50                          | 500           | Learning         |
	| learner b  | R12/Current Academic Year | Jul/Current Academic Year | 450                    | 50                          | 500           | Learning         |
And only the following provider payments will be generated
    | Learner ID | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
    | learner a  | R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
    | learner a  | R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 1000          | Learning         | 
	| learner a  | R03/Current Academic Year | Oct/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
	| learner a  | R04/Current Academic Year | Nov/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
	| learner a  | R05/Current Academic Year | Dec/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
	| learner a  | R06/Current Academic Year | Jan/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
	| learner a  | R07/Current Academic Year | Feb/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
	| learner a  | R08/Current Academic Year | Mar/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
	| learner a  | R09/Current Academic Year | Apr/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
	| learner a  | R10/Current Academic Year | May/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
	| learner a  | R11/Current Academic Year | Jun/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
	| learner a  | R12/Current Academic Year | Jul/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
	| learner b  | R01/Current Academic Year | Aug/Current Academic Year | 450                    | 50                          | 500           | Learning         |
	| learner b  | R02/Current Academic Year | Sep/Current Academic Year | 450                    | 50                          | 500           | Learning         |
	| learner b  | R03/Current Academic Year | Oct/Current Academic Year | 450                    | 50                          | 500           | Learning         |
	| learner b  | R04/Current Academic Year | Nov/Current Academic Year | 450                    | 50                          | 500           | Learning         |
	| learner b  | R05/Current Academic Year | Dec/Current Academic Year | 450                    | 50                          | 500           | Learning         |
	| learner b  | R06/Current Academic Year | Jan/Current Academic Year | 450                    | 50                          | 500           | Learning         |
	| learner b  | R07/Current Academic Year | Feb/Current Academic Year | 450                    | 50                          | 500           | Learning         |
	| learner b  | R08/Current Academic Year | Mar/Current Academic Year | 450                    | 50                          | 500           | Learning         |
	| learner b  | R09/Current Academic Year | Apr/Current Academic Year | 450                    | 50                          | 500           | Learning         |
	| learner b  | R10/Current Academic Year | May/Current Academic Year | 450                    | 50                          | 500           | Learning         |
	| learner b  | R11/Current Academic Year | Jun/Current Academic Year | 450                    | 50                          | 500           | Learning         |
	| learner b  | R12/Current Academic Year | Jul/Current Academic Year | 450                    | 50                          | 500           | Learning         |

Examples: 
    | Collection_Period         | Levy Balance |
    | R01/Current Academic Year | 1500         |
    | R02/Current Academic Year | 1500         |
    | R03/Current Academic Year | 1500         |
    | R04/Current Academic Year | 1500         |
    | R05/Current Academic Year | 1500         |
    | R06/Current Academic Year | 1500         |
    | R07/Current Academic Year | 1500         |
    | R08/Current Academic Year | 1500         |
    | R09/Current Academic Year | 1500         |
    | R10/Current Academic Year | 1500         |
    | R11/Current Academic Year | 1500         |
    | R12/Current Academic Year | 1500         |

#Feature: Payment Priority
#
#Background: 2 learners, paid in priority order
#
#Scenario: Earnings and payments for two Levy learners, levy is spent in priority order and there is enough levy to fund one and a half learners
#
#        Given Two learners are programme only DAS 
#		And the apprenticeship funding band maximum for each learner is 17000
#        
#		And the employer's levy balance is:
#                | 09/18 | 10/18 | 11/18 | 12/18 | ...  | 09/19 | 10/19 |
#                | 1500  | 1500  | 1500  | 1500  | 1500 | 1500  | 1500  |
#        
#		And the following commitments exist on 03/12/2018:
#                | priority | ULN | start date | end date   | agreed price |
#                | 1        | 123 | 01/08/2018 | 28/08/2019 | 15000        |
#                | 2        | 456 | 01/08/2018 | 28/08/2019 | 15000        |
#        
#		When an ILR file is submitted on 03/12/2018 with the following data:
#                | ULN | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date |
#                | 123 | 01/08/2018 | 28/08/2019       |                 | continuing        | 12000                | 01/08/2018                          | 3000                   | 01/08/2018                            |
#                | 456 | 01/08/2018 | 28/08/2019       |                 | continuing        | 12000                | 01/08/2018                          | 3000                   | 01/08/2018                            |
#        
#		Then the provider earnings and payments break down for ULN 123 as follows:
#                | Type                           | 08/18 | 09/18 | 10/18 | 11/18 | ... | 07/19 | 08/19 |
#                | Provider Earned Total          | 1000  | 1000  | 1000  | 1000  | ... | 1000  | 0     |
#                | Provider Earned from SFA       | 1000  | 1000  | 1000  | 1000  | ... | 1000  | 0     |
#                | Provider Earned from Employer  | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#                | Provider Paid by SFA           | 0     | 1000  | 1000  | 1000  | ... | 1000  | 1000  |
#                | Payment due from Employer      | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#                | Levy account debited           | 0     | 1000  | 1000  | 1000  | ... | 1000  | 1000  |
#                | SFA Levy employer budget       | 1000  | 1000  | 1000  | 1000  | ... | 1000  | 0     |
#                | SFA Levy co-funding budget     | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#                | SFA non-Levy co-funding budget | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        
#		And the transaction types for the payments for ULN 123 are:
#				| Payment type                   | 09/18 | 10/18 | 11/18 | ... | 07/19 | 08/19 |
#				| On-program                     | 1000  | 1000  | 1000  | ... | 1000  | 1000  |
#				| Completion                     | 0     | 0     | 0     | ... | 0     | 0     |
#				| Balancing                      | 0     | 0     | 0     | ... | 0     | 0     |
#		
#		
#		And the provider earnings and payments break down for ULN 456 as follows:
#                | Type                           | 08/18 | 09/18 | 10/18 | 11/18 | ... | 07/19 | 08/19 |
#                | Provider Earned Total          | 1000  | 1000  | 1000  | 1000  | ... | 1000  | 0     |
#                | Provider Earned from SFA       | 950   | 950   | 950   | 950   | ... | 950   | 0     |
#                | Provider Earned from Employer  | 50    | 50    | 50    | 50    | ... | 50    | 0     |
#                | Provider Paid by SFA           | 0     | 950   | 950   | 950   | ... | 950   | 950   |
#                | Payment due from Employer      | 0     | 50    | 50    | 50    | ... | 50    | 50    |
#                | Levy account debited           | 0     | 500   | 500   | 500   | ... | 500   | 500   |
#                | SFA Levy employer budget       | 500   | 500   | 500   | 500   | ... | 500   | 0     |
#                | SFA Levy co-funding budget     | 450   | 450   | 450   | 450   | ... | 450   | 0     |
#                | SFA non-Levy co-funding budget | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        
#		And the transaction types for the payments for ULN 123 are:
#				| Payment type                   | 09/18 | 10/18 | 11/18 | ... | 07/19 | 08/19 |
#				| On-program                     | 950   | 950   | 950   | ... | 950   | 950   |
#				| Completion                     | 0     | 0     | 0     | ... | 0     | 0     |
#				| Balancing                      | 0     | 0     | 0     | ... | 0     | 0     |
#		
#		
#		And OBSOLETE - the provider earnings and payments break down as follows:
#                | Type                           | 08/18 | 09/18 | 10/18 | 11/18 | ... | 07/19 | 08/19 |
#                | Provider Earned Total          | 2000  | 2000  | 2000  | 2000  | ... | 2000  | 0     |
#                | Provider Earned from SFA       | 1950  | 1950  | 1950  | 1950  | ... | 1950  | 0     |
#                | Provider Earned from Employer  | 50    | 50    | 50    | 50    | ... | 50    | 0     |
#                | Provider Paid by SFA           | 0     | 1950  | 1950  | 1950  | ... | 1950  | 1950  |
#                | Payment due from Employer      | 0     | 50    | 50    | 50    | ... | 50    | 50    |
#                | Levy account debited           | 0     | 1500  | 1500  | 1500  | ... | 1500  | 1500  |
#                | SFA Levy employer budget       | 1500  | 1500  | 1500  | 1500  | ... | 1500  | 0     |
#                | SFA Levy co-funding budget     | 450   | 450   | 450   | 450   | ... | 450   | 0     |
#                | SFA non-Levy co-funding budget | 0     | 0     | 0     | 0     | ... | 0     | 0     |

	