Feature: PV2-2094-Prevent-duplicate-payment-claw-backs-when-a-learner-is-deleted-from-the-ILR
Scenario: PV2-2094 Prevent duplicate payment claw backs when a learner is deleted from the ILR

Given Commitment exists - which should this match, needs to match the commitments in FM36 in say was as alex spec
When an ILR file is submitted for period R05
And After Period-end following provider payments will be generated in database
    | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | learner a  | R05/Current Academic Year | Aug/Current Academic Year | 600           | Learning         |
    | learner a  | R05/Current Academic Year | Sep/Current Academic Year | 600           | Learning         |
    | learner a  | R05/Current Academic Year | Oct/Current Academic Year | 600           | Learning         |
    | learner a  | R05/Current Academic Year | Nov/Current Academic Year | 600           | Learning         |
    | learner a  | R05/Current Academic Year | Dec/Current Academic Year | 600           | Learning         |
    | learner b  | R05/Current Academic Year | Aug/Current Academic Year | 600           | Learning         |
    | learner b  | R05/Current Academic Year | Sep/Current Academic Year | 600           | Learning         |
    | learner b  | R05/Current Academic Year | Oct/Current Academic Year | 600           | Learning         |
    | learner b  | R05/Current Academic Year | Nov/Current Academic Year | 600           | Learning         |
    | learner b  | R05/Current Academic Year | Dec/Current Academic Year | 600           | Learning         |
When an ILR file is submitted for period R06
And After Period-end following provider payments will be generated in database
    | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | learner a  | R06/Current Academic Year | Jan/Current Academic Year | 600           | Learning         |
    | learner b  | R06/Current Academic Year | Aug/Current Academic Year | -600          | Learning         |
    | learner b  | R06/Current Academic Year | Sep/Current Academic Year | -600          | Learning         |
    | learner b  | R06/Current Academic Year | Oct/Current Academic Year | -600          | Learning         |
    | learner b  | R06/Current Academic Year | Nov/Current Academic Year | -600          | Learning         |
    | learner b  | R06/Current Academic Year | Dec/Current Academic Year | -600          | Learning         |
When an ILR file is submitted for period R08
And After Period-end following provider payments will be generated in database
    | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | learner a  | R08/Current Academic Year | Feb/Current Academic Year | 600           | Learning         |
    | learner a  | R08/Current Academic Year | Mar/Current Academic Year | 600           | Learning         |
    | learner b  | R08/Current Academic Year | Aug/Current Academic Year | 600           | Learning         |
    | learner b  | R08/Current Academic Year | Sep/Current Academic Year | 600           | Learning         |
    | learner b  | R08/Current Academic Year | Oct/Current Academic Year | 600           | Learning         |
    | learner b  | R08/Current Academic Year | Nov/Current Academic Year | 600           | Learning         |
    | learner b  | R08/Current Academic Year | Dec/Current Academic Year | 600           | Learning         |
    | learner b  | R08/Current Academic Year | Jan/Current Academic Year | 600           | Learning         |
    | learner b  | R08/Current Academic Year | Feb/Current Academic Year | 600           | Learning         |
    | learner b  | R08/Current Academic Year | Mar/Current Academic Year | 600           | Learning         |
When an ILR file is submitted for period R11
Then After Period-end following provider payments will be generated in database
    | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | learner a  | R11/Current Academic Year | Apr/Current Academic Year | 600           | Learning         |
    | learner a  | R11/Current Academic Year | May/Current Academic Year | 600           | Learning         |
    | learner a  | R11/Current Academic Year | Jun/Current Academic Year | 600           | Learning         |
    | learner b  | R11/Current Academic Year | Aug/Current Academic Year | -1200         | Learning         |
    | learner b  | R11/Current Academic Year | Sep/Current Academic Year | -1200         | Learning         |
    | learner b  | R11/Current Academic Year | Oct/Current Academic Year | -1200         | Learning         |
    | learner b  | R11/Current Academic Year | Nov/Current Academic Year | -1200         | Learning         |
    | learner b  | R11/Current Academic Year | Dec/Current Academic Year | -1200         | Learning         |
    | learner b  | R11/Current Academic Year | Jan/Current Academic Year | -600          | Learning         |
    | learner b  | R11/Current Academic Year | Feb/Current Academic Year | -600          | Learning         |
    | learner b  | R11/Current Academic Year | Mar/Current Academic Year | -600          | Learning         |