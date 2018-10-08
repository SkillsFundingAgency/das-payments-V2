Feature: Earning Event Test

Background:

	Given a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the Earnings Calculator generates the following earnings:
	|   |   |   |   |   |
	| a | A | A | A | A |

	And the required payments component generates the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount |
	| learnref1      | 10000 | p1                     | 9      | 10000 | Learning (TT1)  | 1000   |
	| learnref1      | 10000 | p1                     | 10     | 10000 | Completion (TT2)| 3000   |
	| learnref1      | 10000 | p1                     | 10     | 10000 | Balancing (TT3) | 3000   |
	
