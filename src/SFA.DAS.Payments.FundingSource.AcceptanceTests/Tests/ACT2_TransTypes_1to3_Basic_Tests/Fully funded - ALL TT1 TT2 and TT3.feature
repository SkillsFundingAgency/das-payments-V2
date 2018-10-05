Feature: Fully funded - ALL TT1 TT2 and TT3

Background:

	Given the current processing period is 10
	And the payments are for the current collection year
	And a learner is undertaking a training with a training provider

	And the SFA contribution percentage is 100%

	And the required payments component generates the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Delivery Period	 | ULN   | TransactionType | Amount |
	| learnref1      | 10000 | p1                     | 9				 | 10000 | Learning (TT1)  | 1000   |
	| learnref1      | 10000 | p1                     | 10				 | 10000 | Completion (TT2)| 3000   |
	| learnref1      | 10000 | p1                     | 10				 | 10000 | Balancing (TT3) | 3000   |
	
@Non-DAS
@Learning (TT1)
@Completion (TT2)
@Balancing (TT3)
@FullyFunded

Scenario: Contract Type 2 Learning payment

	When required payments event is received

	Then the payment source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Delivery Period	  | ULN   | TransactionType | FundingSource			| Amount |
	| learnref1      | 10000 | p1                     | 9					| 10000 | Learning (TT1)  | CoInvestedSfa (FS2)		| 1000   |

Scenario: Contract Type 2 On Programme Completion payment

	When required payments event is received

	Then the payment source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Delivery Period	 | ULN   | TransactionType | FundingSource			| Amount |
	| learnref1      | 10000 | p1                     | 10				| 10000 | Completion (TT2)| CoInvestedSfa (FS2)		| 3000   |


Scenario: Contract Type 2 On Programme Balancing payment

	When required payments event is received

	Then the payment source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Delivery Period	 | ULN   | TransactionType | FundingSource			| Amount |
	| learnref1      | 10000 | p1                     | 10				 | 10000 | Balancing (TT3) | CoInvestedSfa (FS2)		| 3000   |