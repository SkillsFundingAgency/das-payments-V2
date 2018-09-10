#WARNING: This test needs reviewing
#what is the qualified period for withdrawal, will we not pay anything if they withdraw earlier

Feature: A non-DAS learner, learner withdraws before qualifying period
#Learner withdraws before 42 days and shouldn't be paid for 1st month


Background:
	Given the current processing period is 2

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the payments due component generates the following contract type 2 payable earnings:
	| PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount |
	| p1                     | 2      | 10000 | 1               | -1000  |

@Non-DAS
@Learner_finishes_early
@Withdrawal
@minimum_additional
@review
@refund

Scenario: Contract Type 2 Learning payment

	When MASH is received

	Then the payment source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 2      | 10000 | Learning_1      | CoInvestedSfa_2      | -900   |
	| learnref1      | 10000 | p1                     | 2      | 10000 | Learning_1      | CoInvestedEmployer_3 | -100   |
