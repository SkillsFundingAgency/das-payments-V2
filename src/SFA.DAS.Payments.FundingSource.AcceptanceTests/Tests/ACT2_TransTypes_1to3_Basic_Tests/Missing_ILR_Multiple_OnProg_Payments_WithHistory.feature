﻿Feature: R06 - missing historical payments after R02

Background:
	Given the current processing period is 6

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the required payments component generates the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount | SfaContributionPercentage |
	| learnref1      | 10000 | p1                     | 3      | 10000 | Learning_1      | 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 4      | 10000 | Learning_1      | 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 5      | 10000 | Learning_1      | 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 6      | 10000 | Learning_1      | 600    | 0.90000                   |

@Non-DAS
@MissingSubmission
@Learning_1
@CoInvested

Scenario: Contract Type 2 Learning payment

	When required payments event is received

	Then the payment source component will generate the following contract type 2 transaction type Learning_1 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 3      | 10000 | Learning_1      | CoInvestedSfa_2      | 540    |
	| learnref1      | 10000 | p1                     | 3      | 10000 | Learning_1      | CoInvestedEmployer_3 | 60     |
	| learnref1      | 10000 | p1                     | 4      | 10000 | Learning_1      | CoInvestedSfa_2      | 540    |
	| learnref1      | 10000 | p1                     | 4      | 10000 | Learning_1      | CoInvestedEmployer_3 | 60     |
	| learnref1      | 10000 | p1                     | 5      | 10000 | Learning_1      | CoInvestedSfa_2      | 540    |
	| learnref1      | 10000 | p1                     | 5      | 10000 | Learning_1      | CoInvestedEmployer_3 | 60     |
	| learnref1      | 10000 | p1                     | 6      | 10000 | Learning_1      | CoInvestedSfa_2      | 540    |
	| learnref1      | 10000 | p1                     | 6      | 10000 | Learning_1      | CoInvestedEmployer_3 | 60     |


Scenario: Contract Type 2 no completion payment

	When required payments event is received

	Then the payment source component will not generate any contract type 2 transaction type Completion_2 coinvested payments


Scenario: Contract Type 2 no balancing payment

	When required payments event is received

	Then the payment source component will not generate any contract type 2 transaction type Balancing_3 coinvested payments