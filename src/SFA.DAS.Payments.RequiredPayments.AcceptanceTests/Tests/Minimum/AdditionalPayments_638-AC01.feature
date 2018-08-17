Feature: Additional payments
		638-AC01 Non-DAS learner, takes an English qualification that has a planned end date that exceeds the actual end date of the programme aim

Background:
	
	#Check with Dave
	Given the current processing period is 14


	And the following learners:
	| LearnRefNumber | Ukprn | ULN   |
	| learnref7      | 10000 | 10000 |

	#Check with Dave
	And the following course information:
	| LearnRefNumber | Ukprn | ULN   | AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | LearningStartDate | LearningPlannedEndDate | LearningActualEndDate | CompletionStatus |
	| learnref7      | 10000 | 10000 | 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 08/08/2018             | 08/08/2018            | Completed       |
	| learnref7      | 10000 | 10000 | 2            |               |               |             | 471          | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 06/10/2018             | 06/10/2018            | Completed       |


	And the following contract type 2 on programme earnings for periods 1-12 are provided in the latest ILR for the academic year 1718:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice |
	| learnref7      | 10000 | p1                     | 06/08/2017       | 06/08/2017                   | 15000                |

#	And the following contract type 2 on programme earnings for periods 1-12 are provided in the latest ILR for the academic year 1718:
#	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice | Learning_1 |
#	| learnref7      | 10000 | p1                     | 06/08/2017       | 06/08/2017                   | 15000                | 1000       |
#
#	And the following contract type 2 completion earning for period 12 are provided in the latest ILR for the academic year 1718:
#	| LearnRefNumber | Ukprn | Amount |
#	| learnref7      | 10000 | 3000   |  
#

@Non-DAS
@minimum_tests
@additional_payments
@completion
#@Maths_English
#@Maths_English_FinshedLate

Scenario Outline: Contract Type 2 On programme payments

	And the following historical contract type 2 on programme payments exist:   
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref7      | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount> |
	| learnref7      | 10000 | p1                     | 2      | 10000 | <transaction_type> | <amount> |
	| learnref7      | 10000 | p1                     | 3      | 10000 | <transaction_type> | <amount> |
	| learnref7      | 10000 | p1                     | 4      | 10000 | <transaction_type> | <amount> |
	| learnref7      | 10000 | p1                     | 5      | 10000 | <transaction_type> | <amount> |
	| learnref7      | 10000 | p1                     | 6      | 10000 | <transaction_type> | <amount> |
	| learnref7      | 10000 | p1                     | 7      | 10000 | <transaction_type> | <amount> |
	| learnref7      | 10000 | p1                     | 8      | 10000 | <transaction_type> | <amount> |
	| learnref7      | 10000 | p1                     | 9      | 10000 | <transaction_type> | <amount> |
	| learnref7      | 10000 | p1                     | 10     | 10000 | <transaction_type> | <amount> |
	| learnref7      | 10000 | p1                     | 11     | 10000 | <transaction_type> | <amount> |
	| learnref7      | 10000 | p1                     | 12     | 10000 | <transaction_type> | <amount> |

	When a TOBY is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref7      | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount> |
	| learnref7      | 10000 | p1                     | 2      | 10000 | <transaction_type> | <amount> |
	| learnref7      | 10000 | p1                     | 3      | 10000 | <transaction_type> | <amount> |
	| learnref7      | 10000 | p1                     | 4      | 10000 | <transaction_type> | <amount> |
	| learnref7      | 10000 | p1                     | 5      | 10000 | <transaction_type> | <amount> |
	| learnref7      | 10000 | p1                     | 6      | 10000 | <transaction_type> | <amount> |
	| learnref7      | 10000 | p1                     | 7      | 10000 | <transaction_type> | <amount> |
	| learnref7      | 10000 | p1                     | 8      | 10000 | <transaction_type> | <amount> |
	| learnref7      | 10000 | p1                     | 9      | 10000 | <transaction_type> | <amount> |
	| learnref7      | 10000 | p1                     | 10     | 10000 | <transaction_type> | <amount> |
	| learnref7      | 10000 | p1                     | 11     | 10000 | <transaction_type> | <amount> |
	| learnref7      | 10000 | p1                     | 12     | 10000 | <transaction_type> | <amount> |

	Examples: 
	| transaction_type | amount |
	| Learning_1       | 1000    |	
	
