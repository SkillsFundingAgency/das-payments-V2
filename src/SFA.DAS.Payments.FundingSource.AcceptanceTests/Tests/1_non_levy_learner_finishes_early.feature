Feature: Provider earnings and payments where learner completes earlier than planned
Background:
	Given a learner is undertaking a training with a training provider
	And the SFA contribution percentage is 90%

@NonDas_BasicDay
@finishes_early

Scenario: A non-DAS learner, learner finishes early
	Given the current collection period is R02

	And the required payments component generates the following contract type 2 payable earnings:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p2                     | 2				| Completion (TT2)	| 3750		|
	| p2                     | 2				| Balancing (TT3)	| 3000		|

	When required payments event is received
	Then the payment source component will generate the following contract type 2 coinvested payments:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType | FundingSource			| Amount |
	| p2                     | 2				| Completion (TT2)| CoInvestedSfa (FS2)		| 3375   |
	| p2                     | 2				| Balancing (TT3) | CoInvestedSfa (FS2)		| 2700   |
	| p2                     | 2				| Completion (TT2)| CoInvestedEmploer (FS3)	| 375    |
	| p2                     | 2				| Balancing (TT3) | CoInvestedEmploer (FS3)	| 300    |

@withdrawal

Scenario: A non-DAS learner, learner withdraws after qualifying period
	Given the current collection period is R06
	And the required payments component generates no contract type 2 payable earnings
	When no required payments event is received
	Then the payment source component will not generate any contract type 2 coinvested payments