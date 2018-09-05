Feature: R01 - First payment, No previous payments

Background:
	Given the current processing period is 1

	And a learner with LearnRefNumber learnref3 and Uln 10000 undertaking training with training provider 10000

	And the payments due component generates the following contract type 2 payable earnings:
	| PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount |
	| p1                     | 1      | 10000 | 1               | 600    |

@Non-DAS
@No_Historical_Payments
@First_submission
@minimum_additional

Scenario: Contract Type 2 Learning payment

	When MASH is received

	Then the payment source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 1      | 10000 | Learning_1      | CoInvestedSfa_2      | 540    |
	| learnref1      | 10000 | p1                     | 1      | 10000 | Learning_1      | CoInvestedEmployer_3 | 60     |