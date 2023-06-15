Feature: FLP-347-Spike
Scenario: FLP-347-Spike

Given A CalculatedRequiredLevyAmount event is published for a basic levy learner
When Month end is ran for the learner's provider
Then The correct payments should be generated for that learner
	