Feature: TNP is higher than the maximum funding band, completed on time with full history
#Reduced on program funding as per band maximum - 600 instead of 1000. Also, reduced On Programme Completion payment - 1800 instead of 3000

Background:
	Given the current processing period is 13
	And the apprenticeship funding band maximum is 9000

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000
	
	And the SFA contribution percentage is "90%"

	And the payments due component generates the following contract type 2 payable earnings:
	| PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount |
	| p1                     | 13     | 10000 | Completion (TT2)| 1800   |

@Non-DAS
@Completion
@funding_band
@capping
@minimum_additional

Scenario: Contract Type 2 On Programme Completion payment

	When MASH is received

	Then the payment source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 13     | 10000 | Completion (TT2)| CoInvestedSfa (FS2)  | 1620   |
	| learnref1      | 10000 | p1                     | 13     | 10000 | Completion (TT2)| CoInvestedEmployer (FS3)| 180    |