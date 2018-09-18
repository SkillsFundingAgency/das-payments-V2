Feature: R14 - Delayed completion, No OnProgram payment

Background:
	Given the current processing period is 14

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the payments due component generates the following contract type 2 payments due:	

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount | SfaContributionPercentage |
	| learnref1      | 10000 | p1                     | 1      | 10000 | Learning_1			| 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 2      | 10000 | Learning_1			| 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 3      | 10000 | Learning_1			| 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 4      | 10000 | Learning_1			| 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 5      | 10000 | Learning_1			| 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 6      | 10000 | Learning_1			| 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 7      | 10000 | Learning_1			| 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 8      | 10000 | Learning_1			| 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 9      | 10000 | Learning_1			| 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 10     | 10000 | Learning_1			| 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 11     | 10000 | Learning_1			| 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 12     | 10000 | Learning_1			| 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 14     | 10000 | Completion_2       | 3000   | 0.90000                   |

	And the following historical contract type 2 on programme payments exist:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount | SfaContributionPercentage |
	| learnref1      | 10000 | p1                     | 1      | 10000 | Learning_1			| 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 2      | 10000 | Learning_1			| 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 3      | 10000 | Learning_1			| 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 4      | 10000 | Learning_1			| 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 5      | 10000 | Learning_1			| 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 6      | 10000 | Learning_1			| 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 7      | 10000 | Learning_1			| 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 8      | 10000 | Learning_1			| 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 9      | 10000 | Learning_1			| 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 10     | 10000 | Learning_1			| 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 11     | 10000 | Learning_1			| 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 12     | 10000 | Learning_1			| 600    | 0.90000                   |



@Non-DAS
@LateCompletion
@Completion_2

Scenario Outline: Contract Type 2 completion payment

	When a payments due event is received

	Then the required payments component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   | SfaContributionPercentage |
	| learnref1      | 10000 | p1                     | 14     | 10000 | <transaction_type> | <amount> | 0.90000                   |
	
	Examples: 
	| transaction_type | amount |
	| Completion_2     | 1800   |