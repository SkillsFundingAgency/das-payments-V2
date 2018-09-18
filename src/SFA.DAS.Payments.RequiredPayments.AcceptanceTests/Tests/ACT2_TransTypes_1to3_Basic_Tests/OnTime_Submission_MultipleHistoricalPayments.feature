Feature: R03 - with multiple historical payment

Background:
	Given the current processing period is 3

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the payments due component generates the following contract type 2 payments due:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount | SfaContributionPercentage |
	| learnref1      | 10000 | p1                     | 1      | 10000 | 1               | 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 2      | 10000 | 1               | 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 3      | 10000 | 1               | 600    | 0.90000                   |

	And the following historical contract type 2 on programme payments exist:   

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount | SfaContributionPercentage |
	| learnref1      | 10000 | p1                     | 1      | 10000 | Learning_1      | 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 2      | 10000 | Learning_1      | 600    | 0.90000                   |

@Non-DAS
@HistoricalPayments
@Learning_1

Scenario Outline: Contract Type 2 On programme payments

	When a payments due event is received

	Then the required payments component will generate the following contract type 2 payable earnings:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   | SfaContributionPercentage |
	| learnref1      | 10000 | p1                     | 3      | 10000 | <transaction_type> | <amount> | 0.90000                   |
	
	Examples: 
	| transaction_type | amount |
	| Learning_1       | 600    |