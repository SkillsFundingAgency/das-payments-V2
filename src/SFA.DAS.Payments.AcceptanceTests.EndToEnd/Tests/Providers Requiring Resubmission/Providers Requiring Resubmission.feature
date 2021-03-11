Feature: Providers Requiring Resubmission
	At period end when calculating payments the application should use the most up-to date Data Locks position when performing payments calculations.
	The Payments Calc must automatically recalculate data-locks for all apprenticeships that have had a successful change of circumstance
	and publish any new Data Locks through to the Data Match Report for the provider to investigate.

Scenario: Apprenticeship updated before ILR submitted by provider during current collection period
	Given there is no previous submission from provider in current collection period
	When there is a change at approvals side
	Then new record will be added to the ProviderRequiringReprocessing table

Scenario: Apprenticeship updated after IRL submitted by provider during current collection period
	Given there is previous successful/unsuccessful submission from provider in current collection period
	When there is a change at approvals side
	Then new record will be added to the ProviderRequiringReprocessing table

Scenario: Apprenticeship updated before ILR submission but provider already exists in ProviderRequiringReprocessing table
	Given a provider already exists in ProviderRequiringReprocessing table
	When there is a change at approvals side but no new submission has been made by provider
	Then there should not be any change to ProviderRequiringReprocessing table 

Scenario: Existing record in ProviderRequiringReprocessing table is removed when upon successful ILR submission for provider
	Given a provider exists in ProviderRequiringReprocessing for current collection period
	When new successful (appears in latest successful jobs view) submission is processed from that provider
	Then record for provider should be deleted from the ProviderRequiringReprocessing table

Scenario: Existing record in ProviderRequiringReprocessing table is not removed when upon unsuccessful ILR submission for provider
	Given a provider exists in ProviderRequiringReprocessing for current collection period
	When new unsuccessful submission is processed from that provider
	Then record for provider should not be deleted from the ProviderRequiringReprocessing table