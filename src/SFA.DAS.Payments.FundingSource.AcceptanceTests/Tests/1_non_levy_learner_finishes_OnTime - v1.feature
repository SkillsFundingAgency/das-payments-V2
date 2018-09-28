Feature: Non-Levy learner - Basic Day - v1
@NonDas_BasicDay
Background:
	Given the current collection period is R13
	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000
	And the SFA contribution percentage is 90%
	And the required payments component generates the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Delivery Period | ULN   | TransactionType   | Amount	|
	| learnref1      | 10000 | p1                     | 1				| 10000 | Learning (TT1)	| 1000		|
	| learnref1      | 10000 | p1                     | 2				| 10000 | Learning (TT1)	| 1000		|
	| learnref1      | 10000 | p1                     | 3				| 10000 | Learning (TT1)	| 1000		|
	| learnref1      | 10000 | p1                     | 4				| 10000 | Learning (TT1)	| 1000		|
	| learnref1      | 10000 | p1                     | 5				| 10000 | Learning (TT1)	| 1000		|
	| learnref1      | 10000 | p1                     | 6				| 10000 | Learning (TT1)	| 1000		|
	| learnref1      | 10000 | p1                     | 7				| 10000 | Learning (TT1)	| 1000		|
	| learnref1      | 10000 | p1                     | 8				| 10000 | Learning (TT1)	| 1000		|
	| learnref1      | 10000 | p1                     | 9				| 10000 | Learning (TT1)	| 1000		|
	| learnref1      | 10000 | p1                     | 10				| 10000 | Learning (TT1)	| 1000		|
	| learnref1      | 10000 | p1                     | 11				| 10000 | Learning (TT1)	| 1000		|
	| learnref1      | 10000 | p1                     | 12				| 10000 | Learning (TT1)	| 1000		|
	| learnref1      | 10000 | p1                     | 12				| 10000 | Completion (TT2)	| 3000		|
	
	Scenario: 1_non_levy_learner_finishes_OnTime
	When required payments event is received
	Then the payment source component will generate the following contract type 2 coinvested payments:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Delivery Period | ULN   | TransactionType | FundingSource			| Amount |
	| learnref1      | 10000 | p1                     | 1				| 10000 | Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| learnref1      | 10000 | p1                     | 2				| 10000 | Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| learnref1      | 10000 | p1                     | 3				| 10000 | Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| learnref1      | 10000 | p1                     | 4				| 10000 | Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| learnref1      | 10000 | p1                     | 5				| 10000 | Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| learnref1      | 10000 | p1                     | 6				| 10000 | Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| learnref1      | 10000 | p1                     | 7				| 10000 | Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| learnref1      | 10000 | p1                     | 8				| 10000 | Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| learnref1      | 10000 | p1                     | 9				| 10000 | Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| learnref1      | 10000 | p1                     | 10				| 10000 | Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| learnref1      | 10000 | p1                     | 11				| 10000 | Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| learnref1      | 10000 | p1                     | 12				| 10000 | Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| learnref1      | 10000 | p1                     | 12				| 10000 | Completion (TT2)| CoInvestedSfa (FS2)		| 2700   |
	| learnref1      | 10000 | p1                     | 1				| 10000 | Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| learnref1      | 10000 | p1                     | 2				| 10000 | Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| learnref1      | 10000 | p1                     | 3				| 10000 | Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| learnref1      | 10000 | p1                     | 4				| 10000 | Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| learnref1      | 10000 | p1                     | 5				| 10000 | Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| learnref1      | 10000 | p1                     | 6				| 10000 | Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| learnref1      | 10000 | p1                     | 7				| 10000 | Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| learnref1      | 10000 | p1                     | 8				| 10000 | Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| learnref1      | 10000 | p1                     | 9				| 10000 | Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| learnref1      | 10000 | p1                     | 10				| 10000 | Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| learnref1      | 10000 | p1                     | 11				| 10000 | Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| learnref1      | 10000 | p1                     | 12				| 10000 | Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| learnref1      | 10000 | p1                     | 12				| 10000 | Completion (TT2)| CoInvestedEmployer (FS3)| 300    |