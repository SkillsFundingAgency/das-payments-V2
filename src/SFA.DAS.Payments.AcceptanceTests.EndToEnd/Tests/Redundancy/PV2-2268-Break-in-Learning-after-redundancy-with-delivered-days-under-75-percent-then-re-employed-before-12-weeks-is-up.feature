Feature: PV2-2268-Break-in-Learning-after-redundancy-with-delivered-days-under-75-percent-then-re-employed-before-12-weeks-is-up
Scenario: PV2-2268-Break-in-Learning-after-redundancy-with-delivered-days-under-75-percent-then-re-employed-before-12-weeks-is-up

Given a Learner has been made redundant
And a break in learning has also occurred
And the redundancy and break in learning have been correctly recorded in the ILR
And the delivered learning days before the break are under 75 percent
When the learner is re-employed before the 12 weeks redundancy period is exhausted
Then the learner must be funded from the employer levy funds