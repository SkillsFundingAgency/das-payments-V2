Feature: A non-DAS learner, learner withdraws after qualifying period
#Learner withdraws after 4 months so there will be no payments earned after 4 months

Background:
	Given the current processing period is 6

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

@Non-DAS
@Learner_finishes_early
@Withdrawal
@minimum_additional

Scenario: Contract Type 2 no payment

	When a payable earning event is received

	Then the payment source component will generate no contract type 2 coinvested payments


	#-------------------------------------------------------------------------

	#V1 - version

#	Feature: Provider earnings and payments where learner completes earlier than planned
#
#    The earnings and payment rules for early completions are the same as for learners finishing on time, except that the completion payment is earned earlier.
#
#    Background:
#        Given the apprenticeship funding band maximum for each learner is 20000
#
#
#	
#    Scenario: A non-DAS learner, learner withdraws after qualifying period
#    
#        When an ILR file is submitted with the following data:
#            | ULN       | agreed price | learner type           | start date | planned end date | actual end date | completion status |
#            | learner a | 15000        | programme only non-DAS | 01/09/2017 | 08/09/2018       | 08/01/2018      | withdrawn         |
#        Then the provider earnings and payments break down as follows:
#            | Type                          | 09/17 | 10/17 | 11/17 | 12/17 | 01/18 |
#            | Provider Earned Total         | 1000  | 1000  | 1000  | 1000  | 0     |
#            | Provider Earned from SFA      | 900   | 900   | 900   | 900   | 0     |
#            | Provider Earned from Employer | 100   | 100   | 100   | 100   | 0     |
#            | Provider Paid by SFA          | 0     | 900   | 900   | 900   | 900   |
#            | Payment due from Employer     | 0     | 100   | 100   | 100   | 100   |
#            | Levy account debited          | 0     | 0     | 0     | 0     | 0     |
#            | SFA Levy employer budget      | 0     | 0     | 0     | 0     | 0     |
#            | SFA Levy co-funding budget    | 0     | 0     | 0     | 0     | 0     |
#            | SFA non-Levy co-funding budget| 900   | 900   | 900   | 900   | 0     |
#        And the transaction types for the payments are:
#            | Payment type             | 10/17 | 11/17 | 12/17 | 01/18 | 
#            | On-program               | 900   | 900   | 900   | 900   | 
#            | Completion               | 0     | 0     | 0     | 0     | 
#            | Balancing                | 0     | 0     | 0     | 0     | 
#            | Employer 16-18 incentive | 0     | 0     | 0     | 0     | 
#            | Provider 16-18 incentive | 0     | 0     | 0     | 0     | 



#-------------------------------------------------------------------------