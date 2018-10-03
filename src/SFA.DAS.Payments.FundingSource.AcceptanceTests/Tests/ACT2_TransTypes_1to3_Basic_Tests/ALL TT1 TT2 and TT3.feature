Feature: ALL TT1 TT2 and TT3

Background:

	Given the current processing period is 10
	And the payments are for the current collection year

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the SFA contribution percentage is 90%

	And the required payments component generates the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount |
	| learnref1      | 10000 | p1                     | 9      | 10000 | Learning (TT1)  | 1000   |
	| learnref1      | 10000 | p1                     | 10     | 10000 | Completion (TT2)| 3000   |
	| learnref1      | 10000 | p1                     | 10     | 10000 | Balancing (TT3) | 3000   |
	
@Non-DAS
@Learning (TT1)
@Completion (TT2)
@Balancing (TT3)
@CoInvested

Scenario: Contract Type 2 Learning payment

	When required payments event is received

	Then the payment source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period  | ULN   | TransactionType | FundingSource			| Amount |
	| learnref1      | 10000 | p1                     | 9       | 10000 | Learning (TT1)  | CoInvestedSfa (FS2)		| 900    |
	| learnref1      | 10000 | p1                     | 9       | 10000 | Learning (TT1)  | CoInvestedEmployer (FS3)| 100    |

Scenario: Contract Type 2 On Programme Completion payment

	When required payments event is received

	Then the payment source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource			| Amount |
	| learnref1      | 10000 | p1                     | 10     | 10000 | Completion (TT2)| CoInvestedSfa (FS2)		| 2700   |
	| learnref1      | 10000 | p1                     | 10     | 10000 | Completion (TT2)| CoInvestedEmployer (FS3) | 300    |


Scenario: Contract Type 2 On Programme Balancing payment

	When required payments event is received

	Then the payment source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource			| Amount |
	| learnref1      | 10000 | p1                     | 10     | 10000 | Balancing (TT3) | CoInvestedSfa (FS2)		| 2700   |
	| learnref1      | 10000 | p1                     | 10     | 10000 | Balancing (TT3) | CoInvestedEmployer (FS3) | 300    |	