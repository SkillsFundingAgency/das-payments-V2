Feature: Basic Day - 2 learners
	Non-Levy - 1 finishes early and 1 finishes late

Background:
	Given the SFA contribution percentage is 90%
	And following learners are undertaking training with a training provider
	| LearnerId | 
	| L1		|
	| L2		|

@NonDas_BasicDay
@Finishes_Early

Scenario: 2_non_levy_learners_1finishes_Early
	Given the current collection period is R02
	And the required payments component generates the following contract type 2 payable earnings:
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| L1		| p2                     | 2				| Completion (TT2)	| 3750   |
	| L1		| p2                     | 2				| Balancing (TT3)	| 3000   |
	When required payments event is received
	Then the payment source component will generate the following contract type 2 coinvested payments:
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType | FundingSource			| Amount |
	| L1		| p2                     | 2				| Completion (TT2)| CoInvestedSfa (FS2)		| 3375   |
	| L1		| p2                     | 2				| Completion (TT2)| CoInvestedEmployer (FS3)| 375    |
	| L1		| p2                     | 2				| Balancing (TT3) | CoInvestedSfa (FS2)		| 2700   |
	| L1		| p2                     | 2				| Balancing (TT3) | CoInvestedEmployer (FS3)| 300    |

@NonDas_BasicDay
@Finishes_Late

Scenario: 2_non_levy_learners_1finishes_Late
	Given the current collection period is R05
	And the required payments component generates the following contract type 2 payable earnings:
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount |
	| L2		| p4                     | 5				| Completion (TT2)	| 3000   |

	When required payments event is received
	Then the payment source component will generate the following contract type 2 coinvested payments:
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType | FundingSource			| Amount |
	| L2		| p4                     | 5				| Completion (TT2)| CoInvestedSfa (FS2)		| 2700   |
	| L2		| p4                     | 5				| Completion (TT2)| CoInvestedEmployer (FS3)| 300    |
	

@NonDas_BasicDay
@Finishes_Late
@NoHistory

Scenario: 2_non_levy_learners_1finishes_Late - No history - Both learners together
	Given the current collection period is R05
	And the required payments component generates the following contract type 2 payable earnings:
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount |
	| L1		| p2                     | 1				| Learning (TT1)	| 1000   |
	| L1		| p2                     | 2				| Completion (TT2)  | 3750   |
	| L1		| p2                     | 2				| Balancing (TT3)   | 3000   |
	| L2		| p4                     | 1				| Learning (TT1)	| 1000   |
	| L2		| p4                     | 5				| Completion (TT2)	| 3000   |

	When required payments event is received
	Then the payment source component will generate the following contract type 2 coinvested payments:
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType | FundingSource			| Amount |
	| L1		| p2                     | 1				| Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| L1		| p2                     | 1				| Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| L1		| p2                     | 2				| Completion (TT2)| CoInvestedSfa (FS2)		| 3375   |
	| L1		| p2                     | 2				| Completion (TT2)| CoInvestedEmployer (FS3)| 375    |
	| L1		| p2                     | 2				| Balancing (TT3) | CoInvestedSfa (FS2)		| 2700   |
	| L1		| p2                     | 2				| Balancing (TT3) | CoInvestedEmployer (FS3)| 300    |
	| L2		| p4                     | 1				| Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| L2		| p4                     | 1				| Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| L2		| p4                     | 5				| Completion (TT2)| CoInvestedSfa (FS2)		| 2700   |
	| L2		| p4                     | 5				| Completion (TT2)| CoInvestedEmployer (FS3)| 300    |