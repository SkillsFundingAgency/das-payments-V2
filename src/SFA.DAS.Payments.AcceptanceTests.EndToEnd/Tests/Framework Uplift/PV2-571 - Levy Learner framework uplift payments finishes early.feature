Feature: Levy learner, with a framework uplift, finishes early, Balancing payment applied PV2-571
	As a provider,
	I want a Levy learner with a framework uplift, where the learner finishes earlier than planned end date, and a balancing payment is applied
	So that I am accurately paid my apprenticeship provision

Scenario: A Levy learner with a framework uplift payments finishes early PV2-571
	Given the employer levy account balance in collection period R10/2025-26 Academic Year is 8750
	And the following commitments exist
		| start date                   | end date                  | agreed price |	 Framework Code | Pathway Code | Programme Type |
		| 06/Aug/2025-26 Academic Year | 09/Aug/2026-27 Academic Year | 8250         |	 593            | 1            | 20             |
	And the provider previously submitted the following learner details
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                 | SFA Contribution Percentage |
		| 06/Aug/2025-26 Academic Year | 12 months        | 8250                 | 06/Aug/2025-26 Academic Year        | 0                      | 06/Aug/2025-26 Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Levy Contract (non-procured) | 90%                         |
    And the following earnings had been generated for the learner
        | Delivery Period           | On-Programme | Completion | Balancing | OnProgramme16To18FrameworkUplift |
        | Aug/2025-26 Academic Year | 550          | 0          | 0         | 120                              |
        | Sep/2025-26 Academic Year | 550          | 0          | 0         | 120                              |
        | Oct/2025-26 Academic Year | 550          | 0          | 0         | 120                              |
        | Nov/2025-26 Academic Year | 550          | 0          | 0         | 120                              |
        | Dec/2025-26 Academic Year | 550          | 0          | 0         | 120                              |
        | Jan/2025-26 Academic Year | 550          | 0          | 0         | 120                              |
        | Feb/2025-26 Academic Year | 550          | 0          | 0         | 120                              |
        | Mar/2025-26 Academic Year | 550          | 0          | 0         | 120                              |
        | Apr/2025-26 Academic Year | 550          | 0          | 0         | 120                              |
        | May/2025-26 Academic Year | 550          | 0          | 0         | 120                              |
        | Jun/2025-26 Academic Year | 550          | 0          | 0         | 120                              |
        | Jul/2025-26 Academic Year | 550          | 0          | 0         | 120                              |
    And the following provider payments had been generated
        | Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type                 |
        | R01/2025-26 Academic Year | Aug/2025-26 Academic Year | 550           | 0                         | Learning                         |
        | R02/2025-26 Academic Year | Sep/2025-26 Academic Year | 550           | 0                         | Learning                         |
        | R03/2025-26 Academic Year | Oct/2025-26 Academic Year | 550           | 0                         | Learning                         |
        | R04/2025-26 Academic Year | Nov/2025-26 Academic Year | 550           | 0                         | Learning                         |
        | R05/2025-26 Academic Year | Dec/2025-26 Academic Year | 550           | 0                         | Learning                         |
        | R06/2025-26 Academic Year | Jan/2025-26 Academic Year | 550           | 0                         | Learning                         |
        | R07/2025-26 Academic Year | Feb/2025-26 Academic Year | 550           | 0                         | Learning                         |
        | R08/2025-26 Academic Year | Mar/2025-26 Academic Year | 550           | 0                         | Learning                         |
        | R09/2025-26 Academic Year | Apr/2025-26 Academic Year | 550           | 0                         | Learning                         |
        | R01/2025-26 Academic Year | Aug/2025-26 Academic Year | 0             | 120                       | OnProgramme16To18FrameworkUplift |
        | R02/2025-26 Academic Year | Sep/2025-26 Academic Year | 0             | 120                       | OnProgramme16To18FrameworkUplift |
        | R03/2025-26 Academic Year | Oct/2025-26 Academic Year | 0             | 120                       | OnProgramme16To18FrameworkUplift |
        | R04/2025-26 Academic Year | Nov/2025-26 Academic Year | 0             | 120                       | OnProgramme16To18FrameworkUplift |
        | R05/2025-26 Academic Year | Dec/2025-26 Academic Year | 0             | 120                       | OnProgramme16To18FrameworkUplift |
        | R06/2025-26 Academic Year | Jan/2025-26 Academic Year | 0             | 120                       | OnProgramme16To18FrameworkUplift |
        | R07/2025-26 Academic Year | Feb/2025-26 Academic Year | 0             | 120                       | OnProgramme16To18FrameworkUplift |
        | R08/2025-26 Academic Year | Mar/2025-26 Academic Year | 0             | 120                       | OnProgramme16To18FrameworkUplift |
        | R09/2025-26 Academic Year | Apr/2025-26 Academic Year | 0             | 120                       | OnProgramme16To18FrameworkUplift |
    But the Provider now changes the Learner details as follows
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                 | SFA Contribution Percentage |
		| 06/Aug/2025-26 Academic Year | 12 months        | 8250                 | 06/Aug/2025-26 Academic Year        | 0                      | 06/Aug/2025-26 Academic Year          | 9 months        | completed         | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Levy Contract (non-procured) | 90%                         |
	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
		| pe-1             | 8250                 | 06/Aug/2025-26 Academic Year        | Act1          | 1                   | 90%                         |
	When the amended ILR file is re-submitted for the learners in collection period R10/2025-26 Academic Year
	Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing | OnProgramme16To18FrameworkUplift | Completion16To18FrameworkUplift | Balancing16To18FrameworkUplift | Price Episode Identifier |
        | Aug/2025-26 Academic Year | 550          | 0          | 0         | 120                              | 0                               | 0                              | pe-1                     |
        | Sep/2025-26 Academic Year | 550          | 0          | 0         | 120                              | 0                               | 0                              | pe-1                     |
        | Oct/2025-26 Academic Year | 550          | 0          | 0         | 120                              | 0                               | 0                              | pe-1                     |
        | Nov/2025-26 Academic Year | 550          | 0          | 0         | 120                              | 0                               | 0                              | pe-1                     |
        | Dec/2025-26 Academic Year | 550          | 0          | 0         | 120                              | 0                               | 0                              | pe-1                     |
        | Jan/2025-26 Academic Year | 550          | 0          | 0         | 120                              | 0                               | 0                              | pe-1                     |
        | Feb/2025-26 Academic Year | 550          | 0          | 0         | 120                              | 0                               | 0                              | pe-1                     |
        | Mar/2025-26 Academic Year | 550          | 0          | 0         | 120                              | 0                               | 0                              | pe-1                     |
        | Apr/2025-26 Academic Year | 550          | 0          | 0         | 120                              | 0                               | 0                              | pe-1                     |
        | May/2025-26 Academic Year | 0            | 1650       | 1650      | 0                                | 360                             | 360                            | pe-1                     |
        | Jun/2025-26 Academic Year | 0            | 0          | 0         | 0                                | 0                               | 0                              | pe-1                     |
        | Jul/2025-26 Academic Year | 0            | 0          | 0         | 0                                | 0                               | 0                              | pe-1                     |
    And at month end only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | Completion16To18FrameworkUplift | Balancing16To18FrameworkUplift |
		| R10/2025-26 Academic Year | May/2025-26 Academic Year | 0            | 1650       | 1650      | 360                             | 360                            |
	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type                |
		| R10/2025-26 Academic Year | May/2025-26 Academic Year | 1650          | 0                         | Completion                      |
		| R10/2025-26 Academic Year | May/2025-26 Academic Year | 1650          | 0                         | Balancing                       |
		| R10/2025-26 Academic Year | May/2025-26 Academic Year | 0             | 360                       | Completion16To18FrameworkUplift |
		| R10/2025-26 Academic Year | May/2025-26 Academic Year | 0             | 360                       | Balancing16To18FrameworkUplift  |
	And only the following provider payments will be generated
		| Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type                |
		| R10/2025-26 Academic Year | May/2025-26 Academic Year | 1650          | 0                         | Completion                      |
		| R10/2025-26 Academic Year | May/2025-26 Academic Year | 1650          | 0                         | Balancing                       |
		| R10/2025-26 Academic Year | May/2025-26 Academic Year | 0             | 360                       | Completion16To18FrameworkUplift |
		| R10/2025-26 Academic Year | May/2025-26 Academic Year | 0             | 360                       | Balancing16To18FrameworkUplift  |