#New name - small_employers_fully_funded_Additional_Payments_EEF4_Completion
#Old name - small_employers - AC3

Feature: Small Employers fully funded additional payments EEF4 completion
		 AC3- 1 learner aged 19-24, non-DAS, is a care leaver, In paid employment with a small employer at start, is fully funded for on programme and completion payments
#Note: care leavers are flagged on the ILR through EEF code = 4*
#Given the apprenticeship funding band maximum is 9000

Background: 

	Given the current processing period is 13

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the payments due component generates the following contract type 2 payable earnings:
	| PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount |
	| p1                     | 13     | 10000 | 2               | 1500   |
	
@Non-DAS
@minimum_tests
@small_employers
@completion
@fully_funded
#@enhanced_funding
#@16-18 incentive
#@Framework_uplift -- will require funding band


Scenario: Contract Type 2 completion payment

	When MASH is received

	Then the payment source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 13     | 10000 | Completion_2    | FullyFundedSfa_4     | 1500   |

#----------------------------------------------------------------------------------------------------------------------------------------
# Payments V1 - for reference
#
#@SmallEmployerNonDas
#Scenario:AC3- 1 learner aged 19-24, non-DAS, is a care leaver, In paid employment with a small employer at start, is fully funded for on programme and completion payments
#
##Note: care leavers are flagged on the ILR through EEF code = 4*
#	Given the apprenticeship funding band maximum is 9000
#    When an ILR file is submitted with the following data:
#        | ULN       | learner type                 | agreed price | start date | planned end date | actual end date | completion status | framework code | programme type | pathway code | Employment Status  | Employment Status Applies | Employer Id | Small Employer | LearnDelFAM |
#        | learner a | 19-24 programme only non-DAS | 7500         | 06/08/2017 | 08/08/2018       | 08/08/2018      | completed         | 403            | 2              | 1            | In paid employment | 05/08/2017                | 12345678    | SEM1           | EEF4        |
#	And the employment status in the ILR is:
#        | Employer    | Employment Status      | Employment Status Applies | Small Employer |
#        | employer 1  | in paid employment     | 05/08/2017                | SEM1           |
#    Then the provider earnings and payments break down as follows:
#        | Type                                    | 08/17 | 09/17 | 10/17 | 11/17 | 12/17 | ... | 07/18 | 08/18 | 09/18 |
#        | Provider Earned Total                   | 620   | 620   | 620   | 1620  | 620   | ... | 620   | 2860  | 0     |
#        | Provider Earned from SFA                | 620   | 620   | 620   | 1620  | 620   | ... | 620   | 2860  | 0     |
#        | Provider Earned from Employer           | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | Provider Paid by SFA                    | 0     | 620   | 620   | 620   | 1620  | ... | 620   | 620   | 2860  |
#        | Payment due from Employer               | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | Levy account debited                    | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | SFA Levy employer budget                | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | SFA Levy co-funding budget              | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | SFA non-Levy co-funding budget          | 500   | 500   | 500   | 500   | 500   | ... | 500   | 1500  | 0     |
#        | SFA non-Levy additional payments budget | 120   | 120   | 120   | 1120  | 120   | ... | 120   | 1360  | 0     |
#
#    And the transaction types for the payments are:
#        | Payment type                 | 09/17 | 10/17 | 11/17 | 12/17 | ... | 08/18 | 09/18 |
#        | On-program                   | 500   | 500   | 500   | 500   | ... | 500   | 0     |
#        | Completion                   | 0     | 0     | 0     | 0     | ... | 0     | 1500  |
#        | Balancing                    | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | Employer 16-18 incentive     | 0     | 0     | 0     | 500   | ... | 0     | 500   |
#        | Provider 16-18 incentive     | 0     | 0     | 0     | 500   | ... | 0     | 500   |
#        | Framework uplift on-program  | 120   | 120   | 120   | 120   | ... | 120   | 0     |
#        | Framework uplift completion  | 0     | 0     | 0     | 0     | ... | 0     | 360   |
#        | Framework uplift balancing   | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | Provider disadvantage uplift | 0     | 0     | 0     | 0     | ..  | 0     | 0     |
#----------------------------------------------------------------------------------------------------------------------------------------