Feature: Small Employer
Non-DAS learner employed with a small employer, is fully funded for on programme and completion payments

Background:
	Given the current collection period is 13
	And the payments are for the current collection year
	And a learner is undertaking a training with a training provider
	And the SFA contribution percentage is 100%

	And the Earnings Calc has generated the following learner earnings
	| Price Episode Identifier | Periods | completion status | Total training price | Total assessment price | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | Episode Start Date     | Learner Start Date     | Number Of Installments |
	| p1                       | 1-12    | completed         | 6000                 | 1500                   | 1                   | ZPROG001      | 403            | 1            | 2              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | start of academic year | start of academic year | 12                     |

@SmallEmployerNonDas

Scenario: AC1-Payment for a 16-18 non-DAS learner, small employer at start
	When the ILR is submitted and the learner earnings are sent to the earning events service
	Then the earning events service will generate a contract type 2 earnings event for the learner
	And the earnings event will contain the following earnings
	| Price Episode Identifier | Period | OnProgramme Earning Type | Amount |
	| p1                       | 1      | Learning                 | 500    |
	| p1                       | 2      | Learning                 | 500    |
	| p1                       | 3      | Learning                 | 500    |
	| p1                       | 4      | Learning                 | 500    |
	| p1                       | 5      | Learning                 | 500    |
	| p1                       | 6      | Learning                 | 500    |
	| p1                       | 7      | Learning                 | 500    |
	| p1                       | 8      | Learning                 | 500    |
	| p1                       | 9      | Learning                 | 500    |
	| p1                       | 10     | Learning                 | 500    |
	| p1                       | 11     | Learning                 | 500    |
	| p1                       | 12     | Learning                 | 500    |
	| p1                       | 12     | Completion               | 1500   |

Scenario: AC5- Payment for a 16-18 non-DAS learner, employer is not small
	When the ILR is submitted and the learner earnings are sent to the earning events service
	Then the earning events service will generate a contract type 2 earnings event for the learner
	And the earnings event will contain the following earnings
	| Price Episode Identifier | Period | OnProgramme Earning Type | Amount |
	| p1                       | 1      | Learning                 | 500    |
	| p1                       | 2      | Learning                 | 500    |
	| p1                       | 3      | Learning                 | 500    |
	| p1                       | 4      | Learning                 | 500    |
	| p1                       | 5      | Learning                 | 500    |
	| p1                       | 6      | Learning                 | 500    |
	| p1                       | 7      | Learning                 | 500    |
	| p1                       | 8      | Learning                 | 500    |
	| p1                       | 9      | Learning                 | 500    |
	| p1                       | 10     | Learning                 | 500    |
	| p1                       | 11     | Learning                 | 500    |
	| p1                       | 12     | Learning                 | 500    |
	| p1                       | 12     | Completion               | 1500   |
