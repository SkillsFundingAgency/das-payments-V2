Feature: R14 - Delayed completion, No OnProgram payment

Background:
	Given the current processing period is 14

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the required payments component generates the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount | SfaContributionPercentage |
	| learnref1      | 10000 | p1                     | 14     | 10000 | Completion_2    | 1800   | 0.90000                   |

@Non-DAS
@LateCompletion
@Completion_2

Scenario: Contract Type 2 no On programme payments

	When required payments event is received

	Then the payment source component will not generate any contract type 2 transaction type Learning_1 coinvested payments

Scenario: Contract Type 2 completion payment

	When required payments event is received

	Then the payment source component will generate the following contract type 2 coinvested payments:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 14     | 10000 | Completion_2    | CoInvestedSfa_2      | 1620   |
	| learnref1      | 10000 | p1                     | 14     | 10000 | Completion_2    | CoInvestedEmployer_3 | 180    |

Scenario Outline: Contract Type 2 no balancing payment

	When required payments event is received

	Then the payment source component will not generate any contract type 2 transaction type Balancing_3 coinvested payments