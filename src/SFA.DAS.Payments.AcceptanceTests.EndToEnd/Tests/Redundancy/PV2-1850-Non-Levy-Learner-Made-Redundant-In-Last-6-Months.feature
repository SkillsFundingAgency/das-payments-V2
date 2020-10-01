Feature: PV2-1850-Non-Levy-Learner-Made-Redundant-In-Last-6-Months
Scenario: PV2-1850-Non-Levy-Learner-Made-Redundant-In-Last-6-Months

Given a learner funded by a non levy paying employer is made redundant
And there are less than 6 months remaining of the planned learning
And the learner does not find alternative employment
And the ILR submission for the learner contains 'Price episode read status code' not equal to '0'
And the 'Price episode read start date' shows date of redundancy is within 6mths of planned end date
When the Provider submission is processed for payment
Then  fund the remaining monthly instalments of the learning from Funding Source 2 (100% SFA funding) from the date of the Price episode read start date
And continue to fund the monthly instalments prior to redundancy date as per existing ACT2 rules, Funding Source 2 (95%) and Funding Source 3 (5%)