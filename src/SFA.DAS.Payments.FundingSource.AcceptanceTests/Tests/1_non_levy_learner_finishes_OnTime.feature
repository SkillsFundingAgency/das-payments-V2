Feature: Non-Levy learner - Basic Day
@NonDas_BasicDay
Background:
	Given the current collection period is R03
	And a learner is undertaking a training with a training provider
	And the SFA contribution percentage is 90%
	And the required payments component generates the following contract type 2 payable earnings:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p2                     | 1 				| Learning (TT1)	| 1000		|
	| p2                     | 2 				| Completion (TT2)	| 3000		|
	
	Scenario: 1_non_levy_learner_finishes_OnTime
	When required payments event is received
	Then the payment source component will generate the following contract type 2 coinvested payments:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType | FundingSource			| Amount |
	| p2                     | 1				| Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| p2                     | 1				| Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| p2                     | 2				| Completion (TT2)| CoInvestedSfa (FS2)		| 2700   |
	| p2                     | 2				| Completion (TT2)| CoInvestedEmployer (FS3)| 300    |