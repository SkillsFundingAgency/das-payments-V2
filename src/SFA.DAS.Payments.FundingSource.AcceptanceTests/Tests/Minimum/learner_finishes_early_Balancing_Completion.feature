#New name - learner_finishes_early_Balancing_Completion
#Old name - learner_finishes_early - non-DAS learner

Feature: Provider earnings and payments where learner completes earlier than planned

    #The earnings and payment rules for early completions are the same as for learners finishing on time, except that the completion payment is earned earlier.

Background:

	Given the current processing period is 13
	#Given the apprenticeship funding band maximum for each learner is 20000

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the payments due component generates the following contract type 2 payable earnings:
	| PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount |
	| p1                     | 13     | 10000 | 2               | 3750   |
	| p1                     | 13     | 10000 | 3               | 3000   |

	
@Non-DAS
@minimum_tests
@completion
@balancing
@FinishingEarly
@partial

Scenario: Contract Type 2 completion payment

	When MASH is received

	Then the payment source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 13     | 10000 | Completion_2    | CoInvestedSfa_2      | 3375   |
	| learnref1      | 10000 | p1                     | 13     | 10000 | Completion_2    | CoInvestedEmployer_3 | 375    |


Scenario: Contract Type 2 balancing payment

	When MASH is received

	Then the payment source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 13     | 10000 | Balancing_3     | CoInvestedSfa_2      | 2700   |
	| learnref1      | 10000 | p1                     | 13     | 10000 | Balancing_3     | CoInvestedEmployer_3 | 300    |	
	

#----------------------------------------------------------------------------------------------------------------------------------------
#/*  Payments V1 for reference
#
#Feature: Provider earnings and payments where learner completes earlier than planned
#
#    The earnings and payment rules for early completions are the same as for learners finishing on time, except that the completion payment is earned earlier.
#
#    Background:
#        Given the apprenticeship funding band maximum for each learner is 20000

#    Scenario: A non-DAS learner, learner finishes early
#        When an ILR file is submitted with the following data:
#            | ULN       | learner type           | agreed price | start date | planned end date | actual end date | completion status |
#            | learner a | programme only non-DAS | 18750        | 01/09/2017 | 08/12/2018       | 08/09/2018      | completed         |
#        Then the provider earnings and payments break down as follows:
#            | Type                           | 09/17 | 10/17 | 11/17 | ... | 08/18 | 09/18 | 10/18 |
#            | Provider Earned Total          | 1000  | 1000  | 1000  | ... | 1000  | 6750  | 0     |
#            | Provider Earned from SFA       | 900   | 900   | 900   | ... | 900   | 6075  | 0     |
#            | Provider Earned from Employer  | 100   | 100   | 100   | ... | 100   | 675   | 0     |
#            | Provider Paid by SFA           | 0     | 900   | 900   | ... | 900   | 900   | 6075  |
#            | Payment due from Employer      | 0     | 100   | 100   | ... | 100   | 100   | 675   |
#            | Levy account debited           | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#            | SFA Levy employer budget       | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#            | SFA Levy co-funding budget     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#            | SFA non-Levy co-funding budget | 900   | 900   | 900   | ... | 900   | 6075  | 0     |
#        And the transaction types for the payments are:
#            | Payment type             | 10/17 | 11/17 | 12/17 | 01/18 | ... | 09/18 | 10/18 |
#            | On-program               | 900   | 900   | 900   | 900   | ... | 900   | 0     |
#            | Completion               | 0     | 0     | 0     | 0     | ... | 0     | 3375  |
#            | Balancing                | 0     | 0     | 0     | 0     | ... | 0     | 2700  |
#            | Employer 16-18 incentive | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#            | Provider 16-18 incentive | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#*/        
#----------------------------------------------------------------------------------------------------------------------------------------