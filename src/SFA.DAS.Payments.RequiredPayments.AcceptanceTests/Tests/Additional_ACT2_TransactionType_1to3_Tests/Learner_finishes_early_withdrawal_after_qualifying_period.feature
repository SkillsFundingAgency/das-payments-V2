Feature: A non-DAS learner, learner withdraws after qualifying period
#Learner withdraws after 4 months so there will be no payments earned after 4 months

Background:
	Given the current processing period is 6
	
	And a learner with LearnRefNumber learnref3 and Uln 10000 undertaking training with training provider 10000

	And the following course information:
	| AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | LearningStartDate | LearningPlannedEndDate | LearningActualEndDate | CompletionStatus |
	| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 01/08/2017		  | 08/08/2018             | 08/12/2017			   | withdrawn        |

	And the following contract type 2 on programme earnings for periods 1-12 are provided in the latest ILR for the academic year 1718:
	| PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice | Learning_1 |
	| p1                     | 06/08/2017       | 06/08/2017                   | 15000                | 1000       |

@Non-DAS
@Learner_finishes_early
@Withdrawal
@minimum_additional

Scenario Outline: Contract Type 2 On programme payments

	And the following historical contract type 2 on programme payments exist:   
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1        | 10000 | p1                   | 1      | 10000 | <transaction_type> | <amount> |
	| learnref1        | 10000 | p1                   | 2      | 10000 | <transaction_type> | <amount> |
	| learnref1        | 10000 | p1                   | 3      | 10000 | <transaction_type> | <amount> |
	| learnref1        | 10000 | p1                   | 4      | 10000 | <transaction_type> | <amount> |
	When a TOBY is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 2      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 3      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 4      | 10000 | <transaction_type> | <amount> |
		
	Examples: 
	| transaction_type | amount |
	| Learning_1       | 1000   |



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