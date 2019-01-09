﻿Feature: One Non-Levy Learner Finishes Early PV2-195
Provider earnings and payments where learner completes earlier than planned

Background:
	Given a learner is undertaking a training with a training provider
	And the SFA contribution percentage is 90%

@NonDas_BasicDay
@finishes_early

Scenario: A non-DAS learner, learner finishes early
	Given the current collection period is R02
	And the payments are for the current collection year

	And the earning events component generates the following contract type 2 earnings:	
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p2                     | 1				| Learning (TT1)	| 1000		|
	| p2                     | 2				| Completion (TT2)	| 3750		|
	| p2                     | 2				| Balancing (TT3)	| 3000		|

	And the following historical contract type 2 payments exist:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p2                     | 1				| Learning (TT1)	| 1000		|

	When an earning event is received
	Then the required payments component will only generate contract type 2 required payments
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p2                     | 2				| Completion (TT2)	| 3750		|
	| p2                     | 2				| Balancing (TT3)	| 3000		|


@NonDas_BasicDay
@finishes_early
@NoHistory

Scenario: A non-DAS learner, learner finishes early - no history
	Given the current collection period is R02
	And the payments are for the current collection year

	And the earning events component generates the following contract type 2 earnings:	
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p2                     | 1				| Learning (TT1)	| 1000		|
	| p2                     | 2				| Completion (TT2)	| 3750		|
	| p2                     | 2				| Balancing (TT3)	| 3000		|

	When an earning event is received
	Then the required payments component will only generate contract type 2 required payments
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p2                     | 1				| Learning (TT1)	| 1000		|
	| p2                     | 2				| Completion (TT2)	| 3750		|
	| p2                     | 2				| Balancing (TT3)	| 3000		|

@withdrawal

Scenario: A non-DAS learner, learner withdraws after qualifying period
	Given the current collection period is R06
	And the payments are for the current collection year

	And the earning events component generates the following contract type 2 earnings:	
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p1                     | 2				| Learning (TT1)	| 1000		|
	| p1                     | 3				| Learning (TT1)	| 1000		|
	| p1                     | 4				| Learning (TT1)	| 1000		|
	| p1                     | 5				| Learning (TT1)	| 1000		|

	And the following historical contract type 2 payments exist:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p1                     | 2				| Learning (TT1)	| 1000		|
	| p1                     | 3				| Learning (TT1)	| 1000		|
	| p1                     | 4				| Learning (TT1)	| 1000		|
	| p1                     | 5				| Learning (TT1)	| 1000		|

	When an earning event is received
	Then the required payments component will not generate any contract type 2 payable earnings

@withdrawal
@PartialHistory

Scenario: A non-DAS learner, learner withdraws after qualifying period - partial history
	Given the current collection period is R06
	And the payments are for the current collection year

	And the earning events component generates the following contract type 2 earnings:	
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p1                     | 2				| Learning (TT1)	| 1000		|
	| p1                     | 3				| Learning (TT1)	| 1000		|
	| p1                     | 4				| Learning (TT1)	| 1000		|
	| p1                     | 5				| Learning (TT1)	| 1000		|

	And the following historical contract type 2 payments exist:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p1                     | 2				| Learning (TT1)	| 1000		|
	| p1                     | 3				| Learning (TT1)	| 1000		|

	When an earning event is received
	Then the required payments component will only generate contract type 2 required payments
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p1                     | 4				| Learning (TT1)	| 1000		|
	| p1                     | 5				| Learning (TT1)	| 1000		|