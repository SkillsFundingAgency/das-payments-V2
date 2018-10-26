Feature: Apprenticeship Contract 2 onprogramme earnings

Background:
	Given the current collection period is 10
	And the earnings are for the current collection year
	And a learner is undertaking a training with a training provider
	And the SFA contribution percentage is 90%
	And the Earnings Calc has generated the following learner earnings
	| Price Episode Identifier | Periods | completion status | Total training price | Total assessment price | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                      | Episode Start Date     | Learner Start Date     | Number Of Installments |
	| p1                       | 1-12    | continuing        | 9000                 | 6000                   | 1                   | ZPROG001      | 25            | 25             | 16-18 Apprenticeship Non-Levy Contract | start of academic year | start of academic year | 12                     |

Scenario: Contract Type 2 learner submission
	When the ILR is submitted and the learner earnings are sent to the earning events service
	Then the earning events service will generate a contract type 2 earnings event for the learner
	And the earnings event will contain the following earnings
	| Price Episode Identifier | Period | OnProgramme Earning Type | Amount |
	| p1                       | 1      | Learning                 | 1000   |
	| p1                       | 2      | Learning                 | 1000   |
	| p1                       | 3      | Learning                 | 1000   |
	| p1                       | 4      | Learning                 | 1000   |
	| p1                       | 5      | Learning                 | 1000   |
	| p1                       | 6      | Learning                 | 1000   |
	| p1                       | 7      | Learning                 | 1000   |
	| p1                       | 8      | Learning                 | 1000   |
	| p1                       | 9      | Learning                 | 1000   |
	| p1                       | 10     | Learning                 | 1000   |
	| p1                       | 11     | Learning                 | 1000   |
	| p1                       | 12     | Learning                 | 1000   |
	| p1                       | 12     | Completion               | 3000   |
	
