Feature: 807_AC2- Non-DAS learner, is made redundant outside of the last 6 months of planned learning - receives full government funding for the next 12 weeks only 

Background:
	Given the current processing period is 12

	And the following learners:
	| LearnRefNumber | Ukprn | ULN   |
	| learnref1      | 10000 | 10000 |

	And the following course information:
	| AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | LearningStartDate | LearningPlannedEndDate | LearningActualEndDate | CompletionStatus |
	| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 03/08/2017        | 20/08/2018             |                       | continuing       |

	And the employment status in the ILR is:
    | Employer     | Employment Status      | Employment Status Applies |
    | EmployerRef1 | in paid employment     | 02/08/2017                |
    |              | not in paid employment | 19/02/2018                |

	#Should this be 1-12 months or 1-9 months?
	And the following contract type 2 on programme earnings for periods 1-12 are provided in the latest ILR for the academic year 1718:
	| PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice | Learning_1 |
	| p1                     | 03/08/2017       | 03/08/2017                   | 15000                | 1000       |

@Non-DAS
@Redundancy
@query
@Review
#Check the months paid

Scenario Outline: Contract Type 2 On programme payments

	And the following historical contract type 2 on programme payments exist:   
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 2      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 3      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 4      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 5      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 6      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 7      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 8      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 9      | 10000 | <transaction_type> | <amount> |
	

	When a TOBY is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 2      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 3      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 4      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 5      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 6      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 7      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 8      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 9      | 10000 | <transaction_type> | <amount> |
	
	Examples: 
	| transaction_type | amount |
	| Learning_1       | 1000   |



#
#	-------------------------
#
#	V1 - version
#
#@Redundancy
#Scenario:807_AC2- Non-DAS learner, is made redundant outside of the last 6 months of planned learning - receives full government funding for the next 12 weeks only 
#
#        Given the apprenticeship funding band maximum is 15000
#        #And the learner is made redundant less than 6 months before the planned end date
#            
#        When an ILR file is submitted with the following data:
#            | ULN       | learner type           | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date | 
#            | learner a | programme only non-DAS | 03/08/2017 | 20/08/2018       |                 | continuing        | 12000                | 03/08/2017                          | 3000                   | 03/08/2017                            | 
#        
#        And the employment status in the ILR is:
#            | Employer   | Employment Status      | Employment Status Applies |
#            | employer 1 | in paid employment     | 02/08/2017                |
#            |            | not in paid employment | 19/02/2018                |
#              
#        Then the provider earnings and payments break down as follows:
#            | Type                           | 08/17 | 09/17 | 10/17 | ... | 01/18 | 02/18 | 03/18 | 04/18 | 05/18 |
#            | Provider Earned Total          | 1000  | 1000  | 1000  | ... | 1000  | 1000  | 1000  | 1000  | 0     |
#            | Provider Earned from SFA       | 900   | 900   | 900   | ... | 900   | 1000  | 1000  | 1000  | 0     |
#            | Provider Earned from Employer  | 100   | 100   | 100   | ... | 100   | 0     | 0     | 0     | 0     |
#            | Provider Paid by SFA           | 0     | 900   | 900   | ... | 900   | 900   | 1000  | 1000  | 1000  |
#            | Refund taken by SFA            | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     |
#            | Payment due from Employer      | 0     | 100   | 100   | ... | 100   | 100   | 0     | 0     | 0     |
#            | Refund due to employer         | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     |
#            | Levy account debited           | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     |
#            | Levy account credited          | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     |
#            | SFA Levy employer budget       | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     |
#            | SFA Levy co-funding budget     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     |
#            | SFA non-Levy co-funding budget | 900   | 900   | 900   | ... | 900   | 1000  | 1000  | 1000  | 0     |


#	----------------------------