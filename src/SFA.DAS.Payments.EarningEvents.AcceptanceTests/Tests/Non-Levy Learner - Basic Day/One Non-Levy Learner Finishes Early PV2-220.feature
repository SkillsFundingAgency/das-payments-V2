Feature: One Non-Levy Learner Finishes On Time PV2-244

Background:
	Given the current collection period is 3
	And the payments are for the current collection year
	And the earnings are for a test learner and a test provider
	And the SFA contribution percentage is 90%
	And the Earnings Calc has generated the following learner earnings
	| Price Episode Identifier | Periods | completion status | Total training price | Total assessment price | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | Episode Start Date     | Learner Start Date     | Number Of Installments |
	| p1                       | 1-12    | completed         | 9000                 | 6000                   | 1                   | ZPROG001      | 403            | 1            | 2              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | start of academic year | start of academic year | 12                     |

@NonDas_BasicDay
Scenario: 1_non_levy_learner_finishes_OnTime
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


#	Feature: One Non-Levy Learner Finishes Early PV2-195
#Provider earnings and payments where learner completes earlier than planned
#
#Background:
#	Given a learner is undertaking a training with a training provider
#	And the SFA contribution percentage is 90%
#	And the payments are for the current collection year
#
#@NonDas_BasicDay
#@finishes_early
#
#Scenario: A non-DAS learner, learner finishes early
#	Given the current collection period is R02
#	And planned course duration is 15 months
#	And the following course information:
#	| AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | TotalNegotiatedPrice | CompletionStatus |
#	| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 18750                | completed	    |
#
#	And the following contract type 2 On Programme earnings are provided:
#	| PriceEpisodeIdentifier | Delivery Period	| TransactionType | Amount |
#	| p2                     | 1				| Learning (TT1)  | 1000   |
#	| p2                     | 2				| Completion (TT2)| 3750   |
#	| p2                     | 2				| Balancing (TT3) | 3000   |
#
#	When an earnings event is received
#	Then the payments due component will generate the following contract type 2 payments due:
#	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
#	| p2                     | 1				| Learning (TT1)	| 1000		|
#	| p2                     | 2				| Completion (TT2)	| 3750		|
#	| p2                     | 2				| Balancing (TT3)	| 3000		|
#
#
#@withdrawal
#
#Scenario: A non-DAS learner, learner withdraws after qualifying period
#	Given the current collection period is R06
#	And planned course duration is 12 months
#	And the following course information:
#	| AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | TotalNegotiatedPrice | CompletionStatus |
#	| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 15000                | withdrawn	    |
#
#	And the following contract type 2 On Programme earnings are provided:
#	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
#	| p1                     | 2				| Learning (TT1)	| 1000		|
#	| p1                     | 3				| Learning (TT1)	| 1000		|
#	| p1                     | 4				| Learning (TT1)	| 1000		|
#	| p1                     | 5				| Learning (TT1)	| 1000		|
#
#	When an earnings event is received
#	Then the payments due component will generate the following contract type 2 payments due:
#	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
#	| p1                     | 2				| Learning (TT1)	| 1000		|
#	| p1                     | 3				| Learning (TT1)	| 1000		|
#	| p1                     | 4				| Learning (TT1)	| 1000		|
#	| p1                     | 5				| Learning (TT1)	| 1000		|