﻿Feature: One Non-Levy Learner Finishes Early PV2-220
Provider earnings and payments where learner completes earlier than planned

@NonDas_BasicDay
@finishes_early
Scenario: A non-DAS learner, learner finishes early
	Given the current collection period is 2
	And a learner is undertaking a training with a training provider
	And the payments are for the current collection year
	And the earnings are for a test learner and a test provider
	And the SFA contribution percentage is 90%
	And the Earnings Calc has generated the following learner earnings
	| Price Episode Identifier | Periods | completion status | Total training price | Total assessment price | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | Episode Start Date     | Learner Start Date     | Number Of Installments |
	| p1                       | 1-12    | completed         | 12000                | 3000                   | 1                   | ZPROG001      | 403            | 1            | 2              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | start of academic year | start of academic year | 12                     |

	When the ILR is submitted and the learner earnings are sent to the earning events service
	Then the earning events service will generate a contract type 2 earnings event for the learner
	And the earnings event will contain the following earnings
	| Price Episode Identifier | Period | OnProgramme Earning Type | Amount |
	| p1                       | 1      | Learning                 | 1000   |
	| p1                       | 5      | Completion               | 3000   |

@withdrawal
Scenario: A non-DAS learner, learner withdraws after qualifying period
Given the current collection period is 6
	And the payments are for the current collection year
	And a learner is undertaking a training with a training provider
	And the SFA contribution percentage is 90%
	And the Earnings Calc has generated the following learner earnings
	| Price Episode Identifier | Periods | completion status | Total training price | Total assessment price | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | Episode Start Date     | Learner Start Date     | Number Of Installments |
	| p1                       | 1-12    | withdrawn         | 9000                 | 6000                   | 1                   | ZPROG001      | 403            | 1            | 2              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | start of academic year | start of academic year | 12                     |

	When the ILR is submitted and the learner earnings are sent to the earning events service
	Then the earning events service will generate a contract type 2 earnings event for the learner
	And the earnings event will contain the following earnings
	| Price Episode Identifier | Period | OnProgramme Earning Type | Amount |
	| p1                       | 1      | Learning                 | 1000   |