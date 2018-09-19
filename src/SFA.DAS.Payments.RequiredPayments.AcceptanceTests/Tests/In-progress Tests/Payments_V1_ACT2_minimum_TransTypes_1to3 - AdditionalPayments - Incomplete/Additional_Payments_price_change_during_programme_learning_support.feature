#Warning - This test may need reviewing 
#V1 data values (£315) after price change are incorrect

#New name - AdditionalPayments_price_change_during_programme_Learning_support
#Old name - AdditionalPayments_671-AC02

Feature: Additional payments price change during programme Learning support
		 671-AC02 Non-DAS learner, levy available, is taking an English or maths qualification, has Learning support and the negotiated price changes during the programme

Background:
	
	Given the current processing period is 5

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000
	And the SFA contribution percentage is "90%"
	And the following course information:
	| AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | LearningStartDate | LearningPlannedEndDate | LearningActualEndDate | CompletionStatus |
	| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 04/08/2017        | 20/08/2018             |						| Contunuing       |
	| 2            |               |               |             | 471          | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 06/10/2018             |						| Continuing	   |

	And the following contract type 2 On Programme earnings for periods 1-3 are provided in the latest ILR for the academic year 1718:
	| PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice | Learning (TT1) |
	| p1                     | 04/08/2017       | 04/08/2017                   | 11250                | 750        |	

	And the following contract type 2 On Programme earnings for periods 4-12 are provided in the latest ILR for the academic year 1718:
	| PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice | Learning (TT1) |
	| p2                     | 11/11/2017       | 11/11/2017                   | 6750                 | 600        |	

@Non-DAS
@minimum_tests
#@additional_payments
#@Maths_English
#@Provider_Learning_Support
@Change_in_price
#-- funding band
@partial

Scenario Outline: Contract Type 2 On Programme Learning payments

	And the following historical contract type 2 On Programme Learning payments exist:   
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p1				      | 1      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1				      | 2      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1				      | 3      | 10000 | <transaction_type> | <amount> |

	When a TOBY is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 2      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 3      | 10000 | <transaction_type> | <amount> |

	Examples: 
	| transaction_type | amount |
	| Learning (TT1)   | 750    |


Scenario Outline: Contract Type 2 On Programme Learning payments changed price

	And the following historical contract type 2 On Programme Learning payments exist:   
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p2				      | 4      | 10000 | <transaction_type> | <amount> |

	When a TOBY is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p2                     | 4      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p2                     | 5      | 10000 | <transaction_type> | <amount> |

	Examples: 
	| transaction_type | amount |
	| Learning (TT1)   | 600    |
	

#----------------------------------------------------------------------------------------------------------------------------------------
#/*  Payments V1 for reference
#

#============================
#ISSUE in the scenario data
#============================

#@MathsAndEnglishNonDas
#@_Minimum_Acceptance_
#Scenario:671-AC02 Non-DAS learner, levy available, is taking an English or maths qualification, has Learning support and the negotiated price changes during the programme
#    Given the apprenticeship funding band maximum is 18000
#    
#    When an ILR file is submitted with the following data:
#        | ULN       | learner type           | aim type         | start date | planned end date | actual end date | Completion status | aim rate | Total training price 1 | Total training price 1 effective date | Total assessment price 1 | Total assessment price 1 effective date | Total training price 2 | Total training price 2 effective date | Total assessment price 2 | Total assessment price 2 effective date | Learning support code | Learning support date from | Learning support date to |
#        | learner a | programme only non-DAS | programme        | 04/08/2017 | 20/08/2018       |                 | continuing        |          | 9000                   | 04/08/2017                            | 2250                     | 04/08/2017                              | 5400                   | 11/11/2017                            | 1350                     | 11/11/2017                              | 1                     | 06/08/2017                 | 06/10/2018               |
#        | learner a | programme only non-DAS | maths or english | 04/08/2017 | 06/10/2018       |                 | continuing        | 471      |                        |                                       |                          |                                         |                        |                                       |                          |                                         | 1                     | 06/08/2017                 | 06/10/2018               |        
#    Then the provider earnings and payments break down as follows: 
#        | Type                                    | 08/17   | 09/17  | 10/17   | 11/17   | 12/17  | 01/18  | 
#        | Provider Earned Total                   | 933.64  | 933.64 | 933.64  | 533.64  | 533.64 | 533.64 |       
#        | Provider Earned from SFA                | 858.64  | 858.64 | 858.64  | 498.64  | 498.64 | 498.64 |       
#        | Provider Earned from Employer           | 75      | 75     | 75      | 35      | 35     | 35     |       
#        | Provider Paid by SFA                    | 0       | 858.64 | 858.64  | 858.64  | 498.64 | 498.64 |        
#        | Payment due from Employer               | 0       | 75     | 75      | 75      | 35     | 35     |       
#        | Levy account debited                    | 0       | 0      | 0       | 0       | 0      | 0      |         
#        | SFA Levy employer budget                | 0       | 0      | 0       | 0       | 0      | 0      |
#        | SFA Levy co-funding budget              | 0       | 0      | 0       | 0       | 0      | 0      |
#        | SFA non-Levy co-funding budget          | 675     | 675    | 675     | 315     | 315    | 315    |               
#        | SFA Levy additional payments budget     | 0       | 0      | 0       | 0       | 0      | 0      |
#        | SFA non-Levy additional payments budget | 183.64  | 183.64 | 183.64  | 183.64  | 183.64 | 183.64 |         
#    And the transaction types for the payments are:
#		| Payment type                   | 09/17 | 10/17 | 11/17 | 12/17 | 01/18 |
#		| On-program                     | 675   | 675   | 675   | 315   | 315   |
#		| Completion                     | 0     | 0     | 0     | 0     | 0     |
#		| Balancing                      | 0     | 0     | 0     | 0     | 0     |
#        | English and maths On Programme | 33.64 | 33.64 | 33.64 | 33.64 | 33.64 |
#		| English and maths Balancing    | 0     | 0     | 0     | 0     | 0     |
#        | Provider Learning support      | 150   | 150   | 150   | 150   | 150   |
#
#
#----------------------------------------------------------------------------------------------------------------------------------------