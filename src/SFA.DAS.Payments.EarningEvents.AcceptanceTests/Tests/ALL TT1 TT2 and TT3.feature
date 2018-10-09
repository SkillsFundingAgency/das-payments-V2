Feature: ALL TT1 TT2 and TT3

Background:

	Given the current processing period is 10
	And the payments are for the current collection year

	And the earnings calculator generates the following FM36 price episodes:
	| PriceEpisodeIdentifier | AgreedPrice | StartDate  | ActualEndDate | PlannedEndDate |
	| P1                     | 1000        | 06/08/2018 |               | 05/08/2019     |
	
@Non-DAS
@Learning (TT1)
@Completion (TT2)
@Balancing (TT3)
@CoInvested

Scenario: Contract Type 2 Learning payment

	When earning calculator event is received

	Then the earning events component will generate the following earning events:
	| PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource            | Amount |
	| p1                     | 9      | 10000 | Learning (TT1)  | CoInvestedSfa (FS2)      | 900    |
	| p1                     | 9      | 10000 | Learning (TT1)  | CoInvestedEmployer (FS3) | 100    |

Scenario: Contract Type 2 On Programme Completion payment

	When earning calculator event is received

	Then the earning events component will generate the following earning events:
	| PriceEpisodeIdentifier | Period | ULN   | TransactionType  | FundingSource            | Amount |
	| p1                     | 10     | 10000 | Completion (TT2) | CoInvestedSfa (FS2)      | 2700   |
	| p1                     | 10     | 10000 | Completion (TT2) | CoInvestedEmployer (FS3) | 300    |


Scenario: Contract Type 2 On Programme Balancing payment

	When earning calculator event is received

	Then the earning events component will generate the following earning events:
	| PriceEpisodeIdentifier | Period | ULN   | TransactionType | FundingSource            | Amount |
	| p1                     | 10     | 10000 | Balancing (TT3) | CoInvestedSfa (FS2)      | 2700   |
	| p1                     | 10     | 10000 | Balancing (TT3) | CoInvestedEmployer (FS3) | 300    |