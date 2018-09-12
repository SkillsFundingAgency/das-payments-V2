Feature: 807_AC1- Non-DAS learner, is made redundant within the last 6 months of planned learning - receives full government funding for the rest of the programme 
#What will be the learner status after 12 months

Background:
	Given the current processing period is 12

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the payments due component generates the following contract type 2 payable earnings:
	| PriceEpisodeIdentifier | Period | ULN   | TransactionType | Amount |
	| p1                     | 12     | 10000 | 1               | 1000   |

@Non-DAS
@Redundancy
@Review
@minimum_additional

Scenario: Contract Type 2 Learning payment

	When a payable earning event is received

	Then the funding source component will generate the following contract type 2 coinvested payments:

	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource        | Amount |
	| learnref1      | 10000 | p1                     | 12     | 10000 | Learning_1      | CoInvestedSfa_2      | 900    |
	| learnref1      | 10000 | p1                     | 12     | 10000 | Learning_1      | CoInvestedEmployer_3 | 100    |
#
#Scenario Outline: Contract Type 2 completion payment
#
#	When a TOBY is received
#
#	Then the payments due component will generate the following contract type 2 payable earnings:
#	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
#	| learnref1      | 10000 | p1                     | 12     | 10000 | <transaction_type> | <amount> |
#	
#	Examples: 
#	| transaction_type | amount |
#	| Completion_2     | 3000   |




#
#	-------------------------
#
#	V1 - version
#
#	@Redundancy
#Scenario:807_AC1- Non-DAS learner, is made redundant within the last 6 months of planned learning - receives full government funding for the rest of the programme 
#
#        Given the apprenticeship funding band maximum is 15000
#		#the learner is programme only non-DAS
#        
#        #And the learner is made redundant less than 6 months before the planned end date
#            
#        When an ILR file is submitted with the following data:
#            | ULN       | learner type           | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date | 
#            | learner a | programme only non-DAS | 03/08/2017 | 20/08/2018       |                 | continuing        | 12000                | 03/08/2017                          | 3000                   | 03/08/2017                            | 
#        
#        And the employment status in the ILR is:
#            | Employer   | Employment Status      | Employment Status Applies |
#            | employer 1 | in paid employment     | 02/08/2017                |
#            |            | not in paid employment | 21/02/2018                |
#              
#        Then the provider earnings and payments break down as follows:
#            | Type                           | 08/17 | 09/17 | 10/17 | ... | 01/18 | 02/18 | 03/18 | 04/18 | 05/18 |
#            | Provider Earned Total          | 1000  | 1000  | 1000  | ... | 1000  | 1000  | 1000  | 1000  | 1000  |
#            | Provider Earned from SFA       | 900   | 900   | 900   | ... | 900   | 1000  | 1000  | 1000  | 1000  |
#            | Provider Earned from Employer  | 100   | 100   | 100   | ... | 100   | 0     | 0     | 0     | 0     |
#            | Provider Paid by SFA           | 0     | 900   | 900   | ... | 900   | 900   | 1000  | 1000  | 1000  |
#            | Refund taken by SFA            | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     |
#            | Payment due from Employer      | 0     | 100   | 100   | ... | 100   | 100   | 0     | 0     | 0     |
#            | Refund due to employer         | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     |
#            | Levy account debited           | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     |
#            | Levy account credited          | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     |
#            | SFA Levy employer budget       | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     |
#            | SFA Levy co-funding budget     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     |
#            | SFA non-Levy co-funding budget | 900   | 900   | 900   | ... | 900   | 1000  | 1000  | 1000  | 1000  |
#
#	----------------------------