﻿Feature: Provider resubmits ILR
	As a Provider 
	I would like to be able to submit my ILR files multiple times in the same period

Scenario: ILR resubmission after original ILR submission payments have been stored
	Given the current collection period is R02
	And the SFA contribution percentage is 90%
	And the payments are for the current collection year
	And the provider has submitted an ILR file which has generated the following contract type "2" payments:
	| Delivery Period | Transaction Type | Funding Source       | Amount |
	| 1               | Learning (TT1)   | Co-Invested Sfa      | 900    |
	| 1               | Learning (TT1)   | Co-Invested Employer | 100    |
	When the provider re-submits an ILR file which triggers the following contract type "2" funding source payments:
	| Delivery Period | Transaction Type | Funding Source       | Amount |
	| 1               | Learning (TT1)   | Co-Invested Sfa      | 900    |
	| 1               | Completion (TT2) | Co-Invested Sfa      | 2700   |
	| 1               | Balancing (TT3)  | Co-Invested Sfa      | 1800   |
	| 1               | Learning (TT1)   | Co-Invested Employer | 100    |
	| 1               | Completion (TT2) | Co-Invested Employer | 300    |
	| 1               | Balancing (TT3)  | Co-Invested Employer | 1350   |
	Then the provider payments service should remove all payments for job id "12345"
	And the provider payments service will store the following payments:
	| Delivery Period | TransactionType  | FundingSource        | Amount |
	| 1               | Learning (TT1)   | Co-Invested Sfa      | 900    |
	| 1               | Completion (TT2) | Co-Invested Sfa      | 2700   |
	| 1               | Balancing (TT3)  | Co-Invested Sfa      | 1800   |
	| 1               | Learning (TT1)   | Co-Invested Employer | 100    |
	| 1               | Completion (TT2) | Co-Invested Employer | 300    |
	| 1               | Balancing (TT3)  | Co-Invested Employer | 1350   |