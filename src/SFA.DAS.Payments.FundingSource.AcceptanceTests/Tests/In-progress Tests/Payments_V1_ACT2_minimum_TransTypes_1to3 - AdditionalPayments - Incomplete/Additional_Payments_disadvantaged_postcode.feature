#New name - Additional_Payments_disadvantaged_postcode
#Old name - Additional_Payments - AC5
Feature: Additional payments disadvantaged postcode
		AC5- Payment for a non-DAS learner, lives in a disadvantaged postocde area - 1-10% most deprived, funding agreed within band maximum, UNDERTAKING APPRENTICESHIP FRAMEWORK The provider incentive for this postcode group is £600 split equally into 2 payments at 90 and 365 days. INELIGIBLE FOR APPRENTICESHIP STANDARDS

Background: 

	Given the current processing period is 4

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the payments due component generates the following contract type 2 payable earnings:
	| PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount |
	| p1                     | 4      | 10000 | Learning_1      | 1000   |

@Non-DAS
@minimum_tests
#@additional_payments
#@disadvantaged_postcode
@partial
	
Scenario: Contract Type 2 Learning payment

	When MASH is received

	Then the payment source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 4      | 10000 | Learning_1      | CoInvestedSfa_2      | 900    |
	| learnref1      | 10000 | p1                     | 4      | 10000 | Learning_1      | CoInvestedEmployer_3 | 100    |

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