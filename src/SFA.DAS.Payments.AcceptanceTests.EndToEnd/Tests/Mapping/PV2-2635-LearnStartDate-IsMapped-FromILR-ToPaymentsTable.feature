Feature: PV2-2635-LearnStartDate-IsMapped-FromILR-ToPaymentsTable

Scenario: PV2-2635-LearnStartDate-IsMapped-FromILR-ToPaymentsTable
    Given a Learner has been made redundant
    And a break in learning has also occurred
    And the redundancy and break in learning have been correctly recorded in the ILR
    And the delivered learning days before the break are under 75 percent
    When the learner is re-employed before the 12 weeks redundancy period is exhausted
    Then the correct LearningStartDate is set for each generated payment