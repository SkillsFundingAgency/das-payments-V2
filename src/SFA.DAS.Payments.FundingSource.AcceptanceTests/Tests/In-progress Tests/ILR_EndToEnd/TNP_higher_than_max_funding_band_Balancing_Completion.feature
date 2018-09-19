Feature: TNP is higher than the maximum funding band, Learning finishes 3 months early

Background:

	Given the current processing period is 13
	And the apprenticeship funding band maximum is 15000

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the SFA contribution percentage is "90%"

	And the payments due component generates the following contract type 2 payable earnings:
	| PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount |
	| p1                     | 13     | 10000 | Completion (TT2)| 2700   |
	| p1                     | 13     | 10000 | Balancing (TT3) | 2400   |

@Non-DAS
@Completion
@Balancing
@funding_band
@capping
@FinishingEarly
@minimum_additional

Scenario: Contract Type 2 On Programme Completion payment

	When MASH is received

	Then the payment source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 13     | 10000 | Completion (TT2)| CoInvestedSfa (FS2)  | 2430   |
	| learnref1      | 10000 | p1                     | 13     | 10000 | Completion (TT2)| CoInvestedEmployer (FS3)| 270    |


Scenario: Contract Type 2 On Programme Balancing payment

	When MASH is received

	Then the payment source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 13     | 10000 | Balancing (TT3) | CoInvestedSfa (FS2)  | 2160   |
	| learnref1      | 10000 | p1                     | 13     | 10000 | Balancing (TT3) | CoInvestedEmployer (FS3)| 240    |