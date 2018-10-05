Feature: non-DAS learner employed with a small employer, is fully funded for on programme and completion payments

Background:
	Given the current collection period is R13
	And a learner is undertaking a training with a training provider
	And the SFA contribution percentage is 100%
	And the required payments component generates the following contract type 2 payable earnings:
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
	When required payments event is received
	Then the payment source component will generate the following contract type 2 coinvested payments:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType | FundingSource			| Amount |
	| p1                     | 1				| Learning (TT1)  | CoInvestedSfa (FS2)		| 500    |
	| p1                     | 2				| Learning (TT1)  | CoInvestedSfa (FS2)		| 500    |
	| p1                     | 3				| Learning (TT1)  | CoInvestedSfa (FS2)		| 500    |
	| p1                     | 4				| Learning (TT1)  | CoInvestedSfa (FS2)		| 500    |
	| p1                     | 5				| Learning (TT1)  | CoInvestedSfa (FS2)		| 500    |
	| p1                     | 6				| Learning (TT1)  | CoInvestedSfa (FS2)		| 500    |
	| p1                     | 7				| Learning (TT1)  | CoInvestedSfa (FS2)		| 500    |
	| p1                     | 8				| Learning (TT1)  | CoInvestedSfa (FS2)		| 500    |
	| p1                     | 9				| Learning (TT1)  | CoInvestedSfa (FS2)		| 500    |
	| p1                     | 10				| Learning (TT1)  | CoInvestedSfa (FS2)		| 500    |
	| p1                     | 11				| Learning (TT1)  | CoInvestedSfa (FS2)		| 500    |
	| p1                     | 12				| Learning (TT1)  | CoInvestedSfa (FS2)		| 500    |
	| p1                     | 12				| Completion (TT2)| CoInvestedSfa (FS2)		| 1500   |

Scenario: AC5- Payment for a 16-18 non-DAS learner, employer is not small
	Given the SFA contribution percentage is 90%
	When required payments event is received
	Then the payment source component will generate the following contract type 2 coinvested payments:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType | FundingSource			| Amount |
	| p1                     | 1				| Learning (TT1)  | CoInvestedSfa (FS2)		| 450    |
	| p1                     | 2				| Learning (TT1)  | CoInvestedSfa (FS2)		| 450    |
	| p1                     | 3				| Learning (TT1)  | CoInvestedSfa (FS2)		| 450    |
	| p1                     | 4				| Learning (TT1)  | CoInvestedSfa (FS2)		| 450    |
	| p1                     | 5				| Learning (TT1)  | CoInvestedSfa (FS2)		| 450    |
	| p1                     | 6				| Learning (TT1)  | CoInvestedSfa (FS2)		| 450    |
	| p1                     | 7				| Learning (TT1)  | CoInvestedSfa (FS2)		| 450    |
	| p1                     | 8				| Learning (TT1)  | CoInvestedSfa (FS2)		| 450    |
	| p1                     | 9				| Learning (TT1)  | CoInvestedSfa (FS2)		| 450    |
	| p1                     | 10				| Learning (TT1)  | CoInvestedSfa (FS2)		| 450    |
	| p1                     | 11				| Learning (TT1)  | CoInvestedSfa (FS2)		| 450    |
	| p1                     | 12				| Learning (TT1)  | CoInvestedSfa (FS2)		| 450    |
	| p1                     | 12				| Completion (TT2)| CoInvestedSfa (FS2)		| 1350   |
	| p1                     | 1				| Learning (TT1)  | CoInvestedEmployer (FS3)| 50     |
	| p1                     | 2				| Learning (TT1)  | CoInvestedEmployer (FS3)| 50     |
	| p1                     | 3				| Learning (TT1)  | CoInvestedEmployer (FS3)| 50     |
	| p1                     | 4				| Learning (TT1)  | CoInvestedEmployer (FS3)| 50     |
	| p1                     | 5				| Learning (TT1)  | CoInvestedEmployer (FS3)| 50     |
	| p1                     | 6				| Learning (TT1)  | CoInvestedEmployer (FS3)| 50     |
	| p1                     | 7				| Learning (TT1)  | CoInvestedEmployer (FS3)| 50     |
	| p1                     | 8				| Learning (TT1)  | CoInvestedEmployer (FS3)| 50     |
	| p1                     | 9				| Learning (TT1)  | CoInvestedEmployer (FS3)| 50     |
	| p1                     | 10				| Learning (TT1)  | CoInvestedEmployer (FS3)| 50     |
	| p1                     | 11				| Learning (TT1)  | CoInvestedEmployer (FS3)| 50     |
	| p1                     | 12				| Learning (TT1)  | CoInvestedEmployer (FS3)| 50     |
	| p1                     | 12				| Completion (TT2)| CoInvestedEmployer (FS3)| 150    |