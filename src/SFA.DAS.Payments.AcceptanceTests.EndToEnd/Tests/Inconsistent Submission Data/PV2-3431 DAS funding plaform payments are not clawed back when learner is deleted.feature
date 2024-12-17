Feature: PV2-3431 DAS funding plaform payments are not clawed back when learner is deleted

Scenario: Levy learner a is deleted from ILR in 07/18, but Levy learner b is added to the 07/18 ILR PV2-3431	
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
        | learner a  | May/Current Academic Year | 600          | 500        | 1000      |
        | learner a  | Jun/Current Academic Year | 600          | 500        | 1000      |
        | learner a  | Jul/Current Academic Year | 600          | 500        | 1000      |
    And the following DAS platform provider payments had been generated
        | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | learner a  | R10/Current Academic Year | May/Current Academic Year | 600           | Learning         |
        | learner a  | R11/Current Academic Year | Jun/Current Academic Year | 600           | Learning         |
        | learner a  | R10/Current Academic Year | May/Current Academic Year | 500           | Completion       |
        | learner a  | R11/Current Academic Year | Jun/Current Academic Year | 500           | Completion       |
        | learner a  | R10/Current Academic Year | May/Current Academic Year | 1000          | Balancing        |
        | learner a  | R11/Current Academic Year | Jun/Current Academic Year | 1000          | Balancing        |
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
		| Learner ID | Delivery Period           | On-Programme              | Completion | Balancing  | Price Episode Identifier |
		| learner b  | Aug/Current Academic Year | 0                         | 0          | 0          | pe-1                     |
		| learner b  | Sep/Current Academic Year | 0                         | 0          | 0          | pe-1                     |
		| learner b  | Oct/Current Academic Year | 0                         | 0          | 0          | pe-1                     |
		| learner b  | Nov/Current Academic Year | 0                         | 0          | 0          | pe-1                     |
		| learner b  | Dec/Current Academic Year | 0                         | 0          | 0          | pe-1                     |
		| learner b  | Jan/Current Academic Year | 0                         | 0          | 0          | pe-1                     |
		| learner b  | Feb/Current Academic Year | 0                         | 0          | 0          | pe-1                     |
		| learner b  | Mar/Current Academic Year | 0                         | 0          | 0          | pe-1                     |
		| learner b  | Apr/Current Academic Year | 0                         | 0          | 0          | pe-1                     |
		| learner b  | May/Current Academic Year | 600                       | 0          | 0          | pe-1                     |
		| learner b  | Jun/Current Academic Year | 600                       | 0          | 0          | pe-1                     |
		| learner b  | Jul/Current Academic Year | 600                       | 0          | 0          | pe-1                     |
    
    And levy month end is ran
	And only the following provider payments will be recorded
        | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | learner a  | R10/Current Academic Year | May/Current Academic Year | 600           | Learning         |
        | learner a  | R11/Current Academic Year | Jun/Current Academic Year | 600           | Learning         |
        | learner a  | R10/Current Academic Year | May/Current Academic Year | 500           | Completion       |
        | learner a  | R11/Current Academic Year | Jun/Current Academic Year | 500           | Completion       |
        | learner a  | R10/Current Academic Year | May/Current Academic Year | 1000          | Balancing        |
        | learner a  | R11/Current Academic Year | Jun/Current Academic Year | 1000          | Balancing        |
        | learner b  | R12/Current Academic Year | May/Current Academic Year | 600           | Learning         |
        | learner b  | R12/Current Academic Year | Jun/Current Academic Year | 600           | Learning         |
        | learner b  | R12/Current Academic Year | Jul/Current Academic Year | 600           | Learning         |

	And only the following provider payments will be generated
        | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | learner a  | R10/Current Academic Year | May/Current Academic Year | 600           | Learning         |
        | learner a  | R11/Current Academic Year | Jun/Current Academic Year | 600           | Learning         |
        | learner a  | R10/Current Academic Year | May/Current Academic Year | 500           | Completion       |
        | learner a  | R11/Current Academic Year | Jun/Current Academic Year | 500           | Completion       |
        | learner a  | R10/Current Academic Year | May/Current Academic Year | 1000          | Balancing        |
        | learner a  | R11/Current Academic Year | Jun/Current Academic Year | 1000          | Balancing        |
        | learner b  | R12/Current Academic Year | May/Current Academic Year | 600           | Learning         |
        | learner b  | R12/Current Academic Year | Jun/Current Academic Year | 600           | Learning         |
        | learner b  | R12/Current Academic Year | Jul/Current Academic Year | 600           | Learning         |

