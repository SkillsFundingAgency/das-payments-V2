#Scenario: Payment for a DAS learner, requires learning support, doing an apprenticeship framework completes the 
#    Given levy balance > agreed price for all months
#    And the following commitments exist:
#        | commitment Id | version Id | ULN       | start date | end date   | framework code | programme type | pathway code | agreed price | status |
#        | 1             | 1          | learner a | 01/08/2018 | 01/08/2019 | 403            | 2              | 1            | 15000        | active |
#    When an ILR file is submitted with the following data:
#        | ULN       | learner type       | agreed price | start date | planned end date | actual end date | completion status | framework code | programme type | pathway code | learning support code | learning support date from | learning support date to |
#        | learner a | programme only DAS | 15000        | 06/08/2018 | 08/08/2019       | 10/08/2019      | completed         | 403            | 2              | 1            | 1                     | 06/08/2018                 | 10/08/2019			   |
#    Then the provider earnings and payments break down as follows:
#        | Type                                    | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 | ... | 07/19 | 08/19 | 09/19 |
#        | Provider Earned Total                   | 1150  | 1150  | 1150  | 1150  | 1150  | ... | 1150  | 3000  | 0     |
#        | Provider Earned from SFA                | 1150  | 1150  | 1150  | 1150  | 1150  | ... | 1150  | 3000  | 0     |
#        | Provider Earned from Employer           | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | Provider Paid by SFA                    | 0     | 1150  | 1150  | 1150  | 1150  | ... | 1150  | 1150  | 3000  |
#        | Payment due from Employer               | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | Levy account debited                    | 0     | 1000  | 1000  | 1000  | 1000  | ... | 1000  | 1000  | 3000  |
#        | SFA Levy employer budget                | 1000  | 1000  | 1000  | 1000  | 1000  | ... | 1000  | 3000  | 0     |
#        | SFA Levy co-funding budget              | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | SFA non-Levy co-funding budget          | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | SFA Levy additional payments budget     | 150   | 150   | 150   | 150   | 150   | ... | 150   | 0     | 0     |
#        | SFA non-Levy additional payments budget | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#    And the transaction types for the payments are:
#        | Payment type                 | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 | ... | 07/19 | 08/19 | 09/19 |
#        | On-program                   | 0     | 1000  | 1000  | 1000  | 1000  | ... | 1000  | 1000  | 0     |
#        | Completion                   | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 3000  |
#        | Balancing                    | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | Employer 16-18 incentive     | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | Provider 16-18 incentive     | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | Provider disadvantage uplift | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | Provider learning support    | 0     | 150   | 150   | 150   | 150   | ... | 150   | 150   | 0     |