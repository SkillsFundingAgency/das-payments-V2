Feature: Basic Day
	Non-Levy - 2 learners - Both finishes on time

Background:
	Given the current collection period is R02
	And the SFA contribution percentage is 90%
	And following learners are undertaking training with a training provider
	| LearnerId | 
	| L1		|
	| L2		|

@NonLevy_BasicDay
@OnTime
Scenario: Completion for both
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

@NonLevy_BasicDay
@OnTime
@NoHistory	
Scenario: Learning and Completion for both - No history
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

@NonLevy_BasicDay
@OnTime	
@PartialHistory
Scenario: Learning for 1 and Completion for both - 1 learner has history
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