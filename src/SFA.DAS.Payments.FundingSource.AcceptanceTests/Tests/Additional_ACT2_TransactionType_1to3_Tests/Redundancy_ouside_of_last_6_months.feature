Feature: 807_AC2- Non-DAS learner, is made redundant outside of the last 6 months of planned learning - receives full government funding for the next 12 weeks only 
#Learner made redundant after 6 months, receives funding for next 3 months and then no further funding 
#Further test scenarios needs adding for boundary cases

Background:
	Given the current processing period is 10

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

@Non-DAS
@Redundancy
@Review
@minimum_additional

Scenario: Contract Type 2 no payment

	When MASH is received

	Then the payment source component will generate no contract type 2 coinvested payments



#
#	-------------------------
#
#	V1 - version
#
#@Redundancy
#Scenario:807_AC2- Non-DAS learner, is made redundant outside of the last 6 months of planned learning - receives full government funding for the next 12 weeks only 
#
#        Given the apprenticeship funding band maximum is 15000
#        #And the learner is made redundant less than 6 months before the planned end date
#            
#        When an ILR file is submitted with the following data:
#            | ULN       | learner type           | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date | 
#            | learner a | programme only non-DAS | 03/08/2017 | 20/08/2018       |                 | continuing        | 12000                | 03/08/2017                          | 3000                   | 03/08/2017                            | 
#        
#        And the employment status in the ILR is:
#            | Employer   | Employment Status      | Employment Status Applies |
#            | employer 1 | in paid employment     | 02/08/2017                |
#            |            | not in paid employment | 19/02/2018                |
#              
#        Then the provider earnings and payments break down as follows:
#            | Type                           | 08/17 | 09/17 | 10/17 | ... | 01/18 | 02/18 | 03/18 | 04/18 | 05/18 |
#            | Provider Earned Total          | 1000  | 1000  | 1000  | ... | 1000  | 1000  | 1000  | 1000  | 0     |
#            | Provider Earned from SFA       | 900   | 900   | 900   | ... | 900   | 1000  | 1000  | 1000  | 0     |
#            | Provider Earned from Employer  | 100   | 100   | 100   | ... | 100   | 0     | 0     | 0     | 0     |
#            | Provider Paid by SFA           | 0     | 900   | 900   | ... | 900   | 900   | 1000  | 1000  | 1000  |
#            | Refund taken by SFA            | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     |
#            | Payment due from Employer      | 0     | 100   | 100   | ... | 100   | 100   | 0     | 0     | 0     |
#            | Refund due to employer         | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     |
#            | Levy account debited           | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     |
#            | Levy account credited          | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     |
#            | SFA Levy employer budget       | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     |
#            | SFA Levy co-funding budget     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     |
#            | SFA non-Levy co-funding budget | 900   | 900   | 900   | ... | 900   | 1000  | 1000  | 1000  | 0     |


#	----------------------------