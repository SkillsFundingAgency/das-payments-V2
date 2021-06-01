Feature: PV2-2094-2-correctly-refund-duplicate-payment-claw-backs-when-a-learner-is-added-back-into-ILR
Scenario: PV2-2094-2 correctly refund duplicate payment claw backs when a learner is added back into ILR
# this test was failing previously where ApprenticeshipId and ApprenticeshipPriceEpisodeId were set to nulls for previous refunds 
# i.e. in this two cases for refunds in R06 and R11

Given Commitment exists for learner in Period R12
And following provider payments exists in database with ApprenticeshipId
    | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
#learner b Payments
    | learner b  | R05/Current Academic Year | Aug/Current Academic Year | 600           | Learning         |
    | learner b  | R05/Current Academic Year | Sep/Current Academic Year | 600           | Learning         |
    | learner b  | R05/Current Academic Year | Oct/Current Academic Year | 600           | Learning         |
    | learner b  | R05/Current Academic Year | Nov/Current Academic Year | 600           | Learning         |
    | learner b  | R05/Current Academic Year | Dec/Current Academic Year | 600           | Learning         |
#Payments for period R06 where learner a was still submitted 
#now Learner b is removed which generates correct refunds for past 5 periods
    | learner b  | R06/Current Academic Year | Aug/Current Academic Year | -600          | Learning         |
    | learner b  | R06/Current Academic Year | Sep/Current Academic Year | -600          | Learning         |
    | learner b  | R06/Current Academic Year | Oct/Current Academic Year | -600          | Learning         |
    | learner b  | R06/Current Academic Year | Nov/Current Academic Year | -600          | Learning         |
    | learner b  | R06/Current Academic Year | Dec/Current Academic Year | -600          | Learning         |
#Payments for period R08 where learner a was still submitted
#now Learner b added back in
    | learner b  | R08/Current Academic Year | Aug/Current Academic Year | 600           | Learning         |
    | learner b  | R08/Current Academic Year | Sep/Current Academic Year | 600           | Learning         |
    | learner b  | R08/Current Academic Year | Oct/Current Academic Year | 600           | Learning         |
    | learner b  | R08/Current Academic Year | Nov/Current Academic Year | 600           | Learning         |
    | learner b  | R08/Current Academic Year | Dec/Current Academic Year | 600           | Learning         |
    | learner b  | R08/Current Academic Year | Jan/Current Academic Year | 600           | Learning         |
    | learner b  | R08/Current Academic Year | Feb/Current Academic Year | 600           | Learning         |
    | learner b  | R08/Current Academic Year | Mar/Current Academic Year | 600           | Learning         |
#Payments for period R11 where learner a was still submitted
# now Learner b REMOVED again second time
# previously system was generating double refunds this records simulates that incorrect behaviour
    | learner b  | R11/Current Academic Year | Aug/Current Academic Year | -1200         | Learning         |
    | learner b  | R11/Current Academic Year | Sep/Current Academic Year | -1200         | Learning         |
    | learner b  | R11/Current Academic Year | Oct/Current Academic Year | -1200         | Learning         |
    | learner b  | R11/Current Academic Year | Nov/Current Academic Year | -1200         | Learning         |
    | learner b  | R11/Current Academic Year | Dec/Current Academic Year | -1200         | Learning         |
    | learner b  | R11/Current Academic Year | Jan/Current Academic Year | -600          | Learning         |
    | learner b  | R11/Current Academic Year | Feb/Current Academic Year | -600          | Learning         |
    | learner b  | R11/Current Academic Year | Mar/Current Academic Year | -600          | Learning         |
When an ILR file is submitted for period R12
And After Period-end following provider payments will be generated in database
    | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
#Payments for period R12 where learner B was Added back again Thrid time
# this proves that double refunds are corrected
    | learner b  | R12/Current Academic Year | Aug/Current Academic Year | 1200          | Learning         |
    | learner b  | R12/Current Academic Year | Sep/Current Academic Year | 1200          | Learning         |
    | learner b  | R12/Current Academic Year | Oct/Current Academic Year | 1200          | Learning         |
    | learner b  | R12/Current Academic Year | Nov/Current Academic Year | 1200          | Learning         |
    | learner b  | R12/Current Academic Year | Dec/Current Academic Year | 1200          | Learning         |
    | learner b  | R12/Current Academic Year | Jan/Current Academic Year | 600           | Learning         |
    | learner b  | R12/Current Academic Year | Feb/Current Academic Year | 600           | Learning         |
    | learner b  | R12/Current Academic Year | Mar/Current Academic Year | 600           | Learning         |
    | learner b  | R12/Current Academic Year | Apr/Current Academic Year | 600           | Learning         |
    | learner b  | R12/Current Academic Year | May/Current Academic Year | 600           | Learning         |
    | learner b  | R12/Current Academic Year | Jun/Current Academic Year | 600           | Learning         |
    | learner b  | R12/Current Academic Year | Jul/Current Academic Year | 600           | Learning         |
