Feature: Provider earnings and payments where learner completes earlier than planned (3 months early)

Background:

	Given the current processing period is 10

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the payments due component generates the following contract type 2 payable earnings:
	| PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount |
	| p1                     | 10     | 10000 | Completion_2    | 3000   |
	| p1                     | 10     | 10000 | Balancing_3     | 3000   |
	
@Non-DAS
@Completion_2
@Balancing_3
@FinishedEarly
@CoInvested

Scenario: Contract Type 2 completion payment

	When MASH is received

	Then the payment source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 10     | 10000 | Completion_2    | CoInvestedSfa_2      | 2700   |
	| learnref1      | 10000 | p1                     | 10     | 10000 | Completion_2    | CoInvestedEmployer_3 | 300    |


Scenario: Contract Type 2 balancing payment

	When MASH is received

	Then the payment source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 10     | 10000 | Balancing_3     | CoInvestedSfa_2      | 2700   |
	| learnref1      | 10000 | p1                     | 10     | 10000 | Balancing_3     | CoInvestedEmployer_3 | 300    |	