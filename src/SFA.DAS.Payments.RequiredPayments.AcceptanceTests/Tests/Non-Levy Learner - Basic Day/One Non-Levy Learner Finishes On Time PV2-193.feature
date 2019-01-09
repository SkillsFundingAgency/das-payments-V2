﻿Feature: One Non-Levy Learner Finishes On Time PV2-193

Background:
	Given the current collection period is R03
	And the payments are for the current collection year
	And a learner is undertaking a training with a training provider
	And the SFA contribution percentage is 90%

	And the earning events component generates the following contract type 2 earnings:	
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p2                     | 1 				| Learning (TT1)	| 1000		|
	| p2                     | 2 				| Completion (TT2)	| 3000		|

@NonDas_BasicDay
@OnTime
Scenario: 1_non_levy_learner_finishes_OnTime
	Given the following historical contract type 2 payments exist:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p2                     | 1				| Learning (TT1)	| 1000		|

	When an earning event is received
	Then the required payments component will only generate contract type 2 required payments
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p2                     | 2 				| Completion (TT2)	| 3000		|

@NonDas_BasicDay
@OnTime
@NoHistory

Scenario: 1_non_levy_learner_finishes_OnTime - No history
	When an earning event is received
	Then the required payments component will only generate contract type 2 required payments
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p2                     | 1 				| Learning (TT1)	| 1000		|
	| p2                     | 2 				| Completion (TT2)	| 3000		|
