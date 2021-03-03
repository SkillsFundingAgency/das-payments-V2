Feature: Period End Stop should record collection period in the db


Scenario Outline: upon period end stop, collection period is stored in the db
	Given the collection period is <Collection_Period>
	And submission window validation job has been run
	When month end stop event is received
	Then the collection period data for <Collection_Period> should be stored in the db
	
	Examples: 
	 | Collection_Period         |
	 | R01/Current Academic Year |
	 | R05/Current Academic Year |
	 | R06/Current Academic Year |
	 | R12/Current Academic Year |
	 | R14/Current Academic Year |


Scenario Outline: upon period end stop, without any submission window validation job the collection period is NOT stored in the db
	Given the collection period is <Collection_Period>
	And submission window validation job has not been run
	When month end stop event is received
	Then the collection period data for <Collection_Period> should NOT be stored in the db
	
	Examples: 
	 | Collection_Period         |
	 | R01/Current Academic Year |
	 | R05/Current Academic Year |
	 | R06/Current Academic Year |
	 | R12/Current Academic Year |
	 | R14/Current Academic Year |
