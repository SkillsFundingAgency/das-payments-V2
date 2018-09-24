Feature: R13 - Continuing after planned end date, No OnProgram payment

Background:
	Given the current processing period is 13

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the SFA contribution percentage is "90%"

	And the required payments component generates no contract type 2 payable earnings

@Non-DAS
@FinishingLate
@NoPayment

Scenario: Contract Type 2 no On Programme Learning payments

	When no required payments event is received

	Then the payment source component will not generate any contract type 2 Learning (TT1) coinvested payments

Scenario: Contract Type 2 no On Programme Completion payment

	When no required payments event is received

	Then the payment source component will not generate any contract type 2 Completion (TT2) coinvested payments

Scenario: Contract Type 2 no On Programme Balancing payment

	When no required payments event is received

	Then the payment source component will not generate any contract type 2 Balancing (TT3) coinvested payments