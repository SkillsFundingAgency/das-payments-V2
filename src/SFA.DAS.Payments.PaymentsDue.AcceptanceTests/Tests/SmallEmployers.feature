Feature: non-DAS learner employed with a small employer, is fully funded for on programme and completion payments

Background:
	Given the current collection period is R13

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the SFA contribution percentage is 100%

	And planned course duration is 12 months

	And the following course information:
	| AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | TotalNegotiatedPrice | CompletionStatus |
	| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 7500                 | completed	    |

	And the following contract type 2 On Programme earnings are provided in the latest ILR for the current academic year:
	| PriceEpisodeIdentifier | Period | TransactionType | Amount |
	| p1                     | 1      | Learning (TT1)  | 500    |
	| p1                     | 2      | Learning (TT1)  | 500    |
	| p1                     | 3      | Learning (TT1)  | 500    |
	| p1                     | 4      | Learning (TT1)  | 500    |
	| p1                     | 5      | Learning (TT1)  | 500    |
	| p1                     | 6      | Learning (TT1)  | 500    |
	| p1                     | 7      | Learning (TT1)  | 500    |
	| p1                     | 8      | Learning (TT1)  | 500    |
	| p1                     | 9      | Learning (TT1)  | 500    |
	| p1                     | 10     | Learning (TT1)  | 500    |
	| p1                     | 11     | Learning (TT1)  | 500    |
	| p1                     | 12     | Learning (TT1)  | 500    |
	| p1                     | 12     | Completion (TT2)| 1500   |


@Non-DAS
@SmallEmployer

Scenario: AC1-Payment for a 16-18 non-DAS learner, small employer at start



	When an earnings event is received

	Then the payments due component will generate the following contract type 2 payments due:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Delivery Period | ULN   | TransactionType   | Amount	|
	| learnref1      | 10000 | p1                     | 1				| 10000 | Learning (TT1)	| 500		|
	| learnref1      | 10000 | p1                     | 3				| 10000 | Learning (TT1)	| 500		|
	| learnref1      | 10000 | p1                     | 4				| 10000 | Learning (TT1)	| 500		|
	| learnref1      | 10000 | p1                     | 5				| 10000 | Learning (TT1)	| 500		|
	| learnref1      | 10000 | p1                     | 6				| 10000 | Learning (TT1)	| 500		|
	| learnref1      | 10000 | p1                     | 7				| 10000 | Learning (TT1)	| 500		|
	| learnref1      | 10000 | p1                     | 8				| 10000 | Learning (TT1)	| 500		|
	| learnref1      | 10000 | p1                     | 9				| 10000 | Learning (TT1)	| 500		|
	| learnref1      | 10000 | p1                     | 10				| 10000 | Learning (TT1)	| 500		|
	| learnref1      | 10000 | p1                     | 11				| 10000 | Learning (TT1)	| 500		|
	| learnref1      | 10000 | p1                     | 12				| 10000 | Learning (TT1)	| 500		|
	| learnref1      | 10000 | p1                     | 12				| 10000 | Completion (TT2)	| 1500		|

Scenario: AC5- Payment for a 16-18 non-DAS learner, employer is not small

@Non-DAS

	When an earnings event is received

	And the SFA contribution percentage changes to 90%

	Then the payments due component will generate the following contract type 2 payments due:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Delivery Period | ULN   | TransactionType   | Amount	|
	| learnref1      | 10000 | p1                     | 1				| 10000 | Learning (TT1)	| 500		|
	| learnref1      | 10000 | p1                     | 3				| 10000 | Learning (TT1)	| 500		|
	| learnref1      | 10000 | p1                     | 4				| 10000 | Learning (TT1)	| 500		|
	| learnref1      | 10000 | p1                     | 5				| 10000 | Learning (TT1)	| 500		|
	| learnref1      | 10000 | p1                     | 6				| 10000 | Learning (TT1)	| 500		|
	| learnref1      | 10000 | p1                     | 7				| 10000 | Learning (TT1)	| 500		|
	| learnref1      | 10000 | p1                     | 8				| 10000 | Learning (TT1)	| 500		|
	| learnref1      | 10000 | p1                     | 9				| 10000 | Learning (TT1)	| 500		|
	| learnref1      | 10000 | p1                     | 10				| 10000 | Learning (TT1)	| 500		|
	| learnref1      | 10000 | p1                     | 11				| 10000 | Learning (TT1)	| 500		|
	| learnref1      | 10000 | p1                     | 12				| 10000 | Learning (TT1)	| 500		|
	| learnref1      | 10000 | p1                     | 12				| 10000 | Completion (TT2)	| 1500		|

#
#Scenario:AC1-Payment for a 16-18 non-DAS learner, small employer at start
#
#Scenario:AC2- 1 learner aged 19-24, non-DAS, with an Education Health Care (EHC) plan, In paid employment with a small employer at start, is fully funded for on programme and completion payments
#
#Scenario:AC3- 1 learner aged 19-24, non-DAS, is a care leaver, In paid employment with a small employer at start, is fully funded for on programme and completion payments
#
#Scenario:AC4- 1 learner aged 19-24, non-DAS, employed with a small employer at start, is co-funded for on programme and completion payments (this apprentice does not have a Education Health Care plan and is not a care leaver)
#
#Scenario:AC5- Payment for a 16-18 non-DAS learner, employer is not small
#
#Scenario:AC6- 1 learner aged 16-18, non-DAS. Second employment status record added with same employer id but small employer flag removed. Learner retains small employer funding.
#
#Scenario:AC12- Payment for a 16-18 non-DAS learner, small employer at start, change to large employer
#
#Scenario:AC13- Payment for a 16-18 non-DAS learner, large employer at start, change to small employer
#
