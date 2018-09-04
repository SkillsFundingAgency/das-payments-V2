#New name - Refund_price_change_retrospectively_from_beginning
#Old name - Refund_894-AC02

#Payment v1 test has incorrect refund for employer - Rohan to investigatge

Feature: Refunds - Provider earnings and payments where learner refund payments are due
		 894-AC02 - non DAS standard learner, payments made then price is changed retrospectively from beginning

Background:
	Given the current processing period is 3

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the payments due component generates the following contract type 2 payable earnings:
	| PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount |
	| p1                     | 1      | 10000 | 1               | -750   |
	| p1                     | 2      | 10000 | 1               | -750   |
	| p2                     | 1      | 10000 | 1               | 0.6667 |
	| p2                     | 2      | 10000 | 1               | 0.6667 |
	| p2                     | 3      | 10000 | 1               | 0.6667 |

@Non-DAS
@minimum_tests
@Refunds
@price_reduced_retrospectively

Scenario: Contract Type 2 Learning payment

	When MASH is received

	Then the payment source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 1      | 10000 | Learning_1      | CoInvestedSfa_2      | -675   |
	| learnref1      | 10000 | p1                     | 1      | 10000 | Learning_1      | CoInvestedEmployer_3 | -75    |
	| learnref1      | 10000 | p1                     | 2      | 10000 | Learning_1      | CoInvestedSfa_2      | -675   |
	| learnref1      | 10000 | p1                     | 2      | 10000 | Learning_1      | CoInvestedEmployer_3 | -75    |
	| learnref1      | 10000 | p2                     | 1      | 10000 | Learning_1      | CoInvestedSfa_2      | 0.60   |
	| learnref1      | 10000 | p2                     | 1      | 10000 | Learning_1      | CoInvestedEmployer_3 | 0.0667 |
	| learnref1      | 10000 | p2                     | 2      | 10000 | Learning_1      | CoInvestedSfa_2      | 0.60   |
	| learnref1      | 10000 | p2                     | 2      | 10000 | Learning_1      | CoInvestedEmployer_3 | 0.0667 |
	| learnref1      | 10000 | p2                     | 3      | 10000 | Learning_1      | CoInvestedSfa_2      | 0.60   |
	| learnref1      | 10000 | p2                     | 3      | 10000 | Learning_1      | CoInvestedEmployer_3 | 0.0667 |
#-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

#/* V1 - Original Test
#
#@Refunds
#Feature: Provider earnings and payments where learner refund payments are due
#
#@_Minimum_Acceptance_		
# Scenario:894-AC02 - non DAS standard learner, payments made then price is changed retrospectively from beginning
#	Given  the apprenticeship funding band maximum is 27000
#	And levy balance > agreed price for all months
#	
#	And following learning has been recorded for previous payments:
#		| ULN       | employer   | provider   | learner type           | start date | aim sequence number | completion status | programme type | Total training price 1 | Total training price 1 effective date | Total assessment price 1 | Total assessment price 1 effective date |
#		| learner a | employer 0 | provider A | programme only non-DAS | 04/08/2017 | 1                   | continuing        | 25             | 9000                   | 04/08/2017                            | 2250                     | 04/08/2017                              |
#
#	And the following earnings and payments have been made to the provider A for learner a:
#		| Type                           | 08/17 | 09/17 | 10/17 | 11/17 |
#		| Provider Earned Total          | 750   | 750   | 0     | 0     |
#		| Provider Earned from SFA       | 675   | 675   | 0     | 0     |
#		| Provider Earned from Employer  | 75    | 75    | 0     | 0     |
#		| Provider Paid by SFA           | 0     | 675   | 0     | 0     |
#		| Payment due from Employer      | 0     | 75    | 0     | 0     |
#		| Levy account debited           | 0     | 0     | 0     | 0     |
#		| SFA Levy employer budget       | 0     | 0     | 0     | 0     |
#		| SFA Levy co-funding budget     | 0     | 0     | 0     | 0     |
#		| SFA non-Levy co-funding budget | 675   | 675   | 0     | 0     |
#        
#    When an ILR file is submitted for the first time on 10/10/17 with the following data:
#        | ULN       | employer   | provider   | learner type           | start date | planned end date | agreed price | completion status | programme type | Total training price 1 | Total training price 1 effective date | Total assessment price 1 | Total assessment price 1 effective date |
#        | learner a | employer 0 | provider A | programme only non-DAS | 04/08/2017 | 20/08/2018       | 10            | continuing        | 25             | 8                      | 04/08/2017                            | 2                        | 04/08/2017                              |
#	
#    Then the provider earnings and payments break down as follows:
#        | Type                           | 08/17  | 09/17  | 10/17  | 11/17    | 12/17   |
#        | Provider Earned Total          | 0.6667 | 0.6667 | 0.6667 | 0.6667   | 0.6667  |
#        | Provider Earned from SFA       | 0.60   | 0.60   | 0.60   | 0.60     | 0.60    |
#        | Provider Earned from Employer  | 0.0667 | 0.0667 | 0.0667 | 0.0667   | 0.0667  |
#        | Provider Paid by SFA           | 0      | 675    | 675    | 0.60     | 0.60    |
#        | Refund taken by SFA            | 0      | 0      | 0      | -1348.80 | 0       |
#        | Payment due from Employer      | 0      | 75     | 75     | 0.0667   | 0.06667 |
#        | Refund due to employer         | 0      | 0      | 0      | -148.67  | 0       |
#        | Levy account debited           | 0      | 0      | 0      | 0        | 0       |
#        | Levy account credited          | 0      | 0      | 0      | 0        | 0       |
#        | SFA Levy employer budget       | 0      | 0      | 0      | 0        | 0       |
#        | SFA Levy co-funding budget     | 0      | 0      | 0      | 0        | 0       |
#        | SFA non-Levy co-funding budget | 0.60   | 0.60   | 0.60   | 0.60     | 0.60    |
#*/

#-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------