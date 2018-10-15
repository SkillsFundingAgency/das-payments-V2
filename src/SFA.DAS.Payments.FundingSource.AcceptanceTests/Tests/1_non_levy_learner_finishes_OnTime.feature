Feature: Non-Levy learner - Basic Day

Background:
	Given the current collection period is R03
	And the payments are for the current collection year
	And a learner is undertaking a training with a training provider
	And the SFA contribution percentage is 90%

@NonDas_BasicDay
@OnTime
Scenario: 1_non_levy_learner_finishes_OnTime
	Given the required payments component generates the following contract type 2 payable earnings:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p2                     | 2 				| Completion (TT2)	| 3000		|
	When required payments event is received
	Then the payment source component will generate the following contract type 2 coinvested payments:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType | FundingSource			| Amount |
	| p2                     | 2				| Completion (TT2)| CoInvestedSfa (FS2)		| 2700   |
	| p2                     | 2				| Completion (TT2)| CoInvestedEmployer (FS3)| 300    |

@NonDas_BasicDay
@OnTime
@NoHistory	
Scenario: 1_non_levy_learner_finishes_OnTime - No history
	Given the required payments component generates the following contract type 2 payable earnings:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p2                     | 1 				| Learning (TT1)	| 1000		|
	| p2                     | 2 				| Completion (TT2)	| 3000		|
	When required payments event is received
	Then the payment source component will generate the following contract type 2 coinvested payments:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType | FundingSource			| Amount |
	| p2                     | 1				| Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| p2                     | 1				| Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| p2                     | 2				| Completion (TT2)| CoInvestedSfa (FS2)		| 2700   |
	| p2                     | 2				| Completion (TT2)| CoInvestedEmployer (FS3)| 300    |