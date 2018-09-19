Feature: TNP is higher than the maximum funding band, continuing
#Reduced on program funding as per band maximum - 600 instead of 1000

Background:
	Given the current processing period is 3
	And the apprenticeship funding band maximum is 9000

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the SFA contribution percentage is "90%"

	And the payments due component generates the following contract type 2 payable earnings:
	| PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount |
	| p1                     | 3      | 10000 | Learning (TT1)  | 600    |


@Non-DAS
@funding_band
@capping
@minimum_additional

Scenario: Contract Type 2 Learning payment

	When MASH is received

	Then the payment source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 3      | 10000 | Learning (TT1)  | CoInvestedSfa (FS2)  | 540    |
	| learnref1      | 10000 | p1                     | 3      | 10000 | Learning (TT1)  | CoInvestedEmployer (FS3)| 60     |

#------------------------------------------------
#	V1 - Original test
#
#------------------------------------------------
#
#	Scenario: 640-AC02-Payment for a non-DAS learner with a negotiated price above the funding band cap
#    Given the apprenticeship funding band maximum is 15000
#    When an ILR file is submitted with the following data:
#        | Provider   | ULN       | learner type           | agreed price | start date | planned end date | actual end date | Completion status | standard code |
#        | provider a | learner a | programme only non-DAS | 18000        | 06/08/2017 | 08/08/2018       |                 | continuing        | 50            |
#    Then the following capping will apply to the price episodes:
#        | Provider   | price episode | negotiated price | funding cap | previous funding paid | price above cap | effective price for SFA payments |
#        | provider a | 08/17 onwards | 18000            | 15000       | 0                     | 3000            | 15000                            |
#    And the provider earnings and payments break down as follows:
#        | Type                                    | 08/17 | 09/17 | 10/17 | 11/17 | 12/17 |
#        | Provider Earned Total                   | 1000  | 1000  | 1000  | 1000  | 1000  |
#        | Provider Earned from SFA                | 900   | 900   | 900   | 900   | 900   |
#        | Provider Earned from Employer           | 100   | 100   | 100   | 100   | 100   |
#        | Provider Paid by SFA                    | 0     | 900   | 900   | 900   | 900   |
#        | Payment due from Employer               | 0     | 100   | 100   | 100   | 100   |
#        | Levy account debited                    | 0     | 0     | 0     | 0     | 0     |
#        | SFA Levy employer budget                | 0     | 0     | 0     | 0     | 0     |
#        | SFA Levy co-funding budget              | 0     | 0     | 0     | 0     | 0     |
#        | SFA non-Levy co-funding budget          | 900   | 900   | 900   | 900   | 900   |
#        | SFA Levy additional payments budget     | 0     | 0     | 0     | 0     | 0     |
#        | SFA non-Levy additional payments budget | 0     | 0     | 0     | 0     | 0     |
#    And the transaction types for the payments are:
#        | Payment type                 | 09/17 | 10/17 | 11/17 | 12/17 |
#        | On-program                   | 900   | 900   | 900   | 900   |
#        | Completion                   | 0     | 0     | 0     | 0     |
#        | Balancing                    | 0     | 0     | 0     | 0     |
#        | Employer 16-18 incentive     | 0     | 0     | 0     | 0     |
#        | Provider 16-18 incentive     | 0     | 0     | 0     | 0     |
#        | Provider disadvantage uplift | 0     | 0     | 0     | 0     |

#------------------------------------------------