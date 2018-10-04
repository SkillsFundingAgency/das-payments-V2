Feature: Non-Levy learner - Basic Day - 2 learners

Background:
	Given the current collection period is R03
	And the SFA contribution percentage is 90%
	And following learners are undertaking training with a training provider
	| LearnerId | 
	| L1		|
	| L2		|

@NonDas_BasicDay
@OnTime
Scenario: 2_non_levy_learner_finishes_OnTime
	Given the required payments component generates the following contract type 2 payable earnings:
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| L1		| p2                     | 2 				| Completion (TT2)	| 3000		|
	| L2		| p2                     | 2 				| Completion (TT2)	| 2400		|
	When required payments event is received
	Then the payment source component will generate the following contract type 2 coinvested payments:
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType | FundingSource			| Amount |
	| L1		| p2                     | 2				| Completion (TT2)| CoInvestedSfa (FS2)		| 2700   |
	| L1		| p2                     | 2				| Completion (TT2)| CoInvestedEmployer (FS3)| 300    |
	| L2		| p2                     | 2				| Completion (TT2)| CoInvestedSfa (FS2)		| 2160   |
	| L2		| p2                     | 2				| Completion (TT2)| CoInvestedEmployer (FS3)| 240    |

@NonDas_BasicDay
@OnTime
@NoHistory	
Scenario: 2_non_levy_learner_finishes_OnTime - No history
	Given the required payments component generates the following contract type 2 payable earnings:
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| L1		| p2                     | 1 				| Learning (TT1)	| 1000		|
	| L1		| p2                     | 2 				| Completion (TT2)	| 3000		|
	| L2		| p2                     | 1				| Learning (TT1)	| 800		|
	| L2		| p2                     | 2				| Completion (TT2)	| 2400		|
	When required payments event is received
	Then the payment source component will generate the following contract type 2 coinvested payments:
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType | FundingSource			| Amount |
	| L1		| p2                     | 1				| Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| L1		| p2                     | 1				| Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| L1		| p2                     | 2				| Completion (TT2)| CoInvestedSfa (FS2)		| 2700   |
	| L1		| p2                     | 2				| Completion (TT2)| CoInvestedEmployer (FS3)| 300    |
	| L2		| p2                     | 1				| Learning (TT1)  | CoInvestedSfa (FS2)		| 720    |
	| L2		| p2                     | 1				| Learning (TT1)  | CoInvestedEmployer (FS3)| 80     |
	| L2		| p2                     | 2				| Completion (TT2)| CoInvestedSfa (FS2)		| 2160   |
	| L2		| p2                     | 2				| Completion (TT2)| CoInvestedEmployer (FS3)| 240    |

@NonDas_BasicDay
@OnTime	
@PartialHistory
Scenario: 2_non_levy_learner_finishes_OnTime - 1 learner has history
	Given the required payments component generates the following contract type 2 payable earnings:
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| L1		| p2                     | 2 				| Completion (TT2)	| 3000		|
	| L2		| p2                     | 1				| Learning (TT1)	| 800		|
	| L2		| p2                     | 2				| Completion (TT2)	| 2400		|
	When required payments event is received
	Then the payment source component will generate the following contract type 2 coinvested payments:
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType | FundingSource			| Amount |
	| L1		| p2                     | 2				| Completion (TT2)| CoInvestedSfa (FS2)		| 2700   |
	| L1		| p2                     | 2				| Completion (TT2)| CoInvestedEmployer (FS3)| 300    |
	| L2		| p2                     | 1				| Learning (TT1)  | CoInvestedSfa (FS2)		| 720    |
	| L2		| p2                     | 1				| Learning (TT1)  | CoInvestedEmployer (FS3)| 80     |
	| L2		| p2                     | 2				| Completion (TT2)| CoInvestedSfa (FS2)		| 2160   |
	| L2		| p2                     | 2				| Completion (TT2)| CoInvestedEmployer (FS3)| 240    |