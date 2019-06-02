Feature: Non Levy learner, with a framework uplift, finishes early, Balancing payment applied PV2-572
		As a provider,
		I want a Non-Levy learner with a framework uplift, where the learner finishes earlier than planned end date, and a balancing payment is applied
		So that I am accurately paid my apprenticeship provision

Scenario: A non-levy learner with a framework uplift payments finishes early PV2-572
	Given the provider previously submitted the following learner details
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 06/Aug/Current Academic Year | 12 months        | 8250                 | 06/Aug/Current Academic Year        | 0                      | 06/Aug/Current Academic Year          |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |
    And the following earnings had been generated for the learner
        | Delivery Period           | On-Programme | Completion | Balancing | OnProgramme16To18FrameworkUplift |
        | Aug/Current Academic Year | 550          | 0          | 0         | 120                              |
        | Sep/Current Academic Year | 550          | 0          | 0         | 120                              |
        | Oct/Current Academic Year | 550          | 0          | 0         | 120                              |
        | Nov/Current Academic Year | 550          | 0          | 0         | 120                              |
        | Dec/Current Academic Year | 550          | 0          | 0         | 120                              |
        | Jan/Current Academic Year | 550          | 0          | 0         | 120                              |
        | Feb/Current Academic Year | 550          | 0          | 0         | 120                              |
        | Mar/Current Academic Year | 550          | 0          | 0         | 120                              |
        | Apr/Current Academic Year | 550          | 0          | 0         | 120                              |
        | May/Current Academic Year | 550          | 0          | 0         | 120                              |
        | Jun/Current Academic Year | 550          | 0          | 0         | 120                              |
        | Jul/Current Academic Year | 550          | 0          | 0         | 120                              |
    And the following provider payments had been generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type                 |
        | R01/Current Academic Year | Aug/Current Academic Year | 495                    | 55                          | 0                         | Learning                         |
        | R02/Current Academic Year | Sep/Current Academic Year | 495                    | 55                          | 0                         | Learning                         |
        | R03/Current Academic Year | Oct/Current Academic Year | 495                    | 55                          | 0                         | Learning                         |
        | R04/Current Academic Year | Nov/Current Academic Year | 495                    | 55                          | 0                         | Learning                         |
        | R05/Current Academic Year | Dec/Current Academic Year | 495                    | 55                          | 0                         | Learning                         |
        | R06/Current Academic Year | Jan/Current Academic Year | 495                    | 55                          | 0                         | Learning                         |
        | R07/Current Academic Year | Feb/Current Academic Year | 495                    | 55                          | 0                         | Learning                         |
        | R08/Current Academic Year | Mar/Current Academic Year | 495                    | 55                          | 0                         | Learning                         |
        | R09/Current Academic Year | Apr/Current Academic Year | 495                    | 55                          | 0                         | Learning                         |
        | R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R03/Current Academic Year | Oct/Current Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R04/Current Academic Year | Nov/Current Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R05/Current Academic Year | Dec/Current Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R06/Current Academic Year | Jan/Current Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R07/Current Academic Year | Feb/Current Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R08/Current Academic Year | Mar/Current Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
        | R09/Current Academic Year | Apr/Current Academic Year | 0                      | 0                           | 120                       | OnProgramme16To18FrameworkUplift |
    But the Provider now changes the Learner details as follows
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 06/Aug/Current Academic Year | 12 months        | 8250                 | 06/Aug/Current Academic Year        | 0                      | 06/Aug/Current Academic Year          | 9 months        | completed         | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |
	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
		| pe-1             | 8250                 | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | 0                       |                                        | 0                         |                                          | Act1          | 1                   | 90%                         |
	When the amended ILR file is re-submitted for the learners in collection period R10/Current Academic Year
	Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing | OnProgramme16To18FrameworkUplift | Completion16To18FrameworkUplift | Balancing16To18FrameworkUplift | Price Episode Identifier |
        | Aug/Current Academic Year | 550          | 0          | 0         | 120                              | 0                               | 0                              | pe-1                     |
        | Sep/Current Academic Year | 550          | 0          | 0         | 120                              | 0                               | 0                              | pe-1                     |
        | Oct/Current Academic Year | 550          | 0          | 0         | 120                              | 0                               | 0                              | pe-1                     |
        | Nov/Current Academic Year | 550          | 0          | 0         | 120                              | 0                               | 0                              | pe-1                     |
        | Dec/Current Academic Year | 550          | 0          | 0         | 120                              | 0                               | 0                              | pe-1                     |
        | Jan/Current Academic Year | 550          | 0          | 0         | 120                              | 0                               | 0                              | pe-1                     |
        | Feb/Current Academic Year | 550          | 0          | 0         | 120                              | 0                               | 0                              | pe-1                     |
        | Mar/Current Academic Year | 550          | 0          | 0         | 120                              | 0                               | 0                              | pe-1                     |
        | Apr/Current Academic Year | 550          | 0          | 0         | 120                              | 0                               | 0                              | pe-1                     |
        | May/Current Academic Year | 0            | 1650       | 1650      | 0                                | 360                             | 360                            | pe-1                     |
        | Jun/Current Academic Year | 0            | 0          | 0         | 0                                | 0                               | 0                              | pe-1                     |
        | Jul/Current Academic Year | 0            | 0          | 0         | 0                                | 0                               | 0                              | pe-1                     |
    And only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | Completion16To18FrameworkUplift | Balancing16To18FrameworkUplift |
		| R10/Current Academic Year | May/Current Academic Year | 0            | 1650       | 1650      | 360                             | 360                            |
	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type                |
		| R10/Current Academic Year | May/Current Academic Year | 1485                   | 165                         | 0                         | Completion                      |
		| R10/Current Academic Year | May/Current Academic Year | 1485                   | 165                         | 0                         | Balancing                       |
		| R10/Current Academic Year | May/Current Academic Year | 0                      | 0                           | 360                       | Completion16To18FrameworkUplift |
		| R10/Current Academic Year | May/Current Academic Year | 0                      | 0                           | 360                       | Balancing16To18FrameworkUplift  |
	And at month end only the following provider payments will be generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type                |
		| R10/Current Academic Year | May/Current Academic Year | 1485                   | 165                         | 0                         | Completion                      |
		| R10/Current Academic Year | May/Current Academic Year | 1485                   | 165                         | 0                         | Balancing                       |
		| R10/Current Academic Year | May/Current Academic Year | 0                      | 0                           | 360                       | Completion16To18FrameworkUplift |
		| R10/Current Academic Year | May/Current Academic Year | 0                      | 0                           | 360                       | Balancing16To18FrameworkUplift  |