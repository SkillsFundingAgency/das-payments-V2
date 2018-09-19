Feature: Break in Learning, learner takes 2 months break

Background:
	
	Given the current processing period is 8

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the payments due component generates the following contract type 2 payable earnings:
	| PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount |
	| p2                     | 7      | 10000 | Learning_1      | 1000    |	

@Non-DAS
@minimum_additional
@BreakInLearning

Scenario: Contract Type 2 Learning payment

	When a payable earning event is received

	Then the funding source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p2                     | 7      | 10000 | Learning_1      | CoInvestedSfa_2      | 900    |
	| learnref1      | 10000 | p2                     | 7      | 10000 | Learning_1      | CoInvestedEmployer_3 | 100    |

#
#    V1 - DAS test
#	
#    Scenario: Apprentice goes on a planned break midway through the learning episode and this is notified through the ILR
#        Given the following commitments exist on 03/12/2017:
#            | commitment Id | version Id | ULN       | start date | end date   | status | agreed price | effective from | effective to |
#            | 1             | 1-001      | learner a | 01/09/2017 | 08/09/2018 | active | 15000        | 01/09/2017     | 31/10/2017   |
#            | 1             | 1-002      | learner a | 01/09/2017 | 08/09/2018 | paused | 15000        | 01/11/2017     | 02/01/2018   |
#            | 1             | 1-003      | learner a | 01/09/2017 | 08/09/2018 | active | 15000        | 03/01/2018     |              |
#        When an ILR file is submitted on 03/12/2017 with the following data:
#            | ULN       | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date |
#            | learner a | 01/09/2017 | 08/09/2018       | 31/10/2017      | planned break     | 12000                | 01/09/2017                          | 3000                   | 01/09/2017                            |
#            | learner a | 03/01/2018 | 08/11/2018       |                 | continuing        | 12000                | 03/01/2018                          | 3000                   | 03/01/2018                            |
#        Then the provider earnings and payments break down as follows:
#            | Type                     | 09/17 | 10/17 | 11/17 | 12/17 | 01/18 | 02/18 | ... | 10/18 | 11/18 |
#            | Provider Earned from SFA | 1000  | 1000  | 0     | 0     | 1000  | 1000  | ... | 1000  | 0     |
#            | Provider Paid by SFA     | 0     | 1000  | 1000  | 0     | 0     | 1000  | ... | 1000  | 1000  |
#            | Levy account debited     | 0     | 1000  | 1000  | 0     | 0     | 1000  | ... | 1000  | 1000  |
#            | SFA Levy employer budget | 1000  | 1000  | 0     | 0     | 1000  | 1000  | ... | 1000  | 0     |