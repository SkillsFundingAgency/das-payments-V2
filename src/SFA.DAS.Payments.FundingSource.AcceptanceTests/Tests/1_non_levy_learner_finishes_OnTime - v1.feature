Feature: Non-Levy learner - Basic Day - v1
@NonDas_BasicDay
Background:
	Given the current collection period is R13
	And a learner is undertaking a training with a training provider
	And the SFA contribution percentage is 90%
	And the required payments component generates the following contract type 2 payable earnings:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p1                     | 1				| Learning (TT1)	| 1000		|
	| p1                     | 2				| Learning (TT1)	| 1000		|
	| p1                     | 3				| Learning (TT1)	| 1000		|
	| p1                     | 4				| Learning (TT1)	| 1000		|
	| p1                     | 5				| Learning (TT1)	| 1000		|
	| p1                     | 6				| Learning (TT1)	| 1000		|
	| p1                     | 7				| Learning (TT1)	| 1000		|
	| p1                     | 8				| Learning (TT1)	| 1000		|
	| p1                     | 9				| Learning (TT1)	| 1000		|
	| p1                     | 10				| Learning (TT1)	| 1000		|
	| p1                     | 11				| Learning (TT1)	| 1000		|
	| p1                     | 12				| Learning (TT1)	| 1000		|
	| p1                     | 12				| Completion (TT2)	| 3000		|
	
	Scenario: 1_non_levy_learner_finishes_OnTime
	When required payments event is received
	Then the payment source component will generate the following contract type 2 coinvested payments:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType | FundingSource			| Amount |
	| p1                     | 1				| Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| p1                     | 2				| Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| p1                     | 3				| Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| p1                     | 4				| Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| p1                     | 5				| Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| p1                     | 6				| Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| p1                     | 7				| Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| p1                     | 8				| Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| p1                     | 9				| Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| p1                     | 10				| Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| p1                     | 11				| Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| p1                     | 12				| Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| p1                     | 12				| Completion (TT2)| CoInvestedSfa (FS2)		| 2700   |
	| p1                     | 1				| Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| p1                     | 2				| Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| p1                     | 3				| Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| p1                     | 4				| Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| p1                     | 5				| Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| p1                     | 6				| Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| p1                     | 7				| Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| p1                     | 8				| Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| p1                     | 9				| Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| p1                     | 10				| Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| p1                     | 11				| Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| p1                     | 12				| Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| p1                     | 12				| Completion (TT2)| CoInvestedEmployer (FS3)| 300    |