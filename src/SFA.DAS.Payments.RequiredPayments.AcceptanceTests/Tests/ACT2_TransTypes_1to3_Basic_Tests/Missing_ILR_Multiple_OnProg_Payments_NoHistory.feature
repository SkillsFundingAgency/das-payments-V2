Feature: R02 - First payment including previous month

Background:
	Given the current processing period is 2

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the payments due component generates the following contract type 2 payments due:	

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount | SfaContributionPercentage |
	| learnref1      | 10000 | p1                     | 1      | 10000 | Learning_1			| 600    | 0.90000                   |
	| learnref1      | 10000 | p1                     | 2      | 10000 | Learning_1			| 600    | 0.90000                   |

@Non-DAS
@Learning_1
@LateSubmission

Scenario Outline: Contract Type 2 On programme payments

	When a payments due event is received

	Then the required payments component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   | SfaContributionPercentage |
	| learnref1      | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount> | 0.90000                   |
	| learnref1      | 10000 | p1                     | 2      | 10000 | <transaction_type> | <amount> | 0.90000                   |
	
	Examples: 
	| transaction_type | amount |
	| Learning_1       | 600    |


Scenario Outline: Contract Type 2 no completion payment

	When a payments due event is received

	Then the required payments component will not generate any contract type 2 transaction type Completion_2 payable earnings


Scenario Outline: Contract Type 2 no balancing payment

	When a payments due event is received

	Then the required payments component will not generate any contract type 2 transaction type Balancing_3 payable earnings