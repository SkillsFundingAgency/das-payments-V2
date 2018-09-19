Feature: R13 - Continuing after planned end date, No OnProgram payment

Background:
	Given the current processing period is 13

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
@FinishingLate
@NoPayment

Scenario: Contract Type 2 no On programme payments

	When a payments due event is received

	Then the required payments component will not generate any contract type 2 transaction type Learning_1 payable earnings

Scenario: Contract Type 2 no completion payment

	When a payments due event is received

	Then the required payments component will not generate any contract type 2 transaction type Completion_2 payable earnings

Scenario: Contract Type 2 no balancing payment

	When a payments due event is received

	Then the required payments component will not generate any contract type 2 transaction type Balancing_3 payable earnings