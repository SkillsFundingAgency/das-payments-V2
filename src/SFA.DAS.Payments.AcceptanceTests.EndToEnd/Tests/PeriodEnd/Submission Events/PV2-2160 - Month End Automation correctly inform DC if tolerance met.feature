Feature: PV2-2160 - Month End Automation correctly inform DC if tolerance met
	In order to know if I can continue with period end
	As Data Collections
	I want to know if the current period end submission is valid

Scenario Outline: PV2-2160 Period end submission is within configured tolerance 
	Given there are submission summaries for <Collection_Period>
	And the submission percentage <Percentage_Tolerance> within tolerance
	When DC request period end submission window validation
	Then DC job is updated with <Job_Status> status

Examples:
		| Collection_Period         | Percentage_Tolerance | Job_Status |
		| R01/Current Academic Year | is                   | 2          |
		| R01/Current Academic Year | is not               | 3          |

