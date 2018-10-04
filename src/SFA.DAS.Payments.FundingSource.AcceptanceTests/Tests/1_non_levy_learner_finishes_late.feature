Feature: Provider earnings and payments where learner completes later than planned
Background:
	Given a learner is undertaking a training with a training provider
	And the SFA contribution percentage is 90%

@NonDas_BasicDay
@finishes_late

@Completion (TT2)
@CoInvested

Scenario: A non-DAS learner, learner finishes late
	Given the current collection period is R05

	And the required payments component generates the following contract type 2 payable earnings:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p2                     | 5				| Completion (TT2)	| 3000		|

	When required payments event is received
	Then the payment source component will generate the following contract type 2 coinvested payments:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType | FundingSource			| Amount |
	| p2                     | 5				| Completion (TT2)| CoInvestedSfa (FS2)		| 2700   |
	| p2                     | 5				| Completion (TT2)| CoInvestedEmploer (FS3)	| 300    |

@NonDas_BasicDay
@finishes_late
@withdrawal
@NoCompletionPayment

Scenario: A non-DAS learner, learner withdraws after planned end date
	Given the current collection period is R05
	And the required payments component generated no contract type 2 payable earnings
	When no required payments event is received
	Then the payment source component will not generate any contract type 2 coinvested payments


@NonDas_BasicDay
@finishes_late
@Learning (TT1)
@Completion (TT2)
@CoInvested

Scenario: A non-DAS learner, learner finishes late - no history
	Given the current collection period is R05

	And the required payments component generates the following contract type 2 payable earnings:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p2                     | 1				| Learning (TT1)	| 1000		|
	| p2                     | 5				| Completion (TT2)	| 3000		|

	When required payments event is received
	Then the payment source component will generate the following contract type 2 coinvested payments:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType | FundingSource			| Amount |
	| p2                     | 1				| Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| p2                     | 1				| Learning (TT1)  | CoInvestedEmploer (FS3)	| 100    |
	| p2                     | 5				| Completion (TT2)| CoInvestedSfa (FS2)		| 2700   |
	| p2                     | 5				| Completion (TT2)| CoInvestedEmploer (FS3)	| 300    |