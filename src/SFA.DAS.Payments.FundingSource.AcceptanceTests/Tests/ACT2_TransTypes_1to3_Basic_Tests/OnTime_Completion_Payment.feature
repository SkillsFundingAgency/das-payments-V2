Feature: R13 - On time completion with full history

Background:
	Given the current processing period is 13

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the payments due component generates the following contract type 2 payable earnings:

	| PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount | SfaContributionPercentage |
	| p1                     | 13     | 10000 | Completion_2    | 1800   | 0.90000                   |

@Non-DAS
@Completion_2
@CoInvested

Scenario: Contract Type 2 completion payment

	When a payable earning event is received

	Then the funding source component will generate the following contract type 2 coinvested payments:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 13     | 10000 | Completion_2    | CoInvestedSfa_2      | 1620   |
	| learnref1      | 10000 | p1                     | 13     | 10000 | Completion_2    | CoInvestedEmployer_3 | 180    |