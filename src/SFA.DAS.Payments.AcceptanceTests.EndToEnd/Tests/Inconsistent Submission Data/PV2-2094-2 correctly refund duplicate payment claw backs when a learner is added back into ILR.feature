Feature: PV2-2094-2-correctly-refund-duplicate-payment-claw-backs-when-a-learner-is-added-back-into-ILR
Scenario: PV2-2094-2 correctly refund duplicate payment claw backs when a learner is added back into ILR
# this test was failing previously where ApprenticeshipId and ApprenticeshipPriceEpisodeId were set to nulls for previous refunds 
# i.e. in this two cases for refunds in R06 and R11

Given Commitment exists for learner in Period R12
And following provider payments exists in database with ApprenticeshipId
    | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
#learner b Payments
    | learner b  | R05/2025-26 Academic Year | Aug/2025-26 Academic Year | 600           | Learning         |
    | learner b  | R05/2025-26 Academic Year | Sep/2025-26 Academic Year | 600           | Learning         |
    | learner b  | R05/2025-26 Academic Year | Oct/2025-26 Academic Year | 600           | Learning         |
    | learner b  | R05/2025-26 Academic Year | Nov/2025-26 Academic Year | 600           | Learning         |
    | learner b  | R05/2025-26 Academic Year | Dec/2025-26 Academic Year | 600           | Learning         |
#Payments for period R06 where learner a was still submitted 
#now Learner b is removed which generates correct refunds for past 5 periods
    | learner b  | R06/2025-26 Academic Year | Aug/2025-26 Academic Year | -600          | Learning         |
    | learner b  | R06/2025-26 Academic Year | Sep/2025-26 Academic Year | -600          | Learning         |
    | learner b  | R06/2025-26 Academic Year | Oct/2025-26 Academic Year | -600          | Learning         |
    | learner b  | R06/2025-26 Academic Year | Nov/2025-26 Academic Year | -600          | Learning         |
    | learner b  | R06/2025-26 Academic Year | Dec/2025-26 Academic Year | -600          | Learning         |
#Payments for period R08 where learner a was still submitted
#now Learner b added back in
    | learner b  | R08/2025-26 Academic Year | Aug/2025-26 Academic Year | 600           | Learning         |
    | learner b  | R08/2025-26 Academic Year | Sep/2025-26 Academic Year | 600           | Learning         |
    | learner b  | R08/2025-26 Academic Year | Oct/2025-26 Academic Year | 600           | Learning         |
    | learner b  | R08/2025-26 Academic Year | Nov/2025-26 Academic Year | 600           | Learning         |
    | learner b  | R08/2025-26 Academic Year | Dec/2025-26 Academic Year | 600           | Learning         |
    | learner b  | R08/2025-26 Academic Year | Jan/2025-26 Academic Year | 600           | Learning         |
    | learner b  | R08/2025-26 Academic Year | Feb/2025-26 Academic Year | 600           | Learning         |
    | learner b  | R08/2025-26 Academic Year | Mar/2025-26 Academic Year | 600           | Learning         |
#Payments for period R11 where learner a was still submitted
# now Learner b REMOVED again second time
# previously system was generating double refunds this records simulates that incorrect behaviour
    | learner b  | R11/2025-26 Academic Year | Aug/2025-26 Academic Year | -1200         | Learning         |
    | learner b  | R11/2025-26 Academic Year | Sep/2025-26 Academic Year | -1200         | Learning         |
    | learner b  | R11/2025-26 Academic Year | Oct/2025-26 Academic Year | -1200         | Learning         |
    | learner b  | R11/2025-26 Academic Year | Nov/2025-26 Academic Year | -1200         | Learning         |
    | learner b  | R11/2025-26 Academic Year | Dec/2025-26 Academic Year | -1200         | Learning         |
    | learner b  | R11/2025-26 Academic Year | Jan/2025-26 Academic Year | -600          | Learning         |
    | learner b  | R11/2025-26 Academic Year | Feb/2025-26 Academic Year | -600          | Learning         |
    | learner b  | R11/2025-26 Academic Year | Mar/2025-26 Academic Year | -600          | Learning         |
When an ILR file is submitted for period R12
And 12 required payments are generated
And After Period-end following provider payments will be generated in database
    | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
#Payments for period R12 where learner B was Added back again Thrid time
# this proves that double refunds are corrected
    | learner b  | R12/2025-26 Academic Year | Aug/2025-26 Academic Year | 1200          | Learning         |
    | learner b  | R12/2025-26 Academic Year | Sep/2025-26 Academic Year | 1200          | Learning         |
    | learner b  | R12/2025-26 Academic Year | Oct/2025-26 Academic Year | 1200          | Learning         |
    | learner b  | R12/2025-26 Academic Year | Nov/2025-26 Academic Year | 1200          | Learning         |
    | learner b  | R12/2025-26 Academic Year | Dec/2025-26 Academic Year | 1200          | Learning         |
    | learner b  | R12/2025-26 Academic Year | Jan/2025-26 Academic Year | 600           | Learning         |
    | learner b  | R12/2025-26 Academic Year | Feb/2025-26 Academic Year | 600           | Learning         |
    | learner b  | R12/2025-26 Academic Year | Mar/2025-26 Academic Year | 600           | Learning         |
    | learner b  | R12/2025-26 Academic Year | Apr/2025-26 Academic Year | 600           | Learning         |
    | learner b  | R12/2025-26 Academic Year | May/2025-26 Academic Year | 600           | Learning         |
    | learner b  | R12/2025-26 Academic Year | Jun/2025-26 Academic Year | 600           | Learning         |
    | learner b  | R12/2025-26 Academic Year | Jul/2025-26 Academic Year | 600           | Learning         |
