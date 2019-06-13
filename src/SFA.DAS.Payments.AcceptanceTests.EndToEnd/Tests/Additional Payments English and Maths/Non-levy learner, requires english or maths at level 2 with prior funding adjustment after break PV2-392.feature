@ignore
# failing due to incorrect handling of maths and english contract types
# Remaining 42% (197.82) is divided into remaining 7 months - Hence 28.26
# Restart Indicator
#For DC Integration
 #| Funding Adjustment For Prior Learning |
 #| n/a                                   |
 #| 42% for remaining period|
	
#@supports_dc_e2e
Feature: Non-levy learner, requires english or maths at level 2 with prior funding adjustment after break - COMPLETES ON TIME, RETURNS TO SAME PROVIDER PV2-392

Scenario Outline: Non-levy learner, requires english or maths at level 2 with prior funding adjustment after break PV2-392
	Given the following learners
        | Learner Reference Number | Uln      |
        | abc123                   | 12345678 |
	And the following aims
		| Aim Type         | Aim Reference | Start Date                | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type             | Completion Status |
		| Maths or English | 12345         | 06/Aug/Last Academic Year | 12 months        | 5 months        | 1                   | 593            | 1            | 20             | 16-18 Apprenticeship Non-Levy | continuing        |
		| Programme        | ZPROG001      | 06/Aug/Last Academic Year | 12 months        | 5 months        | 2                   | 593            | 1            | 20             | 16-18 Apprenticeship Non-Levy | continuing        |
	And price details as follows	
        | Price Episode Id  | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        |                   | 0                    | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | Act2          | 1                   | 100%                        |
        | 2nd price details | 15000                | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | Act2          | 2                   | 90%                         |
    And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish | Price Episode Identifier |
        | Aug/Last Academic Year | 1000         | 0          | 0         | 0                          | 2nd price details        |
        | Sep/Last Academic Year | 1000         | 0          | 0         | 0                          | 2nd price details        |
        | Oct/Last Academic Year | 1000         | 0          | 0         | 0                          | 2nd price details        |
        | Nov/Last Academic Year | 1000         | 0          | 0         | 0                          | 2nd price details        |
        | Dec/Last Academic Year | 1000         | 0          | 0         | 0                          | 2nd price details        |
        | Jan/Last Academic Year | 1000         | 0          | 0         | 0                          | 2nd price details        |
        | Feb/Last Academic Year | 1000         | 0          | 0         | 0                          | 2nd price details        |
        | Mar/Last Academic Year | 1000         | 0          | 0         | 0                          | 2nd price details        |
        | Apr/Last Academic Year | 1000         | 0          | 0         | 0                          | 2nd price details        |
        | May/Last Academic Year | 1000         | 0          | 0         | 0                          | 2nd price details        |
        | Jun/Last Academic Year | 1000         | 0          | 0         | 0                          | 2nd price details        |
        | Jul/Last Academic Year | 1000         | 0          | 0         | 0                          | 2nd price details        |
        | Aug/Last Academic Year | 0            | 0          | 0         | 39.25                      |                          |
        | Sep/Last Academic Year | 0            | 0          | 0         | 39.25                      |                          |
        | Oct/Last Academic Year | 0            | 0          | 0         | 39.25                      |                          |
        | Nov/Last Academic Year | 0            | 0          | 0         | 39.25                      |                          |
        | Dec/Last Academic Year | 0            | 0          | 0         | 39.25                      |                          |
        | Jan/Last Academic Year | 0            | 0          | 0         | 39.25                      |                          |
        | Feb/Last Academic Year | 0            | 0          | 0         | 39.25                      |                          |
        | Mar/Last Academic Year | 0            | 0          | 0         | 39.25                      |                          |
        | Apr/Last Academic Year | 0            | 0          | 0         | 39.25                      |                          |
        | May/Last Academic Year | 0            | 0          | 0         | 39.25                      |                          |
        | Jun/Last Academic Year | 0            | 0          | 0         | 39.25                      |                          |
        | Jul/Last Academic Year | 0            | 0          | 0         | 39.25                      |                          |
    And the following provider payments had been generated
        | Collection Period      | Delivery Period        | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R01/Last Academic Year | Aug/Last Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R02/Last Academic Year | Sep/Last Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R03/Last Academic Year | Oct/Last Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R04/Last Academic Year | Nov/Last Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R05/Last Academic Year | Dec/Last Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R01/Last Academic Year | Aug/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R02/Last Academic Year | Sep/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R03/Last Academic Year | Oct/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R04/Last Academic Year | Nov/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R05/Last Academic Year | Dec/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
    But aims details are changed as follows
		| Aim Type         | Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type             | Completion Status |
		| Maths or English | 12345         | 06/Aug/Current Academic Year | 7 months         |                 | 1                   | 593            | 1            | 20             | 16-18 Apprenticeship Non-Levy | continuing        |
		| Programme        | ZPROG001      | 06/Aug/Current Academic Year | 7 months         |                 | 2                   | 593            | 1            | 20             | 16-18 Apprenticeship Non-Levy | continuing        |
	And price details are changed as follows		
        | Price Episode Id  | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        |                   | 0                    | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | Act2          | 1                   | 100%                        |
        | 4th price details | 15000                | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | Act2          | 2                   | 90%                         |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
    Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier |
        | Aug/Current Academic Year | 1000         | 0          | 0         | 28.26                      | 2                   | 4th price details        |
        | Sep/Current Academic Year | 1000         | 0          | 0         | 28.26                      | 2                   | 4th price details        |
        | Oct/Current Academic Year | 1000         | 0          | 0         | 28.26                      | 2                   | 4th price details        |
        | Nov/Current Academic Year | 1000         | 0          | 0         | 28.26                      | 2                   | 4th price details        |
        | Dec/Current Academic Year | 1000         | 0          | 0         | 28.26                      | 2                   | 4th price details        |
        | Jan/Current Academic Year | 1000         | 0          | 0         | 28.26                      | 2                   | 4th price details        |
        | Feb/Current Academic Year | 1000         | 0          | 0         | 28.26                      | 2                   | 4th price details        |
        | Mar/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   | 4th price details        |
        | Apr/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   | 4th price details        |
        | May/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   | 4th price details        |
        | Jun/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   | 4th price details        |
        | Jul/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   | 4th price details        |
        | Aug/Current Academic Year | 0            | 0          | 0         | 28.26                      | 1                   |                          |
        | Sep/Current Academic Year | 0            | 0          | 0         | 28.26                      | 1                   |                          |
        | Oct/Current Academic Year | 0            | 0          | 0         | 28.26                      | 1                   |                          |
        | Nov/Current Academic Year | 0            | 0          | 0         | 28.26                      | 1                   |                          |
        | Dec/Current Academic Year | 0            | 0          | 0         | 28.26                      | 1                   |                          |
        | Jan/Current Academic Year | 0            | 0          | 0         | 28.26                      | 1                   |                          |
        | Feb/Current Academic Year | 0            | 0          | 0         | 28.26                      | 1                   |                          |
        | Mar/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   |                          |
        | Apr/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   |                          |
        | May/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   |                          |
        | Jun/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   |                          |
        | Jul/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   |                          |
    And only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish |
        | R01/Current Academic Year | Aug/Current Academic Year | 1000         | 0          | 0         | 28.26                      |
        | R02/Current Academic Year | Sep/Current Academic Year | 1000         | 0          | 0         | 28.26                      |
        | R03/Current Academic Year | Oct/Current Academic Year | 1000         | 0          | 0         | 28.26                      |
        | R04/Current Academic Year | Nov/Current Academic Year | 1000         | 0          | 0         | 28.26                      |
        | R05/Current Academic Year | Dec/Current Academic Year | 1000         | 0          | 0         | 28.26                      |
        | R06/Current Academic Year | Jan/Current Academic Year | 1000         | 0          | 0         | 28.26                      |
        | R07/Current Academic Year | Feb/Current Academic Year | 1000         | 0          | 0         | 28.26                      |
    And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R02/Current Academic Year | Sep/Current Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R03/Current Academic Year | Oct/Current Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R04/Current Academic Year | Nov/Current Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R05/Current Academic Year | Dec/Current Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R06/Current Academic Year | Jan/Current Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R07/Current Academic Year | Feb/Current Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 28.26                     | OnProgrammeMathsAndEnglish |
        | R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 28.26                     | OnProgrammeMathsAndEnglish |
        | R03/Current Academic Year | Oct/Current Academic Year | 0                      | 0                           | 28.26                     | OnProgrammeMathsAndEnglish |
        | R04/Current Academic Year | Nov/Current Academic Year | 0                      | 0                           | 28.26                     | OnProgrammeMathsAndEnglish |
        | R05/Current Academic Year | Dec/Current Academic Year | 0                      | 0                           | 28.26                     | OnProgrammeMathsAndEnglish |
        | R06/Current Academic Year | Jan/Current Academic Year | 0                      | 0                           | 28.26                     | OnProgrammeMathsAndEnglish |
        | R07/Current Academic Year | Feb/Current Academic Year | 0                      | 0                           | 28.26                     | OnProgrammeMathsAndEnglish |
	And at month end only the following provider payments will be generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R02/Current Academic Year | Sep/Current Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R03/Current Academic Year | Oct/Current Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R04/Current Academic Year | Nov/Current Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R05/Current Academic Year | Dec/Current Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R06/Current Academic Year | Jan/Current Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R07/Current Academic Year | Feb/Current Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 28.26                     | OnProgrammeMathsAndEnglish |
        | R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 28.26                     | OnProgrammeMathsAndEnglish |
        | R03/Current Academic Year | Oct/Current Academic Year | 0                      | 0                           | 28.26                     | OnProgrammeMathsAndEnglish |
        | R04/Current Academic Year | Nov/Current Academic Year | 0                      | 0                           | 28.26                     | OnProgrammeMathsAndEnglish |
        | R05/Current Academic Year | Dec/Current Academic Year | 0                      | 0                           | 28.26                     | OnProgrammeMathsAndEnglish |
        | R06/Current Academic Year | Jan/Current Academic Year | 0                      | 0                           | 28.26                     | OnProgrammeMathsAndEnglish |
        | R07/Current Academic Year | Feb/Current Academic Year | 0                      | 0                           | 28.26                     | OnProgrammeMathsAndEnglish |
Examples: 
        | Collection_Period         |
        | R01/Current Academic Year |
        | R02/Current Academic Year |
        | R03/Current Academic Year |
        | R04/Current Academic Year |
        | R05/Current Academic Year |
        | R06/Current Academic Year |
        | R07/Current Academic Year |