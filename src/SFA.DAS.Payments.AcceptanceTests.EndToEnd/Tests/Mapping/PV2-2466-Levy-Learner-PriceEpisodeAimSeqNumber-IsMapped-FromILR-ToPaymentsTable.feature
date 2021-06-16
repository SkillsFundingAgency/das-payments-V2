Feature: PV2-2466-Levy-Learner-PriceEpisodeAimSeqNumber-IsMapped-FromILR-ToPaymentsTable

Scenario Outline: PV2-2466-Levy-Learner-PriceEpisodeAimSeqNumber-IsMapped-FromILR-ToPaymentsTable
    Given a learner funded by a levy paying employer with PriceEpisodeAimSeqNumber: <PriceEpisodeAimSeqNumber> exists
    When A submission is processed for this learner
    Then PriceEpisodeAimSeqNumber is correctly mapped on each payment

Examples: 
| PriceEpisodeAimSeqNumber |
| 1234                     |
| 234325                   |