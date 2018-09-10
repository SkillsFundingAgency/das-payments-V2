#New name - ContractTypeChangesFromAct1ToAct2
#Old name - DPP_965_02
Feature: Contract Type Changes From ACT1 To ACT2
	DPP_965_02 - Non-Levy apprentice, provider edits contract type (ACT) in the ILR, previous on-programme and English/math payments are refunded and repaid according to latest contract type

Background:
	Given the current processing period is 3

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the payments due component generates the following contract type 1 payable earnings:
	| PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount |
	| p1                     | 1      | 10000 | 1               | -600   |
	| p1                     | 2      | 10000 | 1               | -600   |

	And the payments due component generates the following contract type 2 payable earnings:
	| PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount |
	| p2                     | 1      | 10000 | 1               | 600    |
	| p2                     | 2      | 10000 | 1               | 600    |
	| p2                     | 3      | 10000 | 1               | 600    |

@DAS
@minimum_tests
@learner_changes_contract_type
@apprenticeship_contract_type_changes
#@English_Maths

Scenario: Contract Type 1 Learning payment

	When MASH is received

	Then the payment source component will generate the following contract type 1 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 1      | 10000 | Learning_1      | CoInvestedSfa_2      | -540   |
	| learnref1      | 10000 | p1                     | 1      | 10000 | Learning_1      | CoInvestedEmployer_3 | -60    |
	| learnref1      | 10000 | p1                     | 2      | 10000 | Learning_1      | CoInvestedSfa_2      | -540   |
	| learnref1      | 10000 | p1                     | 2      | 10000 | Learning_1      | CoInvestedEmployer_3 | -60    |

@Non-DAS
@minimum_tests
@learner_changes_contract_type
@apprenticeship_contract_type_changes
#@English_Maths

Scenario: Contract Type 2 Learning payment

	When MASH is received

	Then the payment source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 1      | 10000 | Learning_1      | CoInvestedSfa_2      | 540    |
	| learnref1      | 10000 | p1                     | 1      | 10000 | Learning_1      | CoInvestedEmployer_3 | 60     |
	| learnref1      | 10000 | p1                     | 2      | 10000 | Learning_1      | CoInvestedSfa_2      | 540    |
	| learnref1      | 10000 | p1                     | 2      | 10000 | Learning_1      | CoInvestedEmployer_3 | 60     |
	| learnref1      | 10000 | p1                     | 3      | 10000 | Learning_1      | CoInvestedSfa_2      | 540    |
	| learnref1      | 10000 | p1                     | 3      | 10000 | Learning_1      | CoInvestedEmployer_3 | 60     |


#-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

#/* V1 - Original Test


#@learner_changes_contract_type
#Feature: Learner changes contract type
#
#Scenario: DPP_965_02 - Non-Levy apprentice, provider edits contract type (ACT) in the ILR, previous on-programme and English/math payments are refunded and repaid according to latest contract type		
#    Given levy balance > agreed price for all months
#    And the apprenticeship funding band maximum is 9000
#
#	And the following commitments exist:
#		| commitment Id | version Id | ULN       | start date | end date   | framework code | programme type | pathway code | agreed price | status    | effective from | effective to |
#		| 1             | 1          | learner a | 01/08/2017 | 01/08/2018 | 403            | 2              | 1            | 9000         | Active    | 01/08/2017     |              |
#        
#    When an ILR file is submitted for period R01 with the following data:
#		| ULN       | learner type       | agreed price | start date | planned end date | actual end date | completion status | aim type         | aim sequence number | aim rate | framework code | programme type | pathway code | contract type | contract type date from | contract type date to |
#		| learner a | programme only DAS | 9000         | 06/08/2017 | 20/08/2018       |                 | continuing        | programme        | 2                   |          | 403            | 2              | 1            | DAS           | 06/08/2017              | 20/08/2018            |
#		| learner a | programme only DAS |              | 06/08/2017 | 20/08/2018       |                 | continuing        | maths or english | 1                   | 471      | 403            | 2              | 1            |               |                         |                       |
#
#
#    And an ILR file is submitted for period R03 with the following data:
#        | ULN       | learner type           | agreed price | start date | planned end date | actual end date | completion status | aim type         | aim sequence number | aim rate | framework code | programme type | pathway code | contract type | contract type date from | contract type date to |
#        | learner a | programme only non-DAS | 9000         | 06/08/2017 | 20/08/2018       |                 | continuing        | programme        | 2                   |          | 403            | 2              | 1            | Non-DAS       | 06/08/2017              | 20/08/2018            |
#        | learner a | programme only non-DAS |              | 06/08/2017 | 20/08/2018       |                 | continuing        | maths or english | 1                   | 471      | 403            | 2              | 1            |               |                         |                       |
#  
#    Then the provider earnings and payments break down as follows:
#        | Type                                    | 08/17  | 09/17  | 10/17  | 11/17    |
#        | Provider Earned Total                   | 639.25 | 639.25 | 639.25 | 639.25   |
#        | Provider Earned from SFA                | 579.25 | 579.25 | 579.25 | 579.25   |
#        | Provider Earned from Employer           | 60     | 60     | 0      | 0        |
#        | Provider Paid by SFA                    | 0      | 639.25 | 639.25 | 1737.75  |
#        | Refund taken by SFA                     | 0      | 0      | 0      | -1278.50 |
#        | Payment due from Employer               | 0      | 0      | 0      | 180      |
#        | Refund due to employer                  | 0      | 0      | 0      | 0        |
#        | Levy account debited                    | 0      | 600    | 600    | 0        |
#        | Levy account credited                   | 0      | 0      | 0      | 1200     |
#        | SFA Levy employer budget                | 0      | 0      | 0      | 0        |
#        | SFA Levy co-funding budget              | 0      | 0      | 0      | 0        |
#        | SFA Levy additional payments budget     | 0      | 0      | 0      | 0        |
#        | SFA non-Levy co-funding budget          | 540    | 540    | 540    | 540      |
#        | SFA non-Levy additional payments budget | 39.25  | 39.25  | 39.25  | 39.25    |


#*/

#-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------