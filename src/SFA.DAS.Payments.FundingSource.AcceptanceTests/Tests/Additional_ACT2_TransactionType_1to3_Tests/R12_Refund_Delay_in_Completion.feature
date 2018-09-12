Feature: Refund - Delay in completion due to change in planned end date in R12.
		 12 months apprenticeship changed to 15 months. 
		 
Background:
	
	Given the current processing period is 12

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the payments due component generates the following contract type 2 payable earnings:
	| PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount |
	| p1                     | 1      | 10000 | 1               | -1000  |
	| p1                     | 2      | 10000 | 1               | -1000  |
	| p1                     | 3      | 10000 | 1               | -1000  |
	| p1                     | 4      | 10000 | 1               | -1000  |
	| p1                     | 5      | 10000 | 1               | -1000  |
	| p1                     | 6      | 10000 | 1               | -1000  |
	| p1                     | 7      | 10000 | 1               | -1000  |
	| p1                     | 8      | 10000 | 1               | -1000  |
	| p1                     | 9      | 10000 | 1               | -1000  |
	| p1                     | 10     | 10000 | 1               | -1000  |
	| p1                     | 11     | 10000 | 1               | -1000  |
	| p2                     | 1      | 10000 | 1               |  800   |
	| p2                     | 2      | 10000 | 1               |  800   |
	| p2                     | 3      | 10000 | 1               |  800   |
	| p2                     | 4      | 10000 | 1               |  800   |
	| p2                     | 5      | 10000 | 1               |  800   |
	| p2                     | 6      | 10000 | 1               |  800   |
	| p2                     | 7      | 10000 | 1               |  800   |
	| p2                     | 8      | 10000 | 1               |  800   |
	| p2                     | 9      | 10000 | 1               |  800   |
	| p2                     | 10     | 10000 | 1               |  800   |
	| p2                     | 11     | 10000 | 1               |  800   |
	| p2                     | 12     | 10000 | 1               |  800   |

@Non-DAS
@delayed_completion
@Changed_Planned_End_Date
@multiple_price_episodes
@refund
@review

