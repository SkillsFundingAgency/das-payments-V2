﻿Feature: Small Employers
#Non-DAS learner employed with a small employer, is fully funded for on programme and completion payments

Background:
	Given the current collection period is R13
	And the payments are for the current collection year
	And a learner is undertaking a training with a training provider
	And the SFA contribution percentage is 100%
	And the earning events component generates the following contract type 2 earnings:	
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p1                     | 1				| Learning (TT1)	| 500		|
	| p1                     | 2				| Learning (TT1)	| 500		|
	| p1                     | 3				| Learning (TT1)	| 500		|
	| p1                     | 4				| Learning (TT1)	| 500		|
	| p1                     | 5				| Learning (TT1)	| 500		|
	| p1                     | 6				| Learning (TT1)	| 500		|
	| p1                     | 7				| Learning (TT1)	| 500		|
	| p1                     | 8				| Learning (TT1)	| 500		|
	| p1                     | 9				| Learning (TT1)	| 500		|
	| p1                     | 10				| Learning (TT1)	| 500		|
	| p1                     | 11				| Learning (TT1)	| 500		|
	| p1                     | 12				| Learning (TT1)	| 500		|
	| p1                     | 12				| Completion (TT2)	| 1500		|

@SmallEmployerNonDas

Scenario: AC1-Payment for a 16-18 non-DAS learner, small employer at start
	When an earning event is received
	Then the required payments component will only generate contract type 2 required payments
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p1                     | 1				| Learning (TT1)	| 500		|
	| p1                     | 2				| Learning (TT1)	| 500		|
	| p1                     | 3				| Learning (TT1)	| 500		|
	| p1                     | 4				| Learning (TT1)	| 500		|
	| p1                     | 5				| Learning (TT1)	| 500		|
	| p1                     | 6				| Learning (TT1)	| 500		|
	| p1                     | 7				| Learning (TT1)	| 500		|
	| p1                     | 8				| Learning (TT1)	| 500		|
	| p1                     | 9				| Learning (TT1)	| 500		|
	| p1                     | 10				| Learning (TT1)	| 500		|
	| p1                     | 11				| Learning (TT1)	| 500		|
	| p1                     | 12				| Learning (TT1)	| 500		|
	| p1                     | 12				| Completion (TT2)	| 1500		|

@SmallEmployerNonDas

Scenario: AC5- Payment for a 16-18 non-DAS learner, employer is not small
	Given the SFA contribution percentage is 90%
	When an earning event is received
	Then the required payments component will only generate contract type 2 required payments
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p1                     | 1				| Learning (TT1)	| 500		|
	| p1                     | 2				| Learning (TT1)	| 500		|
	| p1                     | 3				| Learning (TT1)	| 500		|
	| p1                     | 4				| Learning (TT1)	| 500		|
	| p1                     | 5				| Learning (TT1)	| 500		|
	| p1                     | 6				| Learning (TT1)	| 500		|
	| p1                     | 7				| Learning (TT1)	| 500		|
	| p1                     | 8				| Learning (TT1)	| 500		|
	| p1                     | 9				| Learning (TT1)	| 500		|
	| p1                     | 10				| Learning (TT1)	| 500		|
	| p1                     | 11				| Learning (TT1)	| 500		|
	| p1                     | 12				| Learning (TT1)	| 500		|
	| p1                     | 12				| Completion (TT2)	| 1500		|

#Additional scenario for historical payments
Scenario: AC1-Payment for a 16-18 non-DAS learner, small employer at start and historical payments exist

	Given the following historical contract type 2 payments exist:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p1                     | 1				| Learning (TT1)	| 500		|
	| p1                     | 2				| Learning (TT1)	| 500		|
	| p1                     | 3				| Learning (TT1)	| 500		|
	| p1                     | 4				| Learning (TT1)	| 500		|
	| p1                     | 5				| Learning (TT1)	| 500		|
	| p1                     | 6				| Learning (TT1)	| 500		|
	| p1                     | 7				| Learning (TT1)	| 500		|
	| p1                     | 8				| Learning (TT1)	| 500		|
	| p1                     | 9				| Learning (TT1)	| 500		|
	| p1                     | 10				| Learning (TT1)	| 500		|
	| p1                     | 11				| Learning (TT1)	| 500		|

	When an earning event is received
	Then the required payments component will only generate contract type 2 required payments
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p1                     | 12				| Learning (TT1)	| 500		|
	| p1                     | 12				| Completion (TT2)	| 1500		|