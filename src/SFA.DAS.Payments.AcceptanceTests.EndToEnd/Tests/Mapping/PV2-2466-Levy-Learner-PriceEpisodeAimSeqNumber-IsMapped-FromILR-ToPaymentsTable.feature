Feature: PV2-2466-Levy-Learner-PriceEpisodeAimSeqNumber-IsMapped-FromILR-ToPaymentsTable

Scenario: PV2-2466-Levy-Learner-PriceEpisodeAimSeqNumber-IsMapped-FromILR-ToPaymentsTable
    Given a learner funded by a levy paying employer exists
    When A submission is processed for this learner
    Then PriceEpisodeAimSeqNumber is correctly mapped on each payment