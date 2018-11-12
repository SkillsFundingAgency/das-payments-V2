Feature: Provider resubmits ILR
	As a Provider 
	I would like to be able to submit my ILR files multiple times in the same period

Scenario: ILR resubmission after original ILR submission payments have been stored
	Given the current collection period is R02
	And the SFA contribution percentage is 90%
	And the payments are for the current collection year
	And the provider has submitted an ILR file which has generated the following contract type 2 payments:
	| Delivery Period | Transaction Type | Funding Source     | Amount |
	| 1               | Learning (TT1)   | CoInvestedSfa      | 900    |
	| 1               | Learning (TT1)   | CoInvestedEmployer | 100    |
	When the provider re-submits an ILR file which triggers the following contract type 2 funding source payments:
	| Delivery Period | Transaction Type | Funding Source     | Amount |
	| 1               | Learning (TT1)   | CoInvestedSfa      | 900    |
	| 1               | Completion (TT2) | CoInvestedSfa      | 2700   |
	| 1               | Balancing (TT3)  | CoInvestedSfa      | 1800   |
	| 1               | Learning (TT1)   | CoInvestedEmployer | 100    |
	| 1               | Completion (TT2) | CoInvestedEmployer | 300    |
	| 1               | Balancing (TT3)  | CoInvestedEmployer | 1350   |
	Then the provider payments service should remove all payments for the previous Ilr submission
	And the provider payments service will store the following payments:
	| Delivery Period | TransactionType  | FundingSource      | Amount |
	| 1               | Learning (TT1)   | CoInvestedSfa      | 900    |
	| 1               | Completion (TT2) | CoInvestedSfa      | 2700   |
	| 1               | Balancing (TT3)  | CoInvestedSfa      | 1800   |
	| 1               | Learning (TT1)   | CoInvestedEmployer | 100    |
	| 1               | Completion (TT2) | CoInvestedEmployer | 300    |
	| 1               | Balancing (TT3)  | CoInvestedEmployer | 1350   |


Scenario: ILR resubmission with payments received prior to ILR Submission event
	Given the current collection period is R02
	And the SFA contribution percentage is 90%
	And the payments are for the current collection year
	And the provider has submitted an ILR file which has generated the following contract type 2 payments:
	| Delivery Period | Transaction Type | Funding Source     | Amount |
	| 1               | Learning (TT1)   | CoInvestedSfa      | 900    |
	| 1               | Learning (TT1)   | CoInvestedEmployer | 100    |
	When the provider re-submits an ILR file which triggers the following contract type 2 funding source payments with the ILR Submission event sent after the payments:
	| Delivery Period | Transaction Type | Funding Source     | Amount |
	| 1               | Learning (TT1)   | CoInvestedSfa      | 900    |
	| 1               | Completion (TT2) | CoInvestedSfa      | 2700   |
	| 1               | Balancing (TT3)  | CoInvestedSfa      | 1800   |
	| 1               | Learning (TT1)   | CoInvestedEmployer | 100    |
	| 1               | Completion (TT2) | CoInvestedEmployer | 300    |
	| 1               | Balancing (TT3)  | CoInvestedEmployer | 1350   |
	Then the provider payments service will store the following payments:
	| Delivery Period | TransactionType  | FundingSource      | Amount |
	| 1               | Learning (TT1)   | CoInvestedSfa      | 900    |
	| 1               | Completion (TT2) | CoInvestedSfa      | 2700   |
	| 1               | Balancing (TT3)  | CoInvestedSfa      | 1800   |
	| 1               | Learning (TT1)   | CoInvestedEmployer | 100    |
	| 1               | Completion (TT2) | CoInvestedEmployer | 300    |
	| 1               | Balancing (TT3)  | CoInvestedEmployer | 1350   |
	And the provider payments service should remove all payments for the previous Ilr submission
