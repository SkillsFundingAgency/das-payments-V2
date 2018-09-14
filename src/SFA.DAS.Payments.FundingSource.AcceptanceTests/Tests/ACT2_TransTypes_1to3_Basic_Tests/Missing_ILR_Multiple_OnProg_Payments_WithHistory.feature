Feature: R06 - missing historical payments after R02

Background:
	Given the current processing period is 6

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the payments due component generates the following contract type 2 payable earnings:
	| PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount | SfaContributionPercentage |
	| p1                     | 3      | 10000 | Learning_1      | 600    | 0.90000                   |
	| p1                     | 4      | 10000 | Learning_1      | 600    | 0.90000                   |
	| p1                     | 5      | 10000 | Learning_1      | 600    | 0.90000                   |
	| p1                     | 6      | 10000 | Learning_1      | 600    | 0.90000                   |

@Non-DAS
@MissingSubmission
@Learning_1
@CoInvested

Scenario: Contract Type 2 Learning payment

	When MASH is received

	Then the payment source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 3      | 10000 | Learning_1      | CoInvestedSfa_2      | 540    |
	| learnref1      | 10000 | p1                     | 3      | 10000 | Learning_1      | CoInvestedEmployer_3 | 60     |
	| learnref1      | 10000 | p1                     | 4      | 10000 | Learning_1      | CoInvestedSfa_2      | 540    |
	| learnref1      | 10000 | p1                     | 4      | 10000 | Learning_1      | CoInvestedEmployer_3 | 60     |
	| learnref1      | 10000 | p1                     | 5      | 10000 | Learning_1      | CoInvestedSfa_2      | 540    |
	| learnref1      | 10000 | p1                     | 5      | 10000 | Learning_1      | CoInvestedEmployer_3 | 60     |
	| learnref1      | 10000 | p1                     | 6      | 10000 | Learning_1      | CoInvestedSfa_2      | 540    |
	| learnref1      | 10000 | p1                     | 6      | 10000 | Learning_1      | CoInvestedEmployer_3 | 60     |