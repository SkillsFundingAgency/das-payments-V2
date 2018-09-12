#In-progress - Check if this is a valid scenarion and review the scenario
Feature: R04 - No change in duration but start and end months changed after few months
#Start date changed to original start date - 1, end date month also changed to original planned month - 1


Background:
	Given the current processing period is 4

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the payments due component generates the following contract type 2 payable earnings:
	| PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount |
	| p1                     | 1      | 10000 | 1               | -600   |
	| p1                     | 2      | 10000 | 1               | -600   |
	| p2                     | 1      | 10000 | 1               | 600    |
	| p2                     | 2      | 10000 | 1               | 600    |
	| p2                     | 3      | 10000 | 1               | 600    |
	| p2                     | 4      | 10000 | 1               | 600    |

@Non-DAS
@Historical_Payments
@Start_Date_Earlier

Scenario: Contract Type 2 Learning payment

	When a payable earning event is received

	Then the funding source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 1      | 10000 | Learning_1      | CoInvestedSfa_2      | -540   |
	| learnref1      | 10000 | p1                     | 1      | 10000 | Learning_1      | CoInvestedEmployer_3 | -60    |
	| learnref1      | 10000 | p1                     | 2      | 10000 | Learning_1      | CoInvestedSfa_2      | -540   |
	| learnref1      | 10000 | p1                     | 2      | 10000 | Learning_1      | CoInvestedEmployer_3 | -60    |
	| learnref1      | 10000 | p2                     | 1      | 10000 | Learning_1      | CoInvestedSfa_2      | 540    |
	| learnref1      | 10000 | p2                     | 1      | 10000 | Learning_1      | CoInvestedEmployer_3 | 60     |
	| learnref1      | 10000 | p2                     | 2      | 10000 | Learning_1      | CoInvestedSfa_2      | 540    |
	| learnref1      | 10000 | p2                     | 2      | 10000 | Learning_1      | CoInvestedEmployer_3 | 60     |
	| learnref1      | 10000 | p2                     | 3      | 10000 | Learning_1      | CoInvestedSfa_2      | 540    |
	| learnref1      | 10000 | p2                     | 3      | 10000 | Learning_1      | CoInvestedEmployer_3 | 60     |
	| learnref1      | 10000 | p2                     | 4      | 10000 | Learning_1      | CoInvestedSfa_2      | 540    |
	| learnref1      | 10000 | p2                     | 4      | 10000 | Learning_1      | CoInvestedEmployer_3 | 60     |