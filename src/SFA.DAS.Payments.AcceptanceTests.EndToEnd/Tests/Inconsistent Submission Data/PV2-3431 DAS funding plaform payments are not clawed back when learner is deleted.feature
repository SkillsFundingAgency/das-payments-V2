Feature: PV2-3431 DAS funding plaform payments are not clawed back when learner is deleted

Scenario: Levy learner a is deleted from ILR in 07/18, but Levy learner b is added to the 07/18 ILR PV2-3431	
	Given the employer levy account balance in collection period R12/2025-26 Academic Year is 9000
	And the following commitments exist
        | Identifier       | Learner ID | start date                   | end date                  | agreed price | Framework Code | Pathway Code | Programme Type |
        | Apprenticeship 1 | learner a  | 01/May/2025-26 Academic Year | 01/May/2026-27 Academic Year | 9000         | 593            | 1            | 20             |
	And the provider previously submitted the following learner details
		| Learner ID | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| learner a  | 06/May/2025-26 Academic Year | 12 months        | 9000                 | 06/May/2025-26 Academic Year        | 0                      | 06/May/2025-26 Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
    And the following earnings had been generated for the learner
        | Learner ID | Delivery Period           | On-Programme | Completion | Balancing |
        | learner a  | Aug/2025-26 Academic Year | 0            | 0          | 0         |
        | learner a  | Sep/2025-26 Academic Year | 0            | 0          | 0         |
        | learner a  | Oct/2025-26 Academic Year | 0            | 0          | 0         |
        | learner a  | Nov/2025-26 Academic Year | 0            | 0          | 0         |
        | learner a  | Dec/2025-26 Academic Year | 0            | 0          | 0         |
        | learner a  | Jan/2025-26 Academic Year | 0            | 0          | 0         |
        | learner a  | Feb/2025-26 Academic Year | 0            | 0          | 0         |
        | learner a  | Mar/2025-26 Academic Year | 0            | 0          | 0         |
        | learner a  | Apr/2025-26 Academic Year | 0            | 0          | 0         |
        | learner a  | May/2025-26 Academic Year | 600          | 500        | 1000      |
        | learner a  | Jun/2025-26 Academic Year | 600          | 500        | 1000      |
        | learner a  | Jul/2025-26 Academic Year | 600          | 500        | 1000      |
    And the following DAS platform provider payments had been generated
        | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | learner a  | R10/2025-26 Academic Year | May/2025-26 Academic Year | 600           | Learning         |
        | learner a  | R11/2025-26 Academic Year | Jun/2025-26 Academic Year | 600           | Learning         |
        | learner a  | R10/2025-26 Academic Year | May/2025-26 Academic Year | 500           | Completion       |
        | learner a  | R11/2025-26 Academic Year | Jun/2025-26 Academic Year | 500           | Completion       |
        | learner a  | R10/2025-26 Academic Year | May/2025-26 Academic Year | 1000          | Balancing        |
        | learner a  | R11/2025-26 Academic Year | Jun/2025-26 Academic Year | 1000          | Balancing        |
    But the Commitment details are changed as follows
        | Identifier       | Learner ID | start date                   | end date                  | agreed price | Framework Code | Pathway Code | Programme Type |
        | Apprenticeship 2 | learner b  | 01/May/2025-26 Academic Year | 01/May/2026-27 Academic Year | 9000         | 593            | 1            | 20             |
    And the Provider now changes the Learner details as follows
		| Learner ID | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| learner b  | 06/May/2025-26 Academic Year | 12 months        | 9000                 | 06/May/2025-26 Academic Year        | 0                      | 06/May/2025-26 Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
		| pe-1             | 9000                 | 06/May/2025-26 Academic Year        | 0                      | 06/May/2025-26 Academic Year          | 0                       |                                        | 0                         |                                          | Act1          | 1                   | 90%                         |

	When the amended ILR file is re-submitted for the learners in collection period R12/2025-26 Academic Year
	Then the following learner earnings should be generated
		| Learner ID | Delivery Period           | On-Programme              | Completion | Balancing  | Price Episode Identifier |
		| learner b  | Aug/2025-26 Academic Year | 0                         | 0          | 0          | pe-1                     |
		| learner b  | Sep/2025-26 Academic Year | 0                         | 0          | 0          | pe-1                     |
		| learner b  | Oct/2025-26 Academic Year | 0                         | 0          | 0          | pe-1                     |
		| learner b  | Nov/2025-26 Academic Year | 0                         | 0          | 0          | pe-1                     |
		| learner b  | Dec/2025-26 Academic Year | 0                         | 0          | 0          | pe-1                     |
		| learner b  | Jan/2025-26 Academic Year | 0                         | 0          | 0          | pe-1                     |
		| learner b  | Feb/2025-26 Academic Year | 0                         | 0          | 0          | pe-1                     |
		| learner b  | Mar/2025-26 Academic Year | 0                         | 0          | 0          | pe-1                     |
		| learner b  | Apr/2025-26 Academic Year | 0                         | 0          | 0          | pe-1                     |
		| learner b  | May/2025-26 Academic Year | 600                       | 0          | 0          | pe-1                     |
		| learner b  | Jun/2025-26 Academic Year | 600                       | 0          | 0          | pe-1                     |
		| learner b  | Jul/2025-26 Academic Year | 600                       | 0          | 0          | pe-1                     |
    
    And levy month end is ran
	And only the following provider payments will be recorded
        | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | learner a  | R10/2025-26 Academic Year | May/2025-26 Academic Year | 600           | Learning         |
        | learner a  | R11/2025-26 Academic Year | Jun/2025-26 Academic Year | 600           | Learning         |
        | learner a  | R10/2025-26 Academic Year | May/2025-26 Academic Year | 500           | Completion       |
        | learner a  | R11/2025-26 Academic Year | Jun/2025-26 Academic Year | 500           | Completion       |
        | learner a  | R10/2025-26 Academic Year | May/2025-26 Academic Year | 1000          | Balancing        |
        | learner a  | R11/2025-26 Academic Year | Jun/2025-26 Academic Year | 1000          | Balancing        |
        | learner b  | R12/2025-26 Academic Year | May/2025-26 Academic Year | 600           | Learning         |
        | learner b  | R12/2025-26 Academic Year | Jun/2025-26 Academic Year | 600           | Learning         |
        | learner b  | R12/2025-26 Academic Year | Jul/2025-26 Academic Year | 600           | Learning         |

	And only the following provider payments will be generated
        | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | learner a  | R10/2025-26 Academic Year | May/2025-26 Academic Year | 600           | Learning         |
        | learner a  | R11/2025-26 Academic Year | Jun/2025-26 Academic Year | 600           | Learning         |
        | learner a  | R10/2025-26 Academic Year | May/2025-26 Academic Year | 500           | Completion       |
        | learner a  | R11/2025-26 Academic Year | Jun/2025-26 Academic Year | 500           | Completion       |
        | learner a  | R10/2025-26 Academic Year | May/2025-26 Academic Year | 1000          | Balancing        |
        | learner a  | R11/2025-26 Academic Year | Jun/2025-26 Academic Year | 1000          | Balancing        |
        | learner b  | R12/2025-26 Academic Year | May/2025-26 Academic Year | 600           | Learning         |
        | learner b  | R12/2025-26 Academic Year | Jun/2025-26 Academic Year | 600           | Learning         |
        | learner b  | R12/2025-26 Academic Year | Jul/2025-26 Academic Year | 600           | Learning         |

