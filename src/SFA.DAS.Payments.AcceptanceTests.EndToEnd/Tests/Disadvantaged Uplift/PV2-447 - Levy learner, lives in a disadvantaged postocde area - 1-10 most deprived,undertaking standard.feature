Feature: PV2-447 Payment for a DAS learner, lives in a disadvantaged postocde area - 1-10% most deprived, undertaking apprenticeship standard - Disadvantage Uplift is not paid
		As a provider,
		I want a levy learner living in a Disadvantaged Postcode area (1-10% most deprived) to undertake an Apprenticeship Standard course
		So that I am not paid the Disadvantage Uplift payment.
Scenario Outline: Levy learner, on standard, Disadvantage Uplift 1-10% not paid PV2-447
	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
	And the following commitments exist
		| start date                   | end date                  | agreed price | standard code | status |Standard Code | Programme Type |
		| 01/Aug/2025-26 Academic Year | 01/Aug/2026-27 Academic Year | 15000        | 50            | active |50            | 25             |
    And the provider is providing training for the following learners
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 06/Aug/2025-26 Academic Year | 12 months        | 14000                | 06/Aug/2025-26 Academic Year        | 1000                   | 06/Aug/2025-26 Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 50            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Contract Type |
		| pe-1             | 14000                | 06/Aug/2025-26 Academic Year        | 1000                   | 06/Aug/2025-26 Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         | Act1          |
	When the ILR file is submitted for the learners for collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
        | Aug/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
        | Sep/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
        | Oct/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     | 
        | Nov/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     | 
        | Dec/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     | 
        | Jan/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
        | Feb/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     | 
        | Mar/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     | 
        | Apr/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
        | May/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     | 
        | Jun/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     | 
        | Jul/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
    And at month end only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
		| R01/2025-26 Academic Year | Aug/2025-26 Academic Year | 1000         | 0          | 0         |
		| R02/2025-26 Academic Year | Sep/2025-26 Academic Year | 1000         | 0          | 0         |
		| R03/2025-26 Academic Year | Oct/2025-26 Academic Year | 1000         | 0          | 0         |
        | R04/2025-26 Academic Year | Nov/2025-26 Academic Year | 1000         | 0          | 0         |
        | R05/2025-26 Academic Year | Dec/2025-26 Academic Year | 1000         | 0          | 0         |
		| R06/2025-26 Academic Year | Jan/2025-26 Academic Year | 1000         | 0          | 0         |
		| R07/2025-26 Academic Year | Feb/2025-26 Academic Year | 1000         | 0          | 0         |
		| R08/2025-26 Academic Year | Mar/2025-26 Academic Year | 1000         | 0          | 0         |
		| R09/2025-26 Academic Year | Apr/2025-26 Academic Year | 1000         | 0          | 0         |
		| R10/2025-26 Academic Year | May/2025-26 Academic Year | 1000         | 0          | 0         |
		| R11/2025-26 Academic Year | Jun/2025-26 Academic Year | 1000         | 0          | 0         |
		| R12/2025-26 Academic Year | Jul/2025-26 Academic Year | 1000         | 0          | 0         |
	And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R01/2025-26 Academic Year | Aug/2025-26 Academic Year | 1000          | Learning         |
        | R02/2025-26 Academic Year | Sep/2025-26 Academic Year | 1000          | Learning         |
        | R03/2025-26 Academic Year | Oct/2025-26 Academic Year | 1000          | Learning         |
        | R04/2025-26 Academic Year | Nov/2025-26 Academic Year | 1000          | Learning         |
        | R05/2025-26 Academic Year | Dec/2025-26 Academic Year | 1000          | Learning         |
        | R06/2025-26 Academic Year | Jan/2025-26 Academic Year | 1000          | Learning         |
        | R07/2025-26 Academic Year | Feb/2025-26 Academic Year | 1000          | Learning         |
        | R08/2025-26 Academic Year | Mar/2025-26 Academic Year | 1000          | Learning         |
        | R09/2025-26 Academic Year | Apr/2025-26 Academic Year | 1000          | Learning         |
        | R10/2025-26 Academic Year | May/2025-26 Academic Year | 1000          | Learning         |
        | R11/2025-26 Academic Year | Jun/2025-26 Academic Year | 1000          | Learning         |
        | R12/2025-26 Academic Year | Jul/2025-26 Academic Year | 1000          | Learning         |
	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R01/2025-26 Academic Year | Aug/2025-26 Academic Year | 1000          | Learning         |
        | R02/2025-26 Academic Year | Sep/2025-26 Academic Year | 1000          | Learning         |
        | R03/2025-26 Academic Year | Oct/2025-26 Academic Year | 1000          | Learning         |
        | R04/2025-26 Academic Year | Nov/2025-26 Academic Year | 1000          | Learning         |
        | R05/2025-26 Academic Year | Dec/2025-26 Academic Year | 1000          | Learning         |
        | R06/2025-26 Academic Year | Jan/2025-26 Academic Year | 1000          | Learning         |
        | R07/2025-26 Academic Year | Feb/2025-26 Academic Year | 1000          | Learning         |
        | R08/2025-26 Academic Year | Mar/2025-26 Academic Year | 1000          | Learning         |
        | R09/2025-26 Academic Year | Apr/2025-26 Academic Year | 1000          | Learning         |
        | R10/2025-26 Academic Year | May/2025-26 Academic Year | 1000          | Learning         |
        | R11/2025-26 Academic Year | Jun/2025-26 Academic Year | 1000          | Learning         |
        | R12/2025-26 Academic Year | Jul/2025-26 Academic Year | 1000          | Learning         |
Examples: 
        | Collection_Period         | Levy Balance |
        | R01/2025-26 Academic Year | 15500        |
        | R02/2025-26 Academic Year | 14500        |
        | R03/2025-26 Academic Year | 13500        |
        | R04/2025-26 Academic Year | 12500        |
        | R05/2025-26 Academic Year | 11500        |
        | R06/2025-26 Academic Year | 10500        |
        | R07/2025-26 Academic Year | 9500         |
        | R08/2025-26 Academic Year | 8500         |
        | R09/2025-26 Academic Year | 7500         |
        | R10/2025-26 Academic Year | 6500         |
        | R11/2025-26 Academic Year | 5500         |
        | R12/2025-26 Academic Year | 4500         |