Scenario: Contract Type 2 Learning payment

	When MASH is received

	Then the payment source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 1      | 10000 | Learning_1      | CoInvestedSfa_2      | -900   |
	| learnref1      | 10000 | p1                     | 1      | 10000 | Learning_1      | CoInvestedEmployer_3 | -100   |
	| learnref1      | 10000 | p1                     | 2      | 10000 | Learning_1      | CoInvestedSfa_2      | -900   |
	| learnref1      | 10000 | p1                     | 2      | 10000 | Learning_1      | CoInvestedEmployer_3 | -100   |
	| learnref1      | 10000 | p1                     | 3      | 10000 | Learning_1      | CoInvestedSfa_2      | -900   |
	| learnref1      | 10000 | p1                     | 3      | 10000 | Learning_1      | CoInvestedEmployer_3 | -100   |
	| learnref1      | 10000 | p1                     | 4      | 10000 | Learning_1      | CoInvestedSfa_2      | -900   |
	| learnref1      | 10000 | p1                     | 4      | 10000 | Learning_1      | CoInvestedEmployer_3 | -100   |
	| learnref1      | 10000 | p1                     | 5      | 10000 | Learning_1      | CoInvestedSfa_2      | -900   |
	| learnref1      | 10000 | p1                     | 5      | 10000 | Learning_1      | CoInvestedEmployer_3 | -100   |
	| learnref1      | 10000 | p1                     | 6      | 10000 | Learning_1      | CoInvestedSfa_2      | -900   |
	| learnref1      | 10000 | p1                     | 6      | 10000 | Learning_1      | CoInvestedEmployer_3 | -100   |
	| learnref1      | 10000 | p1                     | 7      | 10000 | Learning_1      | CoInvestedSfa_2      | -900   |
	| learnref1      | 10000 | p1                     | 7      | 10000 | Learning_1      | CoInvestedEmployer_3 | -100   |
	| learnref1      | 10000 | p1                     | 8      | 10000 | Learning_1      | CoInvestedSfa_2      | -900   |
	| learnref1      | 10000 | p1                     | 8      | 10000 | Learning_1      | CoInvestedEmployer_3 | -100   |
	| learnref1      | 10000 | p1                     | 9      | 10000 | Learning_1      | CoInvestedSfa_2      | -900   |
	| learnref1      | 10000 | p1                     | 9      | 10000 | Learning_1      | CoInvestedEmployer_3 | -100   |
	| learnref1      | 10000 | p1                     | 10     | 10000 | Learning_1      | CoInvestedSfa_2      | -900   |
	| learnref1      | 10000 | p1                     | 10     | 10000 | Learning_1      | CoInvestedEmployer_3 | -100   |
	| learnref1      | 10000 | p1                     | 11     | 10000 | Learning_1      | CoInvestedSfa_2      | -900   |
	| learnref1      | 10000 | p1                     | 11     | 10000 | Learning_1      | CoInvestedEmployer_3 | -100   |

	| learnref1      | 10000 | p2                     | 1      | 10000 | Learning_1      | CoInvestedSfa_2      |  720   |
	| learnref1      | 10000 | p2                     | 1      | 10000 | Learning_1      | CoInvestedEmployer_3 |  80    |
	| learnref1      | 10000 | p2                     | 2      | 10000 | Learning_1      | CoInvestedSfa_2      |  720   |
	| learnref1      | 10000 | p2                     | 2      | 10000 | Learning_1      | CoInvestedEmployer_3 |  80    |
	| learnref1      | 10000 | p2                     | 3      | 10000 | Learning_1      | CoInvestedSfa_2      |  720   |
	| learnref1      | 10000 | p2                     | 3      | 10000 | Learning_1      | CoInvestedEmployer_3 |  80    |
	| learnref1      | 10000 | p2                     | 4      | 10000 | Learning_1      | CoInvestedSfa_2      |  720   |
	| learnref1      | 10000 | p2                     | 4      | 10000 | Learning_1      | CoInvestedEmployer_3 |  80    |
	| learnref1      | 10000 | p2                     | 5      | 10000 | Learning_1      | CoInvestedSfa_2      |  720   |
	| learnref1      | 10000 | p2                     | 5      | 10000 | Learning_1      | CoInvestedEmployer_3 |  80    |
	| learnref1      | 10000 | p2                     | 6      | 10000 | Learning_1      | CoInvestedSfa_2      |  720   |
	| learnref1      | 10000 | p2                     | 6      | 10000 | Learning_1      | CoInvestedEmployer_3 |  80    |
	| learnref1      | 10000 | p2                     | 7      | 10000 | Learning_1      | CoInvestedSfa_2      |  720   |
	| learnref1      | 10000 | p2                     | 7      | 10000 | Learning_1      | CoInvestedEmployer_3 |  80    |
	| learnref1      | 10000 | p2                     | 8      | 10000 | Learning_1      | CoInvestedSfa_2      |  720   |
	| learnref1      | 10000 | p2                     | 8      | 10000 | Learning_1      | CoInvestedEmployer_3 |  80    |
	| learnref1      | 10000 | p2                     | 9      | 10000 | Learning_1      | CoInvestedSfa_2      |  720   |
	| learnref1      | 10000 | p2                     | 9      | 10000 | Learning_1      | CoInvestedEmployer_3 |  80    |
	| learnref1      | 10000 | p2                     | 10     | 10000 | Learning_1      | CoInvestedSfa_2      |  720   |
	| learnref1      | 10000 | p2                     | 10     | 10000 | Learning_1      | CoInvestedEmployer_3 |  80    |
	| learnref1      | 10000 | p2                     | 11     | 10000 | Learning_1      | CoInvestedSfa_2      |  720   |
	| learnref1      | 10000 | p2                     | 11     | 10000 | Learning_1      | CoInvestedEmployer_3 |  80    |
	| learnref1      | 10000 | p2                     | 12     | 10000 | Learning_1      | CoInvestedSfa_2      |  720   |
	| learnref1      | 10000 | p2                     | 12     | 10000 | Learning_1      | CoInvestedEmployer_3 |  80    |