@ignore
Feature: Two levy learners, levy available but for only one learner, levy spent in priority order PV2-603
	As a provider,
	I want 2 Levy learners, where levy is spent in priority order and there is only enough levy available for one learner
	So that I am accurately paid the apprenticeship amount by SFA

Scenario Outline: Two levy learners, levy available but for only one learner, levy spent in priority order PV2-603
	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
	And the following commitments exist
		| Identifier           | Learner ID | priority | start date                   | end date                     | agreed price |	Framework Code | Pathway Code | Programme Type |
		| Apprenticeship 1      | learner a  | 1        | 01/Aug/Current Academic Year | 08/Sep/Current Academic Year | 15000        |	403            | 1            | 2              |
		| Apprenticeship 2      | learner b  | 2        | 01/Aug/Current Academic Year | 08/Sep/Current Academic Year | 15000        |	403            | 1            | 2              |
	And the provider is providing training for the following learners
		| Learner ID | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| learner a  | 01/Aug/Current Academic Year | 12 months        | 15000                | 01/Aug/Current Academic Year        | 0                      | 01/Aug/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 403            | 1            | 2              | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
		| learner b  | 01/Aug/Current Academic Year | 12 months        | 15000                | 01/Aug/Current Academic Year        | 0                      | 01/Aug/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 403            | 1            | 2              | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
	And price details as follows
		| Learner ID | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Contract Type | Aim Sequence Number |
		| learner a  | pe-1             | 15000                | 01/Aug/Current Academic Year        | 0                      | 01/Aug/Current Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         | Act1          | 1                   |
		| learner b  | pe-2             | 15000                | 01/Aug/Current Academic Year        | 0                      | 01/Aug/Current Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         | Act1          | 1                   |
	When the ILR file is submitted for the learners for collection period <Collection_Period>
    Then the following learner earnings should be generated
        | Learner ID | Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
        | learner a  | Aug/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
        | learner a  | Sep/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
        | learner a  | Oct/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
        | learner a  | Nov/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
        | learner a  | Dec/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
        | learner a  | Jan/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
        | learner a  | Feb/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
        | learner a  | Mar/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
        | learner a  | Apr/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
        | learner a  | May/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
        | learner a  | Jun/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
        | learner a  | Jul/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
        | learner b  | Aug/Current Academic Year | 1000         | 0          | 0         | pe-2                     |
        | learner b  | Sep/Current Academic Year | 1000         | 0          | 0         | pe-2                     |
        | learner b  | Oct/Current Academic Year | 1000         | 0          | 0         | pe-2                     |
        | learner b  | Nov/Current Academic Year | 1000         | 0          | 0         | pe-2                     |
        | learner b  | Dec/Current Academic Year | 1000         | 0          | 0         | pe-2                     |
        | learner b  | Jan/Current Academic Year | 1000         | 0          | 0         | pe-2                     |
        | learner b  | Feb/Current Academic Year | 1000         | 0          | 0         | pe-2                     |
        | learner b  | Mar/Current Academic Year | 1000         | 0          | 0         | pe-2                     |
        | learner b  | Apr/Current Academic Year | 1000         | 0          | 0         | pe-2                     |
        | learner b  | May/Current Academic Year | 1000         | 0          | 0         | pe-2                     |
        | learner b  | Jun/Current Academic Year | 1000         | 0          | 0         | pe-2                     |
        | learner b  | Jul/Current Academic Year | 1000         | 0          | 0         | pe-2                     |
	And at month end only the following payments will be calculated
        | Learner ID | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
        | learner a  | R03/Current Academic Year | Aug/Current Academic Year | 1000         | 0          | 0         |
        | learner a  | R03/Current Academic Year | Sep/Current Academic Year | 1000         | 0          | 0         |
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
        | learner b  | R03/Current Academic Year | Aug/Current Academic Year | 1000         | 0          | 0         |
        | learner b  | R03/Current Academic Year | Sep/Current Academic Year | 1000         | 0          | 0         |
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
        | learner a  | R03/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
        | learner a  | R03/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
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
        | learner b  | R03/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R03/Current Academic Year | Sep/Current Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R03/Current Academic Year | Oct/Current Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R04/Current Academic Year | Nov/Current Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R05/Current Academic Year | Dec/Current Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R06/Current Academic Year | Jan/Current Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R07/Current Academic Year | Feb/Current Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R08/Current Academic Year | Mar/Current Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R09/Current Academic Year | Apr/Current Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R10/Current Academic Year | May/Current Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R11/Current Academic Year | Jun/Current Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R12/Current Academic Year | Jul/Current Academic Year | 900                    | 100                         | 0             | Learning         |
    And only the following provider payments will be generated
        | Learner ID | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
        | learner a  | R03/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
        | learner a  | R03/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 1000          | Learning         |
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
        | learner b  | R03/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R03/Current Academic Year | Sep/Current Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R03/Current Academic Year | Oct/Current Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R04/Current Academic Year | Nov/Current Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R05/Current Academic Year | Dec/Current Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R06/Current Academic Year | Jan/Current Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R07/Current Academic Year | Feb/Current Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R08/Current Academic Year | Mar/Current Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R09/Current Academic Year | Apr/Current Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R10/Current Academic Year | May/Current Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R11/Current Academic Year | Jun/Current Academic Year | 900                    | 100                         | 0             | Learning         |
        | learner b  | R12/Current Academic Year | Jul/Current Academic Year | 900                    | 100                         | 0             | Learning         |
