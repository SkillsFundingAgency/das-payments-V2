Feature: R13 - Continuing after planned end date, No OnProgram payment

Background:
	Given the current processing period is 13

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the required payments component generates no contract type 2 payable earnings

@Non-DAS
@FinishingLate
@NoPayment

Scenario: Contract Type 2 no On programme payments

	When no required payments event is received

	Then the payment source component will not generate any contract type 2 transaction type Learning_1 coinvested payments

Scenario Outline: Contract Type 2 no completion payment

	When no required payments event is received

	Then the payment source component will not generate any contract type 2 transaction type Completion_2 coinvested payments

Scenario Outline: Contract Type 2 no balancing payment

	When no required payments event is received

	Then the payment source component will not generate any contract type 2 transaction type Balancing_3 coinvested payments