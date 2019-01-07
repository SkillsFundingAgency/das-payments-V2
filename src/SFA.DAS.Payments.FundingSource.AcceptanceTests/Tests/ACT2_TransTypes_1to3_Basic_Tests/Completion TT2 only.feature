﻿Feature: Completion (TT2) only

Background:
	Given the current processing period is 1
	And the payments are for the current collection year
	And a learner is undertaking a training with a training provider
	
	And the SFA contribution percentage is 90%
	
	And the required payments component generates the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Delivery Period	 | ULN   | TransactionType | Amount |
	| learnref1      | 10000 | p1                     | 13				 | 10000 | Completion (TT2)| 1800   |

@Non-DAS
@Completion (TT2)
@CoInvested

Scenario: Contract Type 2 no On Programme Learning payments

	When required payments event is received

	Then the payment source component will not generate any contract type 2 Learning (TT1) coinvested payments


Scenario: Contract Type 2 On Programme Completion payment

	When required payments event is received

	Then the payment source component will generate the following contract type 2 coinvested payments:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Delivery Period	 | ULN   | TransactionType | FundingSource				| Amount |
	| learnref1      | 10000 | p1                     | 13				 | 10000 | Completion (TT2)| CoInvestedSfa (FS2)		| 1620   |
	| learnref1      | 10000 | p1                     | 13				 | 10000 | Completion (TT2)| CoInvestedEmployer (FS3)	| 180    |

Scenario: Contract Type 2 no On Programme Balancing payment

	When required payments event is received

	Then the payment source component will not generate any contract type 2 Balancing (TT3) coinvested payments