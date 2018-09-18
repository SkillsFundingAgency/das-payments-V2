Feature: Provider earnings and payments where learner completes earlier than planned (3 months early) but missing previous submission

Background:

	Given the current processing period is 10

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the required payments component generates the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount | SfaContributionPercentage |
	| learnref1      | 10000 | p1                     | 9      | 10000 | Learning_1      | 1000   | 0.90000                   |
	| learnref1      | 10000 | p1                     | 10     | 10000 | Completion_2    | 3000   | 0.90000                   |
	| learnref1      | 10000 | p1                     | 10     | 10000 | Balancing_3     | 3000   | 0.90000                   |
	
@Non-DAS
@Completion_2
@Balancing_3
@FinishedEarly
@MissingSubmission
@CoInvested

Scenario: Contract Type 2 Learning payment

	When required payments event is received

	Then the payment source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period  | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 9       | 10000 | Learning_1      | CoInvestedSfa_2      | 900    |
	| learnref1      | 10000 | p1                     | 9       | 10000 | Learning_1      | CoInvestedEmployer_3 | 100    |

Scenario: Contract Type 2 completion payment

	When required payments event is received

	Then the payment source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 10     | 10000 | Completion_2    | CoInvestedSfa_2      | 2700   |
	| learnref1      | 10000 | p1                     | 10     | 10000 | Completion_2    | CoInvestedEmployer_3 | 300    |


Scenario: Contract Type 2 balancing payment

	When required payments event is received

	Then the payment source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 10     | 10000 | Balancing_3     | CoInvestedSfa_2      | 2700   |
	| learnref1      | 10000 | p1                     | 10     | 10000 | Balancing_3     | CoInvestedEmployer_3 | 300    |	