Examples: 
        | Collection_Period         | Levy Balance |
        | R03/Current Academic Year | 3000         |
        | R04/Current Academic Year | 1000         |
        | R05/Current Academic Year | 1000         |
        | R06/Current Academic Year | 1000         |
        | R07/Current Academic Year | 1000         |
        | R08/Current Academic Year | 1000         |
        | R09/Current Academic Year | 1000         |
        | R10/Current Academic Year | 1000         |
        | R11/Current Academic Year | 1000         |
        | R12/Current Academic Year | 1000         |

#Feature: Payment Priority
#
#Background: 2 learners, paid in priority order
#
#Scenario: Earnings and payments for two Levy learners, levy is spent in priority order and is available for one learner only
#
#        Given Two learners are programme only DAS 
#		
#        And the apprenticeship funding band maximum for each learner is 17000
#		
#		And the employer's levy balance is:
#                | 09/18 | 10/18 | 11/18 | 12/18 | ... | 09/19 | 10/19 |
#                | 1000  | 1000  | 1000  | 1000  | ... | 1000  | 1000  |
#        
#		And the following commitments exist on 03/11/2018:
#                | priority | ULN | start date | end date   | agreed price |
#                | 1        | 123 | 01/08/2018 | 28/08/2019 | 15000        |
#                | 2        | 456 | 01/08/2018 | 28/08/2019 | 15000        |
#        
#		When an ILR file is submitted on 03/11/2018 with the following data:
#                | ULN | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date |
#                | 123 | 01/08/2018 | 28/08/2019       |                 | continuing        | 12000                | 01/08/2018                          | 3000                   | 01/08/2018                            |
#                | 456 | 01/08/2018 | 28/08/2019       |                 | continuing        | 12000                | 01/08/2018                          | 3000                   | 01/08/2018                            |
#        
#		Then the provider earnings and payments break down for ULN 123 as follows:
#                | Type                           | 08/18 | 09/18 | 10/18 | 11/18 | ... | 07/19 | 08/19 |
#                | Provider Earned Total          | 1000  | 1000  | 1000  | 1000  | ... | 1000  | 0     |
#                | Provider Earned from SFA       | 1000  | 1000  | 1000  | 1000  | ... | 1000  | 0     |
#                | Provider Earned from Employer  | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#                | Provider Paid by SFA           | 0     | 0     | 0     | 3000  | ... | 1000  | 1000  |
#                | Payment due from Employer      | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#                | Levy account debited           | 0     | 0     | 0     | 3000  | ... | 1000  | 1000  |
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
#                | Provider Earned from SFA       | 900   | 900   | 900   | 900   | ... | 900   | 0     |
#                | Provider Earned from Employer  | 100   | 100   | 100   | 100   | ... | 100   | 0     |
#                | Provider Paid by SFA           | 0     | 0     | 0     | 2700  | ... | 900   | 900   |
#                | Payment due from Employer      | 0     | 0     | 0     | 300   | ... | 100   | 100   |
#                | Levy account debited           | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#                | SFA Levy employer budget       | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#                | SFA Levy co-funding budget     | 900   | 900   | 900   | 900   | ... | 900   | 0     |
#                | SFA non-Levy co-funding budget | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        
#		And the transaction types for the payments for ULN 456 are:
#				| Payment type                   | 09/18 | 10/18 | 11/18 | ... | 07/19 | 08/19 |
#				| On-program                     | 900   | 900   | 900   | ... | 900   | 900   |
#				| Completion                     | 0     | 0     | 0     | ... | 0     | 0     |
#				| Balancing                      | 0     | 0     | 0     | ... | 0     | 0     |
#		
#		
#		And OBSOLETE - the provider earnings and payments break down as follows:
#                | Type                           | 08/18 | 09/18 | 10/18 | 11/18 | ... | 07/19 | 08/19 |
#                | Provider Earned Total          | 2000  | 2000  | 2000  | 2000  | ... | 2000  | 0     |
#                | Provider Earned from SFA       | 1900  | 1900  | 1900  | 1900  | ... | 1900  | 0     |
#                | Provider Earned from Employer  | 100   | 100   | 100   | 100   | ... | 100   | 0     |
#                | Provider Paid by SFA           | 0     | 0     | 0     | 5700  | ... | 1900  | 1900  |
#                | Payment due from Employer      | 0     | 0     | 0     | 300   | ... | 100   | 100   |
#                | Levy account debited           | 0     | 0     | 0     | 3000  | ... | 1000  | 1000  |
#                | SFA Levy employer budget       | 1000  | 1000  | 1000  | 1000  | ... | 1000  | 0     |
#                | SFA Levy co-funding budget     | 900   | 900   | 900   | 900   | ... | 900   | 0     |
#                | SFA non-Levy co-funding budget | 0     | 0     | 0     | 0     | ... | 0     | 0     |

