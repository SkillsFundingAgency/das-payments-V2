Feature: R13 - Delayed completion, No OnProgram payment

Background:
	Given the current processing period is 13

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

@Non-DAS
@NoPayment

Scenario: Contract Type 2 no payment

	When MASH is received

	Then the payment source component will generate no contract type 2 coinvested payments