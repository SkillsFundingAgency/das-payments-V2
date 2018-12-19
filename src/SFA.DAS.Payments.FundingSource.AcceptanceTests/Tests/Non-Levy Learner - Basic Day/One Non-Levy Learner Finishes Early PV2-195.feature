Feature: One Non-Levy Learner Finishes Early PV2-195
Provider earnings and payments where learner completes earlier than planned

Background:
	Given a learner is undertaking a training with a training provider
	And the SFA contribution percentage is 90%
	And the payments are for the current collection year

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
	| p2                     | 2				| Completion (TT2)| CoInvestedEmployer (FS3)	| 375    |
	| p2                     | 2				| Balancing (TT3) | CoInvestedSfa (FS2)		| 2700   |
	| p2                     | 2				| Balancing (TT3) | CoInvestedEmployer (FS3)	| 300    |

@NonDas_BasicDay
@finishes_early
@NoHistory

Scenario: A non-DAS learner, learner finishes early - no history
	Given the current collection period is R02

	And the required payments component generates the following contract type 2 payable earnings:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p2                     | 1				| Learning (TT1)	| 1000		|
	| p2                     | 2				| Completion (TT2)	| 3750		|
	| p2                     | 2				| Balancing (TT3)	| 3000		|

	When required payments event is received
	Then the payment source component will generate the following contract type 2 coinvested payments:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType | FundingSource			| Amount |
	| p2                     | 1				| Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| p2                     | 1				| Learning (TT1)  | CoInvestedEmployer (FS3)	| 100    |
	| p2                     | 2				| Completion (TT2)| CoInvestedSfa (FS2)		| 3375   |
	| p2                     | 2				| Completion (TT2)| CoInvestedEmployer (FS3)	| 375    |
	| p2                     | 2				| Balancing (TT3) | CoInvestedSfa (FS2)		| 2700   |
	| p2                     | 2				| Balancing (TT3) | CoInvestedEmployer (FS3)	| 300    |


@withdrawal
@PartialHistory

Scenario: A non-DAS learner, learner withdraws after qualifying period - partial history
	Given the current collection period is R06

	And the required payments component generates the following contract type 2 payable earnings:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p2                     | 4				| Learning (TT1)	| 1000		|
	| p2                     | 5				| Learning (TT1)	| 1000		|

	When required payments event is received
	Then the payment source component will generate the following contract type 2 coinvested payments:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType | FundingSource			| Amount |
	| p2                     | 4				| Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| p2                     | 4				| Learning (TT1)  | CoInvestedEmployer (FS3)	| 100    |
	| p2                     | 5				| Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| p2                     | 5				| Learning (TT1)  | CoInvestedEmployer (FS3)	| 100    |