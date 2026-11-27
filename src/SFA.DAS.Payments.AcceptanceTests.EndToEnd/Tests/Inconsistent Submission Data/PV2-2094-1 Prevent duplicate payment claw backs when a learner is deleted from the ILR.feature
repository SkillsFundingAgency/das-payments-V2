Feature: PV2-2094-1-Prevent-duplicate-payment-claw-backs-when-a-learner-is-deleted-from-the-ILR
Scenario: PV2-2094 Prevent duplicate payment claw backs when a learner is deleted from the ILR

Given Commitment exists for learner in Period R08
And following provider payments exists in database without ApprenticeshipId
    | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
#Payments for period R05 where 2 learners were submitted    
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
When an ILR file is submitted for period R08
And 8 required payments are generated
And After Period-end following provider payments will be generated in database
    | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | learner b  | R08/2025-26 Academic Year | Aug/2025-26 Academic Year | 600           | Learning         |
    | learner b  | R08/2025-26 Academic Year | Sep/2025-26 Academic Year | 600           | Learning         |
    | learner b  | R08/2025-26 Academic Year | Oct/2025-26 Academic Year | 600           | Learning         |
    | learner b  | R08/2025-26 Academic Year | Nov/2025-26 Academic Year | 600           | Learning         |
    | learner b  | R08/2025-26 Academic Year | Dec/2025-26 Academic Year | 600           | Learning         |
    | learner b  | R08/2025-26 Academic Year | Jan/2025-26 Academic Year | 600           | Learning         |
    | learner b  | R08/2025-26 Academic Year | Feb/2025-26 Academic Year | 600           | Learning         |
    | learner b  | R08/2025-26 Academic Year | Mar/2025-26 Academic Year | 600           | Learning         |
When an ILR file is submitted for period R11
And 18 required payments are generated
Then After Period-end following provider payments will be generated in database
    | Learner ID | Collection Period          | Delivery Period           | Levy Payments | Transaction Type |
# because old payments do not have clawbackpaymenteventId every single previous payments will be reversed
    | learner b  | R11/2025-26 Academic Year | Aug/2025-26 Academic Year | -600          | Learning         |
    | learner b  | R11/2025-26 Academic Year | Sep/2025-26 Academic Year | -600          | Learning         |
    | learner b  | R11/2025-26 Academic Year | Oct/2025-26 Academic Year | -600          | Learning         |
    | learner b  | R11/2025-26 Academic Year | Nov/2025-26 Academic Year | -600          | Learning         |
    | learner b  | R11/2025-26 Academic Year | Dec/2025-26 Academic Year | -600          | Learning         |
    | learner b  | R11/2025-26 Academic Year | Aug/2025-26 Academic Year | 600           | Learning         |
    | learner b  | R11/2025-26 Academic Year | Sep/2025-26 Academic Year | 600           | Learning         |
    | learner b  | R11/2025-26 Academic Year | Oct/2025-26 Academic Year | 600           | Learning         |
    | learner b  | R11/2025-26 Academic Year | Nov/2025-26 Academic Year | 600           | Learning         |
    | learner b  | R11/2025-26 Academic Year | Dec/2025-26 Academic Year | 600           | Learning         |
# this is the actual reversal from R08
    | learner b  | R11/2025-26 Academic Year | Aug/2025-26 Academic Year | -600          | Learning         |
    | learner b  | R11/2025-26 Academic Year | Sep/2025-26 Academic Year | -600          | Learning         |
    | learner b  | R11/2025-26 Academic Year | Oct/2025-26 Academic Year | -600          | Learning         |
    | learner b  | R11/2025-26 Academic Year | Nov/2025-26 Academic Year | -600          | Learning         |
    | learner b  | R11/2025-26 Academic Year | Dec/2025-26 Academic Year | -600          | Learning         |
    | learner b  | R11/2025-26 Academic Year | Jan/2025-26 Academic Year | -600          | Learning         |
    | learner b  | R11/2025-26 Academic Year | Feb/2025-26 Academic Year | -600          | Learning         |
    | learner b  | R11/2025-26 Academic Year | Mar/2025-26 Academic Year | -600          | Learning         |
# all the payments still adds up to zero