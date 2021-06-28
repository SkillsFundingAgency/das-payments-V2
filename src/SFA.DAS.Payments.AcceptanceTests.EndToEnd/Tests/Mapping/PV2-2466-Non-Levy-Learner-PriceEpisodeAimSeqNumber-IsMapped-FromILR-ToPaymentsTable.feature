Feature: PV2-2466-Non-Levy-Learner-PriceEpisodeAimSeqNumber-IsMapped-FromILR-ToPaymentsTable

Scenario: PV2-2466-Non-Levy-Learner-PriceEpisodeAimSeqNumber-IsMapped-FromILR-ToPaymentsTable
    Given a learner funded by a non levy paying employer exists
    When A submission is processed for the non levy learner
    Then PriceEpisodeAimSeqNumber is correctly mapped on each of 9 payments