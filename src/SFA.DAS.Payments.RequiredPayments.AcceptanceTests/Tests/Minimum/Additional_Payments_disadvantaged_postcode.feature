﻿#New name - Additional_Payments_disadvantaged_postcode
#Old name - Additional_Payments - AC5
Feature: Additional payments disadvantaged postcode
		AC5- Payment for a non-DAS learner, lives in a disadvantaged postocde area - 1-10% most deprived, funding agreed within band maximum, UNDERTAKING APPRENTICESHIP FRAMEWORK The provider incentive for this postcode group is £600 split equally into 2 payments at 90 and 365 days. INELIGIBLE FOR APPRENTICESHIP STANDARDS

Background: 

	Given the current processing period is 4
  
	And the following learners:
	| LearnRefNumber | Ukprn | ULN   |
	| learnref1      | 10000 | 10000 |

	And the following course information:
	| AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | LearningStartDate | LearningPlannedEndDate | LearningActualEndDate | CompletionStatus |
	| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 09/08/2018             |                       | Continuing       |

	And the following contract type 2 on programme earnings for periods 1-12 are provided in the latest ILR for the academic year 1718:
	| PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice | Learning_1 |
	| p1                     | 06/08/2017       | 06/08/2017                   | 15000                | 1000       |

@Non-DAS
@minimum_tests
@additional_payments
#@disadvantaged_postcode
	
Scenario Outline: Contract Type 2 On programme payments
	And the following historical contract type 2 on programme payments exist:   
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 2      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 3      | 10000 | <transaction_type> | <amount> |

	When a TOBY is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 2      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 3      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 4      | 10000 | <transaction_type> | <amount> |

	Examples: 
	| transaction_type | amount |
	| Learning_1       | 1000    |	
	

	

#----------------------------------------------------------------------------------------------------------------------------------------	
	
#--  Payments V1 for reference
#
#@AdditionalPayments
#Feature: 16 to 18 learner incentives, framework uplifts, level 2 english or maths payments
#
#Scenario:AC5- Payment for a non-DAS learner, lives in a disadvantaged postocde area - 1-10% most deprived, funding agreed within band maximum, UNDERTAKING APPRENTICESHIP FRAMEWORK The provider incentive for this postcode group is £600 split equally into 2 payments at 90 and 365 days. INELIGIBLE FOR APPRENTICESHIP STANDARDS
#  
#    When an ILR file is submitted with the following data:
#        | ULN       | learner type             | agreed price | start date | planned end date | actual end date | completion status | framework code | programme type | pathway code | home postcode deprivation |
#		  | learner a | programme only non-DAS | 15000        | 06/08/2017 | 08/08/2018       |                 | continuing        | 403            | 2              | 1            | 1-10%                     |
#    Then the provider earnings and payments break down as follows:
#        | Type                                    | 08/17 | 09/17 | 10/17 | 11/17 | 12/17 | ... | 07/18 | 08/18 | 09/18 |
#        | Provider Earned Total                   | 1000  | 1000  | 1000  | 1300  | 1000  | ... | 1000  | 300   | 0     |
#        | Provider Paid by SFA                    | 0     | 900   | 900   | 900   | 1200  | ... | 900   | 900   | 300   |
#        | Payment due from Employer               | 0     | 100   | 100   | 100   | 100   | ... | 100   | 100   | 0     |
#        | Levy account debited                    | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | SFA Levy employer budget                | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | SFA Levy co-funding budget              | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | SFA non-Levy co-funding budget          | 900   | 900   | 900   | 900   | 900   | ... | 900   | 0     | 0     |
#        | SFA non-Levy additional payments budget | 0     | 0     | 0     | 300   | 0     | ... | 0     | 300   | 0     |
#        | SFA Levy additional payments budget     | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#    And the transaction types for the payments are:
#        | Payment type                 | 09/17 | 10/17 | 11/17 | 12/17 | ... | 08/18 | 09/18 |
#        | On-program                   | 900   | 900   | 900   | 900   | ... | 900   | 0     |
#        | Completion                   | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | Balancing                    | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | Provider disadvantage uplift | 0     | 0     | 0     | 300   | ... | 0     | 300   |
#

#----------------------------------------------------------------------------------------------------------------------------------------