Scenario Outline: Contract Type 2 completion payment

	And the following historical contract type 2 completion payment exist:   
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref7      | 10000 | p1                     | 12      | 10000 | <transaction_type> | <amount> |

	When a TOBY is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref7      | 10000 | p1                     | 12      | 10000 | <transaction_type> | <amount> |
	
	Examples: 
	| transaction_type | amount |
	| Completion_2     | 3000   |
	

#----------------------------------------------------------------------------------------------------------------------------------------
#/*  Payments V1 for reference
#
#@MathsAndEnglishNonDas
#@_Minimum_Acceptance_
#Scenario:638-AC01 Non-DAS learner, takes an English qualification that has a planned end date that exceeds the actual end date of the programme aim
#
#	When an ILR file is submitted with the following data:
#		| ULN       | learner type           | aim type         | agreed price | aim rate | start date | planned end date | actual end date | completion status | 
#		| learner a | programme only non-DAS | programme        | 15000        |          | 06/08/2017 | 08/08/2018       | 08/08/2018      | completed         | 
#		| learner a | programme only non-DAS | maths or english |              | 471      | 06/08/2017 | 06/10/2018       | 06/10/2018      | completed         |
#	Then the provider earnings and payments break down as follows:
#		| Type                                    | 08/17   | 09/17   | 10/17   | ... | 05/18   | 06/18   | 07/18   | 08/18   | 09/18   | 10/18 | 11/18 |
#		| Provider Earned Total                   | 1033.64 | 1033.64 | 1033.64 | ... | 1033.64 | 1033.64 | 1033.64 | 3033.64 | 33.64   | 0     | 0     |
#		| Provider Paid by SFA                    | 0       | 933.64  | 933.64  | ... | 933.64  | 933.64  | 933.64  | 933.64  | 2733.64 | 33.64 | 0     |
#		| Payment due from Employer               | 0       | 100     | 100     | ... | 100     | 100     | 100     | 100     | 300     | 0     | 0     |
#		| Levy account debited                    | 0       | 0       | 0       | ... | 0       | 0       | 0       | 0       | 0       | 0     | 0     |
#		| SFA Levy employer budget                | 0       | 0       | 0       | ... | 0       | 0       | 0       | 0       | 0       | 0     | 0     |
#		| SFA Levy co-funding budget              | 0       | 0       | 0       | ... | 0       | 0       | 0       | 0       | 0       | 0     | 0     |
#		| SFA non-Levy co-funding budget          | 900     | 900     | 900     | ... | 900     | 900     | 900     | 2700    | 0       | 0     | 0     |
#		| SFA Levy additional payments budget     | 0       | 0       | 0       | ... | 0       | 0       | 0       | 0       | 0       | 0     | 0     |
#		| SFA non-Levy additional payments budget | 33.64   | 33.64   | 33.64   | ... | 33.64   | 33.64   | 33.64   | 33.64   | 33.64   | 0     | 0     |
#    And the transaction types for the payments are:
#		| Payment type                   | 09/17 | 10/17 | ... | 05/18 | 06/18 | 07/18 | 08/18 | 09/18 | 10/18 | 11/18 |
#		| On-program                     | 900   | 900   | ... | 900   | 900   | 900   | 900   | 0     | 0     | 0     |
#		| Completion                     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 2700  | 0     | 0     |
#		| Balancing                      | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     | 0     | 0     |
#		| English and maths on programme | 33.64 | 33.64 | ... | 33.64 | 33.64 | 33.64 | 33.64 | 33.64 | 33.64 | 0     |
#		| English and maths Balancing    | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     | 0     | 0     |

#----------------------------------------------------------------------------------------------------------------------------------------