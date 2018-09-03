#Warning - This test may need reviewing 
#V1 data values (£315) after price change are incorrect

#New name - AdditionalPayments_price_change_during_programme_learning_support
#Old name - AdditionalPayments_671-AC02

Feature: Additional payments price change during programme learning support
		 671-AC02 Non-DAS learner, levy available, is taking an English or maths qualification, has learning support and the negotiated price changes during the programme

Background:
	
	Given the current processing period is 5


	And the following learners:
	| LearnRefNumber | Ukprn | ULN   |
	| learnref1      | 10000 | 10000 |

	And the payments due component generates the following contract type 2 on program earnings:
	| PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount |
	| p2                     | 5      | 10000 | 1               | 600    |

@Non-DAS
@minimum_tests
#@additional_payments
#@Maths_English
#@Provider_Learning_Support
#@Change_in_price
#-- funding band


Scenario: Contract Type 2 Learning payment

	When MASH is received

	Then the payment source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p2                     | 5      | 10000 | Learning_1      | CoInvestedSfa_2      | 540    |
	| learnref1      | 10000 | p2                     | 5      | 10000 | Learning_1      | CoInvestedEmployer_3 | 60     |
	

#----------------------------------------------------------------------------------------------------------------------------------------
#/*  Payments V1 for reference
#

#============================
#ISSUE in the scenario data
#============================

#@MathsAndEnglishNonDas
#@_Minimum_Acceptance_
#Scenario:671-AC02 Non-DAS learner, levy available, is taking an English or maths qualification, has learning support and the negotiated price changes during the programme
#    Given the apprenticeship funding band maximum is 18000
#    
#    When an ILR file is submitted with the following data:
#        | ULN       | learner type           | aim type         | start date | planned end date | actual end date | completion status | aim rate | Total training price 1 | Total training price 1 effective date | Total assessment price 1 | Total assessment price 1 effective date | Total training price 2 | Total training price 2 effective date | Total assessment price 2 | Total assessment price 2 effective date | learning support code | learning support date from | learning support date to |
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
#        | English and maths on programme | 33.64 | 33.64 | 33.64 | 33.64 | 33.64 |
#		| English and maths Balancing    | 0     | 0     | 0     | 0     | 0     |
#        | Provider learning support      | 150   | 150   | 150   | 150   | 150   |
#
#
#----------------------------------------------------------------------------------------------------------------------------------------