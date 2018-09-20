Feature: R02 - First payment including previous month

Background:
	Given the current processing period is 2

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the SFA contribution percentage is "90%"

	And the required payments component generates the following contract type 2 payable earnings:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount |
	| learnref1      | 10000 | p1                     | 1      | 10000 | Learning (TT1)  | 600    |
	| learnref1      | 10000 | p1                     | 2      | 10000 | Learning (TT1)  | 600    |

@Non-DAS
@Learning (TT1)
@LateSubmission
@CoInvested

Scenario: Contract Type 2 Learning payment

	When required payments event is received

	Then the payment source component will generate the following contract type 2 Learning (TT1) coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource			| Amount |
	| learnref1      | 10000 | p1                     | 1      | 10000 | Learning (TT1)  | CoInvestedSfa (FS2)		| 540    |
	| learnref1      | 10000 | p1                     | 1      | 10000 | Learning (TT1)  | CoInvestedEmployer (FS3)	| 60     |
	| learnref1      | 10000 | p1                     | 2      | 10000 | Learning (TT1)  | CoInvestedSfa (FS2)		| 540    |
	| learnref1      | 10000 | p1                     | 2      | 10000 | Learning (TT1)  | CoInvestedEmployer (FS3)	| 60     |


Scenario: Contract Type 2 no On Programme Completion payment

	When required payments event is received

	Then the payment source component will not generate any contract type 2 Completion (TT2) coinvested payments


Scenario: Contract Type 2 no On Programme Balancing payment

	When required payments event is received

	Then the payment source component will not generate any contract type 2 Balancing (TT3) coinvested payments
