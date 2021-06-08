Feature: PV2-2465-Duplicate-Price-Episode-Bug
Scenario: PV2-2465-Duplicate-Price-Episode-Bug

Given a learner with multiple price episodes and learning aims
Then the correct course date from the earlier learning aim is set on the earning event
	