Feature: Non-Levy learner - Basic Day - v2
@NonDas_BasicDay
Background:
	Given the current collection period is R02
	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000
	And the SFA contribution percentage is 90%
	And the required payments component generates the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Delivery Period | ULN   | TransactionType   | Amount	|
	| learnref1      | 10000 | p2                     | 1 				| 10000 | Learning (TT1)	| 1000		|
	| learnref1      | 10000 | p2                     | 1 				| 10000 | Completion (TT2)	| 3000		|
	
	Scenario: 1_non_levy_learner_finishes_OnTime
	When required payments event is received
	Then the payment source component will generate the following contract type 2 coinvested payments:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Delivery Period | ULN   | TransactionType | FundingSource			| Amount |
	| learnref1      | 10000 | p2                     | 1				| 10000 | Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| learnref1      | 10000 | p2                     | 1				| 10000 | Completion (TT2)| CoInvestedSfa (FS2)		| 2700   |
	| learnref1      | 10000 | p2                     | 1				| 10000 | Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |
	| learnref1      | 10000 | p2                     | 1				| 10000 | Completion (TT2)| CoInvestedEmployer (FS3)| 300    |