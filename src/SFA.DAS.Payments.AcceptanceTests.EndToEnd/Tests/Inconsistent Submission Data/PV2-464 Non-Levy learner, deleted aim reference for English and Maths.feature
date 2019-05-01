﻿@ignore
Feature: Non-levy learner, provider deletes english/maths aim, earlier paymnts will be refunded PV2-464

Scenario Outline: Non-levy learner provider deletes english/maths aim, earlier paymnts will be refunded PV2-464
	Given the following learners
        | Learner Reference Number | Uln      |
        | abc123                   | 12345678 |
	# New field - Aim Type
	And the following aims
		| Aim Type         | Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type             | Completion Status |
		| Maths or English | 12345         | 06/Aug/Current Academic Year | 12 months        |                 | 1                   | 403            | 1            | 2              | 16-18 Apprenticeship Non-Levy | continuing        |
		| Programme        | ZPROG001      | 06/Aug/Current Academic Year | 12 months        |                 | 2                   | 403            | 1            | 2              | 16-18 Apprenticeship Non-Levy | continuing        |
	And price details as follows	
	# Price details
        | Price Details     | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | 1st price details | 0                    | 06/Aug/Current Academic Year        | 0                      | 06/Aug/Current Academic Year          | Act2          | 1                   | 100%                        |
        | 2nd price details | 9000                 | 06/Aug/Current Academic Year        | 0                      | 06/Aug/Current Academic Year          | Act2          | 2                   | 90%                         |
    And the following earnings had been generated for the learner
        | Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish |
        | Aug/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | Sep/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | Oct/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | Nov/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | Dec/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | Jan/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | Feb/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | Mar/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | Apr/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | May/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | Jun/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | Jul/Current Academic Year | 600          | 0          | 0         | 39.25                      |
    And the following provider payments had been generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R01/Current Academic Year | Aug/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R02/Current Academic Year | Sep/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
	# New step 
	# Additional field Aim Type is just for readability and not used in the code
    But aims details are changed as follows
		| Aim Type  | Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type             | Completion Status |
		| Programme | ZPROG001      | 06/Oct/Current Academic Year | 12 months        |                 | 1                   | 403            | 1            | 2              | 16-18 Apprenticeship Non-Levy | continuing        |
	# New step 
	# Note the order of Aim Sequence Number
	And price details are changed as follows		
        | Price Details     | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | 2nd price details | 9000                 | 06/Aug/Current Academic Year        | 0                      | 06/Aug/Current Academic Year          | Act2          | 1                   | 90%                         |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	# New OnProgrammeMathsAndEnglish column
    Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish |
        | Aug/Current Academic Year | 600          | 0          | 0         | 0                          |
        | Sep/Current Academic Year | 600          | 0          | 0         | 0                          |
        | Oct/Current Academic Year | 600          | 0          | 0         | 0                          |
        | Nov/Current Academic Year | 600          | 0          | 0         | 0                          |
        | Dec/Current Academic Year | 600          | 0          | 0         | 0                          |
        | Jan/Current Academic Year | 600          | 0          | 0         | 0                          |
        | Feb/Current Academic Year | 600          | 0          | 0         | 0                          |
        | Mar/Current Academic Year | 600          | 0          | 0         | 0                          |
        | Apr/Current Academic Year | 600          | 0          | 0         | 0                          |
        | May/Current Academic Year | 600          | 0          | 0         | 0                          |
        | Jun/Current Academic Year | 600          | 0          | 0         | 0                          |
        | Jul/Current Academic Year | 600          | 0          | 0         | 0                          |
    And only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish |
        | R03/Current Academic Year | Aug/Current Academic Year | 600          | 0          | 0         | -39.25                     |
        | R03/Current Academic Year | Sep/Current Academic Year | 600          | 0          | 0         | -39.25                     |
        | R03/Current Academic Year | Oct/Current Academic Year | 600          | 0          | 0         | 0                          |
        | R04/Current Academic Year | Nov/Current Academic Year | 600          | 0          | 0         | 0                          |
        | R05/Current Academic Year | Dec/Current Academic Year | 600          | 0          | 0         | 0                          |
        | R06/Current Academic Year | Jan/Current Academic Year | 600          | 0          | 0         | 0                          |
        | R07/Current Academic Year | Feb/Current Academic Year | 600          | 0          | 0         | 0                          |
        | R08/Current Academic Year | Mar/Current Academic Year | 600          | 0          | 0         | 0                          |
        | R09/Current Academic Year | Apr/Current Academic Year | 600          | 0          | 0         | 0                          |
        | R10/Current Academic Year | May/Current Academic Year | 600          | 0          | 0         | 0                          |
        | R11/Current Academic Year | Jun/Current Academic Year | 600          | 0          | 0         | 0                          |
        | R12/Current Academic Year | Jul/Current Academic Year | 600          | 0          | 0         | 0                          |
	# New transaction type and SFA Fully-Funded Payments column
    And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R03/Current Academic Year | Aug/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R03/Current Academic Year | Sep/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R03/Current Academic Year | Oct/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R04/Current Academic Year | Nov/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R05/Current Academic Year | Dec/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R06/Current Academic Year | Jan/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R07/Current Academic Year | Feb/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R08/Current Academic Year | Mar/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R09/Current Academic Year | Apr/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R10/Current Academic Year | May/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R11/Current Academic Year | Jun/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R12/Current Academic Year | Jul/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R03/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | -39.25                    | OnProgrammeMathsAndEnglish |
        | R03/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | -39.25                    | OnProgrammeMathsAndEnglish |
	And at month end only the following provider payments will be generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R03/Current Academic Year | Aug/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R03/Current Academic Year | Sep/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R03/Current Academic Year | Oct/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R04/Current Academic Year | Nov/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R05/Current Academic Year | Dec/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R06/Current Academic Year | Jan/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R07/Current Academic Year | Feb/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R08/Current Academic Year | Mar/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R09/Current Academic Year | Apr/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R10/Current Academic Year | May/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R11/Current Academic Year | Jun/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R12/Current Academic Year | Jul/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R03/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | -39.25                    | OnProgrammeMathsAndEnglish |
        | R03/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | -39.25                    | OnProgrammeMathsAndEnglish |
Examples: 
        | Collection_Period         |
        | R03/Current Academic Year |
        | R04/Current Academic Year |
        | R05/Current Academic Year |
        | R06/Current Academic Year |
        | R07/Current Academic Year |
        | R08/Current Academic Year |
        | R09/Current Academic Year |
        | R10/Current Academic Year |
        | R11/Current Academic Year |
        | R12/Current Academic Year |