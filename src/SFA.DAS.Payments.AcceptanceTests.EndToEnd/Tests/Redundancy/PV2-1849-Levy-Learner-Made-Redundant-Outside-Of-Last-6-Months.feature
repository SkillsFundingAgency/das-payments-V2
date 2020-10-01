Feature: PV2-1849-Levy-Learner-Made-Redundant-Outside-Of-Last-6-Months
Scenario: PV2-1849-Levy-Learner-Made-Redundant-Outside-Of-Last-6-Months


Given a learner funded by a levy paying employer is made redundant
And there is more than 6 months remaining of the planned learning
And the learner does not find alternative employment
And the ILR submission for the learner contains 'Price episode read status code' not equal to '0'
And the 'Price episode read start date' shows date of redundancy is more than 6mths of planned learning
When the submission is processed for payment
Then bypass the data lock rules
And fund 12 weeks of the learning from Funding Source 2, with 100% SFA funding, from the date of the Price episode read start date
And continue to fund the monthly instalments prior to redundancy date as per existing ACT1 rules (Funding Source 1)