Feature: PV2-2094-1-Prevent-duplicate-payment-claw-backs-when-a-learner-is-deleted-from-the-ILR
Scenario: PV2-2094 Prevent duplicate payment claw backs when a learner is deleted from the ILR

Given Commitment exists for learner in Period R08
And following provider payments exists in database without ApprenticeshipId
    | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
#Payments for period R05 where 2 learners were submitted    
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
When an ILR file is submitted for period R08
And 8 required payments are generated
And After Period-end following provider payments will be generated in database
    | Learner ID | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | learner b  | R08/Current Academic Year | Aug/Current Academic Year | 600           | Learning         |
    | learner b  | R08/Current Academic Year | Sep/Current Academic Year | 600           | Learning         |
    | learner b  | R08/Current Academic Year | Oct/Current Academic Year | 600           | Learning         |
    | learner b  | R08/Current Academic Year | Nov/Current Academic Year | 600           | Learning         |
    | learner b  | R08/Current Academic Year | Dec/Current Academic Year | 600           | Learning         |
    | learner b  | R08/Current Academic Year | Jan/Current Academic Year | 600           | Learning         |
    | learner b  | R08/Current Academic Year | Feb/Current Academic Year | 600           | Learning         |
    | learner b  | R08/Current Academic Year | Mar/Current Academic Year | 600           | Learning         |
When an ILR file is submitted for period R11
And 18 required payments are generated
Then After Period-end following provider payments will be generated in database
    | Learner ID | Collection Period          | Delivery Period           | Levy Payments | Transaction Type |
# because old payments do not have clawbackpaymenteventId every single previous payments will be reversed
    | learner b  | R11/Current Academic Year | Aug/Current Academic Year | -600          | Learning         |
    | learner b  | R11/Current Academic Year | Sep/Current Academic Year | -600          | Learning         |
    | learner b  | R11/Current Academic Year | Oct/Current Academic Year | -600          | Learning         |
    | learner b  | R11/Current Academic Year | Nov/Current Academic Year | -600          | Learning         |
    | learner b  | R11/Current Academic Year | Dec/Current Academic Year | -600          | Learning         |
    | learner b  | R11/Current Academic Year | Aug/Current Academic Year | 600           | Learning         |
    | learner b  | R11/Current Academic Year | Sep/Current Academic Year | 600           | Learning         |
    | learner b  | R11/Current Academic Year | Oct/Current Academic Year | 600           | Learning         |
    | learner b  | R11/Current Academic Year | Nov/Current Academic Year | 600           | Learning         |
    | learner b  | R11/Current Academic Year | Dec/Current Academic Year | 600           | Learning         |
# this is the actual reversal from R08
    | learner b  | R11/Current Academic Year | Aug/Current Academic Year | -600          | Learning         |
    | learner b  | R11/Current Academic Year | Sep/Current Academic Year | -600          | Learning         |
    | learner b  | R11/Current Academic Year | Oct/Current Academic Year | -600          | Learning         |
    | learner b  | R11/Current Academic Year | Nov/Current Academic Year | -600          | Learning         |
    | learner b  | R11/Current Academic Year | Dec/Current Academic Year | -600          | Learning         |
    | learner b  | R11/Current Academic Year | Jan/Current Academic Year | -600          | Learning         |
    | learner b  | R11/Current Academic Year | Feb/Current Academic Year | -600          | Learning         |
    | learner b  | R11/Current Academic Year | Mar/Current Academic Year | -600          | Learning         |
# all the payments still adds up to zero