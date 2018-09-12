#New name - Additional_Payments_Framework_Uplift_Balancing_Completion
#Old name - AdditionalPayments_581-AC01

Feature: Additional payments framework uplift balancing completion
		581-AC01-Non DAS learner finishes early, price equals the funding band maximum, earns balancing and completion framework uplift payments. Assumes 15 month apprenticeship and learner completes after 12 months.

Background:

	Given the current processing period is 13

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the payments due component generates the following contract type 2 payable earnings:
	| PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount |
	| p1                     | 13     | 10000 | 2               | 1800   |
	| p1                     | 13     | 10000 | 3               | 1440   |

@Non-DAS
@minimum_tests
#@additional_payments
@completion
@balancing
@FinishingEarly
#@Price_same_as_FundingBand
#@Framework_uplift -- missing, will require funding band
@partial
	
Scenario: Contract Type 2 completion payment

	When a payable earning event is received

	Then the funding source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 13     | 10000 | Completion_2    | CoInvestedSfa_2      | 1620   |
	| learnref1      | 10000 | p1                     | 13     | 10000 | Completion_2    | CoInvestedEmployer_3 | 180    |


Scenario: Contract Type 2 balancing payment

	When a payable earning event is received

	Then the funding source component will generate the following contract type 2 coinvested payments:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 13     | 10000 | Balancing_3     | CoInvestedSfa_2      | 1296   |
	| learnref1      | 10000 | p1                     | 13     | 10000 | Balancing_3     | CoInvestedEmployer_3 | 144    |

#----------------------------------------------------------------------------------------------------------------------------------------
#/*  Payments V1 for reference
#
#
#@FrameworkUpliftsForNonDasFinishingEarly
#Scenario: 581-AC01-Non DAS learner finishes early, price equals the funding band maximum, earns balancing and completion framework uplift payments. Assumes 15 month apprenticeship and learner completes after 12 months.
#    Given the apprenticeship funding band maximum is 9000
#    When an ILR file is submitted with the following data:
#		| ULN    | learner type                 | agreed price | start date | planned end date | actual end date | completion status | framework code | programme type | pathway code |
#        | 123456 | 16-18 programme only non-DAS | 9000         | 06/08/2017 | 09/11/2018       | 09/08/2018      | Completed         | 403            | 2              | 1            |
#    Then the provider earnings and payments break down as follows:
#        | Type                                    | 08/17 | 09/17 | 10/17 | 11/17 | 12/17 | ... | 06/18 | 07/18 | 08/18 | 09/18 |
#        | Provider Earned Total                   | 576   | 576   | 576   | 1576  | 576   | ... | 576   | 576   | 4888  | 0     |
#        | Provider Earned from SFA                | 528   | 528   | 528   | 1528  | 528   | ... | 528   | 528   | 4564  | 0     |
#        | Provider Earned from Employer           | 48    | 48    | 48    | 48    | 48    | ... | 48    | 48    | 324   | 0     |
#        | Provider Paid by SFA                    | 0     | 528   | 528   | 528   | 1528  | ... | 528   | 528   | 528   | 4564  |
#        | Payment due from Employer               | 0     | 48    | 48    | 48    | 48    | ... | 48    | 48    | 48    | 324   |
#        | Levy account debited                    | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     |
#        | SFA Levy employer budget                | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     |
#        | SFA Levy co-funding budget              | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     |
#		| SFA non-Levy co-funding budget          | 432   | 432   | 432   | 432   | 432   | ... | 432   | 432   | 2916  | 0     |
#        | SFA non-Levy additional payments budget | 96    | 96    | 96    | 1096  | 96    | ... | 96    | 96    | 1648  | 0     |
#    And the transaction types for the payments are:
#        | Payment type                 | 08/17 | 09/17 | 10/17 | 11/17 | 12/17 | ... | 06/18 | 07/18 | 08/18 | 09/18 |
#        | On-program                   | 0     | 432   | 432   | 432   | 432   | ... | 432   | 432   | 432   | 0     |
#        | Completion                   | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 1620  |
#        | Balancing                    | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 1296  |
#        | Employer 16-18 incentive     | 0     | 0     | 0     | 0     | 500   | ... | 0     | 0     | 0     | 500   |
#        | Provider 16-18 incentive     | 0     | 0     | 0     | 0     | 500   | ... | 0     | 0     | 0     | 500   |
#        | Framework uplift on-program  | 0     | 96    | 96    | 96    | 96    | ... | 96    | 96    | 96    | 0     |
#        | Framework uplift completion  | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 360   |
#        | Framework uplift balancing   | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 288   |
#        | Provider disadvantage uplift | 0     | 0     | 0     | 0     | 0     | ..  | 0     | 0     | 0     | 0     |
#*/        
#----------------------------------------------------------------------------------------------------------------------------------------