﻿Feature: No Payment - Continuing after planned end date
R13 - Continuing after planned end date, No OnProgram payment

Background:
	Given the current processing period is 13
	And the payments are for the current collection year
	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000
	And the SFA contribution percentage is 90%
	And the earning events component generates the following contract type 2 earnings:	
	| PriceEpisodeIdentifier | Delivery Period | TransactionType | Amount |
	| p1                     | 1               | Learning (TT1)  | 600    |
	| p1                     | 2               | Learning (TT1)  | 600    |
	| p1                     | 3               | Learning (TT1)  | 600    |
	| p1                     | 4               | Learning (TT1)  | 600    |
	| p1                     | 5               | Learning (TT1)  | 600    |
	| p1                     | 6               | Learning (TT1)  | 600    |
	| p1                     | 7               | Learning (TT1)  | 600    |
	| p1                     | 8               | Learning (TT1)  | 600    |
	| p1                     | 9               | Learning (TT1)  | 600    |
	| p1                     | 10              | Learning (TT1)  | 600    |
	| p1                     | 11              | Learning (TT1)  | 600    |
	| p1                     | 12              | Learning (TT1)  | 600    |

	And the following historical contract type 2 payments exist:
	| PriceEpisodeIdentifier | Delivery Period | TransactionType | Amount |
	| p1                     | 1               | Learning (TT1)  | 600    |
	| p1                     | 2               | Learning (TT1)  | 600    |
	| p1                     | 3               | Learning (TT1)  | 600    |
	| p1                     | 4               | Learning (TT1)  | 600    |
	| p1                     | 5               | Learning (TT1)  | 600    |
	| p1                     | 6               | Learning (TT1)  | 600    |
	| p1                     | 7               | Learning (TT1)  | 600    |
	| p1                     | 8               | Learning (TT1)  | 600    |
	| p1                     | 9               | Learning (TT1)  | 600    |
	| p1                     | 10              | Learning (TT1)  | 600    |
	| p1                     | 11              | Learning (TT1)  | 600    |
	| p1                     | 12              | Learning (TT1)  | 600    |

@Non-DAS
@NoPayment

Scenario: Contract Type 2 no On Programme Learning payments
	When an earning event is received
	Then the required payments component will not generate any contract type 2 Learning (TT1) payable earnings

Scenario: Contract Type 2 no On Programme Completion payment
	When an earning event is received
	Then the required payments component will not generate any contract type 2 Completion (TT2) payable earnings

Scenario: Contract Type 2 no On Programme Balancing payment
	When an earning event is received
	Then the required payments component will not generate any contract type 2 Balancing (TT3) payable earnings