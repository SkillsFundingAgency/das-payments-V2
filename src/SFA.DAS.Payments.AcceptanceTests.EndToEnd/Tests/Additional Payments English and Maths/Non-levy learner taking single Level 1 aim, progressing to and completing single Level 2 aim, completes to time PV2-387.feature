Feature: Non-levy learner, taking single Level 1 aim, progressing to and completing single Level 2 aim, completes to time PV2-387
	As a provider,
	I want a non-levy learner taking single Level 1 aim, progressing to and completing single Level 2 aim, completes to time, to be paid the correct amount
	So that I am accurately paid my apprenticeship provision.
Scenario Outline: Non-levy learner taking single Level 1 aim, progressing to and completing single Level 2 aim, completes to time PV2-387
	Given the following learners
		| Learner Reference Number | Uln      |
		| abc123                   | 12345678 |
	And the following aims
		| Aim Type         | Aim Reference | Start Date                | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type             | Completion Status |
		| Maths or English | 12345         | 06/Aug/Last Academic Year | 12 months        |                 | 1                   | 593            | 1            | 20             | 16-18 Apprenticeship Non-Levy | continuing        |
		| Programme        | ZPROG001      | 06/Aug/Last Academic Year | 24 months        |                 | 2                   | 593            | 1            | 20             | 16-18 Apprenticeship Non-Levy | continuing        |
	And price details as follows
		| Price Details     | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
		| 1st price details | 0                    | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | Act2          | 1                   | 100%                        |
		| 2nd price details | 9000                 | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | Act2          | 2                   | 90%                         |
	And the following earnings had been generated for the learner
		| Delivery Period        | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish |
		| Aug/Last Academic Year | 600          | 0          | 0         | 39.25                      |
		| Sep/Last Academic Year | 600          | 0          | 0         | 39.25                      |
		| Oct/Last Academic Year | 600          | 0          | 0         | 39.25                      |
		| Nov/Last Academic Year | 600          | 0          | 0         | 39.25                      |
		| Dec/Last Academic Year | 600          | 0          | 0         | 39.25                      |
		| Jan/Last Academic Year | 600          | 0          | 0         | 39.25                      |
		| Feb/Last Academic Year | 600          | 0          | 0         | 39.25                      |
		| Mar/Last Academic Year | 600          | 0          | 0         | 39.25                      |
		| Apr/Last Academic Year | 600          | 0          | 0         | 39.25                      |
		| May/Last Academic Year | 600          | 0          | 0         | 39.25                      |
		| Jun/Last Academic Year | 600          | 0          | 0         | 39.25                      |
		| Jul/Last Academic Year | 600          | 0          | 0         | 39.25                      |
	And the following provider payments had been generated
		| Collection Period      | Delivery Period        | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type           |
		| R01/Last Academic Year | Aug/Last Academic Year | 540                    | 60                          | 0                         | Learning                   |
		| R02/Last Academic Year | Sep/Last Academic Year | 540                    | 60                          | 0                         | Learning                   |
		| R03/Last Academic Year | Oct/Last Academic Year | 540                    | 60                          | 0                         | Learning                   |
		| R04/Last Academic Year | Nov/Last Academic Year | 540                    | 60                          | 0                         | Learning                   |
		| R05/Last Academic Year | Dec/Last Academic Year | 540                    | 60                          | 0                         | Learning                   |
		| R06/Last Academic Year | Jan/Last Academic Year | 540                    | 60                          | 0                         | Learning                   |
		| R07/Last Academic Year | Feb/Last Academic Year | 540                    | 60                          | 0                         | Learning                   |
		| R08/Last Academic Year | Mar/Last Academic Year | 540                    | 60                          | 0                         | Learning                   |
		| R09/Last Academic Year | Apr/Last Academic Year | 540                    | 60                          | 0                         | Learning                   |
		| R10/Last Academic Year | May/Last Academic Year | 540                    | 60                          | 0                         | Learning                   |
		| R11/Last Academic Year | Jun/Last Academic Year | 540                    | 60                          | 0                         | Learning                   |
		| R12/Last Academic Year | Jul/Last Academic Year | 540                    | 60                          | 0                         | Learning                   |
		| R01/Last Academic Year | Aug/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R02/Last Academic Year | Sep/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R03/Last Academic Year | Oct/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R04/Last Academic Year | Nov/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R05/Last Academic Year | Dec/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R06/Last Academic Year | Jan/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R07/Last Academic Year | Feb/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R08/Last Academic Year | Mar/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R09/Last Academic Year | Apr/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R10/Last Academic Year | May/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R11/Last Academic Year | Jun/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R12/Last Academic Year | Jul/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
	But aims details are changed as follows
		| Aim Type         | Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type             | Completion Status |
		| Maths or English | 12345         | 06/Aug/Last Academic Year    | 12 months        | 12 months       | 1                   | 593            | 1            | 20             | 16-18 Apprenticeship Non-Levy | completed         |
		| Programme        | ZPROG001      | 06/Aug/Last Academic Year    | 24 months        |                 | 2                   | 593            | 1            | 20             | 16-18 Apprenticeship Non-Levy | continuing        |
		| Maths or English | 67890         | 09/Aug/Current Academic Year | 12 months        |                 | 3                   | 593            | 1            | 20             | 16-18 Apprenticeship Non-Levy | continuing        |
	And price details are changed as follows
		| Price Details     | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
		| 1st price details | 0                    | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | Act2          | 1                   | 100%                        |
		| 2nd price details | 9000                 | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | Act2          | 2                   | 90%                         |
		| 3rd price details | 0                    | 09/Aug/Current Academic Year        | 0                      | 09/Aug/Current Academic Year          | Act2          | 3                   | 100%                        |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
		#pe-1
		| Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish | Aim Sequence Number |
		| Aug/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |
		| Sep/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |
		| Oct/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |
		| Nov/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |
		| Dec/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |
		| Jan/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |
		| Feb/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |
		| Mar/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |
		| Apr/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |
		| May/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |
		| Jun/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |
		| Jul/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |
		#pe-2
		| Aug/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   |
		| Sep/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   |
		| Oct/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   |
		| Nov/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   |
		| Dec/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   |
		| Jan/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   |
		| Feb/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   |
		| Mar/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   |
		| Apr/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   |
		| May/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   |
		| Jun/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   |
		| Jul/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   |
	And only the following payments will be calculated													                     
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish |
		| R01/Current Academic Year | Aug/Current Academic Year | 600          | 0          | 0         | 39.25                      |
		| R02/Current Academic Year | Sep/Current Academic Year | 600          | 0          | 0         | 39.25                      |
		| R03/Current Academic Year | Oct/Current Academic Year | 600          | 0          | 0         | 39.25                      |
		| R04/Current Academic Year | Nov/Current Academic Year | 600          | 0          | 0         | 39.25                      |
		| R05/Current Academic Year | Dec/Current Academic Year | 600          | 0          | 0         | 39.25                      |
		| R06/Current Academic Year | Jan/Current Academic Year | 600          | 0          | 0         | 39.25                      |
		| R07/Current Academic Year | Feb/Current Academic Year | 600          | 0          | 0         | 39.25                      |
		| R08/Current Academic Year | Mar/Current Academic Year | 600          | 0          | 0         | 39.25                      |
		| R09/Current Academic Year | Apr/Current Academic Year | 600          | 0          | 0         | 39.25                      |
		| R10/Current Academic Year | May/Current Academic Year | 600          | 0          | 0         | 39.25                      |
		| R11/Current Academic Year | Jun/Current Academic Year | 600          | 0          | 0         | 39.25                      |
		| R12/Current Academic Year | Jul/Current Academic Year | 600          | 0          | 0         | 39.25                      |
	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type           |
		| R01/Current Academic Year | Aug/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
		| R02/Current Academic Year | Sep/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
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
		| R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R03/Current Academic Year | Oct/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R04/Current Academic Year | Nov/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R05/Current Academic Year | Dec/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R06/Current Academic Year | Jan/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R07/Current Academic Year | Feb/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R08/Current Academic Year | Mar/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R09/Current Academic Year | Apr/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R10/Current Academic Year | May/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R11/Current Academic Year | Jun/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R12/Current Academic Year | Jul/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
	And at month end only the following provider payments will be generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type           |
		| R01/Current Academic Year | Aug/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
		| R02/Current Academic Year | Sep/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
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
		| R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R03/Current Academic Year | Oct/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R04/Current Academic Year | Nov/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R05/Current Academic Year | Dec/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R06/Current Academic Year | Jan/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R07/Current Academic Year | Feb/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R08/Current Academic Year | Mar/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R09/Current Academic Year | Apr/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R10/Current Academic Year | May/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R11/Current Academic Year | Jun/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
		| R12/Current Academic Year | Jul/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
	Examples:
		| Collection_Period         |
		| R01/Current Academic Year |
		| R02/Current Academic Year |
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