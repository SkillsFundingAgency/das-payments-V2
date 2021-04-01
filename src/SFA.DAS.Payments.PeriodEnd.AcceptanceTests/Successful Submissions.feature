Feature: Successful Submissions
	Ensure that if an apprenticeship is updated and there are no further submissions for that apprentice's 
	provider that the provider is not included in the successful submissions list

Scenario: Successful submission followed by a change to the apprenticeship 
	Given the provider makes a successful submission
	When there is a change to the apprenticeship details for one of the provider's learners
	Then the provider should not appear in the results

Scenario: No successful submission followed by a change to the apprenticeship
	Given the provider makes a failed submission
	When there is a change to the apprenticeship details for one of the provider's learners
	Then the provider should not appear in the results

Scenario: Successful submission after a change to the apprenticeship
	Given there is a change to the apprenticeship details for one of the provider's learners
	When the provider makes a successful submission
	Then the provider should appear in the results

Scenario: Failed submission after a change to the apprenticeship after a successful submission
	Given the provider makes a successful submission
	And there is a change to the apprenticeship details for one of the provider's learners
	When the provider makes a failed submission
	Then the provider should not appear in the results

Scenario: Failed submission after a change to the apprenticeship after no successful submissions
	Given there is a change to the apprenticeship details for one of the provider's learners
	When the provider makes a failed submission
	Then the provider should not appear in the results








