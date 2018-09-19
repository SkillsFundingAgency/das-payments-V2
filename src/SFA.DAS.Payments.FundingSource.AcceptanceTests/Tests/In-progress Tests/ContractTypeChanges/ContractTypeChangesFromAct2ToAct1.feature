#New name - ContractTypeChangesFromAct2ToAct1
#Old name - DPP_965_01

Feature: Contract Type Changes From ACT2 To ACT1
	DPP_965_01 - Levy apprentice, provider edits contract type (ACT) in the ILR, previous on-programme and English/math payments are refunded and repaid according to latest contract type

Background:
	Given the current processing period is 3

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the SFA contribution percentage is "90%"

	And the payments due component generates the following contract type 2 payable earnings:
	| PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount |
	| p1                     | 1      | 10000 | Learning (TT1)  | -600   |
	| p1                     | 2      | 10000 | Learning (TT1)  | -600   |

	And the payments due component generates the following contract type 1 payable earnings:
	| PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount |
	| p2                     | 1      | 10000 | Learning (TT1)  | 600    |
	| p2                     | 2      | 10000 | Learning (TT1)  | 600    |
	| p2                     | 3      | 10000 | Learning (TT1)  | 600    |

@Non-DAS
@minimum_tests
@learner_changes_contract_type
@apprenticeship_contract_type_changes
#@English_Maths
@partial

Scenario: Contract Type 2 Learning payment

	When MASH is received

	Then the payment source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 1      | 10000 | Learning (TT1)  | CoInvestedSfa (FS2)  | -540   |
	| learnref1      | 10000 | p1                     | 1      | 10000 | Learning (TT1)  | CoInvestedEmployer (FS3)| -60    |
	| learnref1      | 10000 | p1                     | 2      | 10000 | Learning (TT1)  | CoInvestedSfa (FS2)  | -540   |
	| learnref1      | 10000 | p1                     | 2      | 10000 | Learning (TT1)  | CoInvestedEmployer (FS3)| -60    |

@DAS
@minimum_tests
@learner_changes_contract_type
@apprenticeship_contract_type_changes
#@English_Maths
@partial

Scenario: Contract Type 1 Learning payment

	When MASH is received

	Then the payment source component will generate the following contract type 1 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 1      | 10000 | Learning (TT1)  | CoInvestedSfa (FS2)  | 540    |
	| learnref1      | 10000 | p1                     | 1      | 10000 | Learning (TT1)  | CoInvestedEmployer (FS3)| 60     |
	| learnref1      | 10000 | p1                     | 2      | 10000 | Learning (TT1)  | CoInvestedSfa (FS2)  | 540    |
	| learnref1      | 10000 | p1                     | 2      | 10000 | Learning (TT1)  | CoInvestedEmployer (FS3)| 60     |
	| learnref1      | 10000 | p1                     | 3      | 10000 | Learning (TT1)  | CoInvestedSfa (FS2)  | 540    |
	| learnref1      | 10000 | p1                     | 3      | 10000 | Learning (TT1)  | CoInvestedEmployer (FS3)| 60     |



#-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

# /* V1 - Original Test
#
# @learner_changes_contract_type
#Feature: Learner changes contract type
#
#Scenario: DPP_965_01 - Levy apprentice, provider edits contract type (ACT) in the ILR, previous on-programme and English/math payments are refunded and repaid according to latest contract type
#    Given The learner is programme only DAS
#	And levy balance > agreed price for all months
#	And the apprenticeship funding band maximum is 9000
#
#	And the following commitments exist:
#		| commitment Id | version Id | ULN       | start date | end date   | framework code | programme type | pathway code | agreed price | status | effective from | effective to |
#		| 1             | 1          | learner a | 01/08/2017 | 01/08/2018 | 403            | 2              | 1            | 9000         | Active | 01/08/2017     |              |
#        
#	When an ILR file is submitted for period R01 with the following data:
#        | ULN       | learner type           | agreed price | start date | planned end date | actual end date | Completion status | aim type         | aim sequence number | aim rate | framework code | programme type | pathway code | contract type | contract type date from |
#        | learner a | programme only non-DAS | 9000         | 06/08/2017 | 20/08/2018       |                 | continuing        | programme        | 2                   |          | 403            | 2              | 1            | Non-DAS       | 06/08/2017              |
#        | learner a | programme only non-DAS |              | 06/08/2017 | 20/08/2018       |                 | continuing        | maths or english | 1                   | 471      | 403            | 2              | 1            |               |                         |
#        
#    And an ILR file is submitted for period R03 with the following data:
#        | ULN       | learner type       | agreed price | start date | planned end date | actual end date | Completion status | aim type         | aim sequence number | aim rate | framework code | programme type | pathway code | contract type | contract type date from |
#        | learner a | programme only DAS | 9000         | 06/08/2017 | 20/08/2018       |                 | continuing        | programme        | 2                   |          | 403            | 2              | 1            | DAS           | 06/08/2017              |
#        | learner a | programme only DAS |              | 06/08/2017 | 20/08/2018       |                 | continuing        | maths or english | 1                   | 471      | 403            | 2              | 1            |               |                         |
#
#	Then the provider earnings and payments break down as follows:
#        | Type                                    | 08/17  | 09/17  | 10/17  | 11/17    |
#        | Provider Earned Total                   | 639.25 | 639.25 | 639.25 | 639.25   |
#        | Provider Earned from SFA                | 639.25 | 639.25 | 639.25 | 639.25   |
#        | Provider Earned from Employer           | 0      | 0      | 0      | 0        |
#        | Provider Paid by SFA                    | 0      | 579.25 | 579.25 | 1917.75  |
#        | Refund taken by SFA                     | 0      | 0      | 0      | -1158.50 |
#        | Payment due from Employer               | 0      | 60     | 60     | 0        |
#        | Refund due to employer                  | 0      | 0      | 0      | 120      |
#        | Levy account debited                    | 0      | 0      | 0      | 1800     |
#        | Levy account credited                   | 0      | 0      | 0      | 0        |
#        | SFA Levy employer budget                | 600    | 600    | 600    | 600      |
#        | SFA Levy co-funding budget              | 0      | 0      | 0      | 0        |
#        | SFA Levy additional payments budget     | 39.25  | 39.25  | 39.25  | 39.25    |
#        | SFA non-Levy co-funding budget          | 0      | 0      | 0      | 0        |
#        | SFA non-Levy additional payments budget | 0      | 0      | 0      | 0        |
#
#
# */


#-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------