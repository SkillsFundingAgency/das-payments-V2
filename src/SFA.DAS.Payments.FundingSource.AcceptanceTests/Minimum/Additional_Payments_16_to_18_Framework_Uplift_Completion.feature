#Additional_Payments_16_to_18_Framework_Uplift_Completion
#Old name - Additional_Payments - AC3
Feature: Additional payments 16 to 18 framework uplift completion
	AC3-Learner finishes on time, earns on-programme and completion payments. 
	#Original description is with Payments V1 team to review as implementation is different.
	#AC3-Learner finishes on time, earns on-programme and completion payments. Assumes 12 month apprenticeship and learner completes after 10 months.

Background: 

	Given the current processing period is 12

	And the following learners:
	| LearnRefNumber | Ukprn | ULN   |
	| learnref3      | 10000 | 10000 |

	And the payments due component generates the following contract type 2 on programme earnings:
	| PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount |
	| p1                     | 12     | 10000 | 1               | 600    |

	And the payments due component generates the following contract type 2 completion earnings:
	| PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount |
	| p1                     | 12     | 10000 | 2               | 1800   |

@Non-DAS
@minimum_tests
@additional_payments
@completion
#@16-18 incentive
#@Framework_uplift -- will require funding band

Scenario: Contract Type 2 On programme payments

	When MASH is received

	Then the payment source component will generate the following contract type 2 coinvested payments:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref3      | 10000 | p1                     | 12     | 10000 | Learning_1      | CoInvestedSfa_2      | 540    |
	| learnref3      | 10000 | p1                     | 12     | 10000 | Learning_1      | CoInvestedEmployer_3 | 60     |

Scenario: Contract Type 2 completion payment

	When MASH is received

	Then the payment source component will generate the following contract type 2 coinvested payments:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref3      | 10000 | p1                     | 12     | 10000 | Completion_2    | CoInvestedSfa_2      | 1620   |
	| learnref3      | 10000 | p1                     | 12     | 10000 | Completion_2    | CoInvestedEmployer_3 | 180    |

#----------------------------------------------------------------------------------------------------------------------------------------
# Payments V1 - for reference
#
#@AdditionalPayments
#Feature: 16 to 18 learner incentives, framework uplifts, level 2 english or maths payments
#
#@_Minimum_Acceptance_
#Scenario:AC3-Learner finishes on time, earns on-programme and completion payments. Assumes 12 month apprenticeship and learner completes after 10 months.
#    Given the apprenticeship funding band maximum is 9000
#    When an ILR file is submitted with the following data:
#		| ULN    | learner type                 | agreed price | start date | planned end date | actual end date | completion status | framework code | programme type | pathway code |
#        | 123456 | 16-18 programme only non-DAS | 9000         | 06/08/2017 | 09/08/2018       | 10/08/2018      | Completed         | 403            | 2              | 1            |
#
#    Then the provider earnings and payments break down as follows:
#        | Type                                    | 08/17 | 09/17 | 10/17 | 11/17 | 12/17 | ... | 06/18 | 07/18 | 08/18 | 09/18 |
#        | Provider Earned Total                   | 720   | 720   | 720   | 1720  | 720   | ... | 720   | 720   | 3160  | 0     |
#        | Provider Earned from SFA                | 660   | 660   | 660   | 1660  | 660   | ... | 660   | 660   | 2980  | 0     |
#        | Provider Earned from Employer           | 60    | 60    | 60    | 60    | 60    | ... | 60    | 60    | 180   | 0     |
#        | Provider Paid by SFA                    | 0     | 660   | 660   | 660   | 1660  | ... | 660   | 660   | 660   | 2980  |
#        | Payment due from Employer               | 0     | 60    | 60    | 60    | 60    | ... | 60    | 60    | 60    | 180   |
#        | Levy account debited                    | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     |
#        | SFA Levy employer budget                | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     |
#        | SFA Levy co-funding budget              | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     |
#		| SFA non-Levy co-funding budget          | 540   | 540   | 540   | 540   | 540   | ... | 540   | 540   | 1620  | 0     |
#        | SFA non-Levy additional payments budget | 120   | 120   | 120   | 1120  | 120   | ... | 120   | 120   | 1360  | 0     |
#
#    And the transaction types for the payments are:
#        | Payment type                 | 08/17 | 09/17 | 10/17 | 11/17 | 12/17 | ... | 06/18 | 07/18 | 08/18 | 09/18 |
#        | On-program                   | 0     | 540   | 540   | 540   | 540   | ... | 540   | 540   | 540   | 0     |
#        | Completion                   | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 1620  |
#        | Balancing                    | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     |
#        | Employer 16-18 incentive     | 0     | 0     | 0     | 0     | 500   | ... | 0     | 0     | 0     | 500   |
#        | Provider 16-18 incentive     | 0     | 0     | 0     | 0     | 500   | ... | 0     | 0     | 0     | 500   |
#        | Framework uplift on-program  | 0     | 120   | 120   | 120   | 120   | ... | 120   | 120   | 120   | 0     |
#        | Framework uplift completion  | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 360   |
#        | Framework uplift balancing   | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     |
#        | Provider disadvantage uplift | 0     | 0     | 0     | 0     | 0     | ..  | 0     | 0     | 0     | 0     |

#----------------------------------------------------------------------------------------------------------------------------------------