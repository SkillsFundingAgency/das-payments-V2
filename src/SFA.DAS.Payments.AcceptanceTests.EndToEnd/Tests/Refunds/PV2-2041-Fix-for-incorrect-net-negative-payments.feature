Feature: PV2-2012-refund-issues
Scenario: PV2-2012-net-negative-earnings


Given a learner has submitted an ilr
    And the following payments exist
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co Funded Payments | Sfa Fully Funded Payments | Transaction Type                 |
        | R03/Current Academic Year | R01/Current Academic Year | 0                      | 6.40                        | 0                         | Learning                         |
        | R03/Current Academic Year | R02/Current Academic Year | 0                      | 6.4                         | 0                         | Learning                         |
        | R03/Current Academic Year | R03/Current Academic Year | 0                      | 6.4                         | 0                         | Learning                         |
        | R04/Current Academic Year | R03/Current Academic Year | 0                      | -6.4                        | 0                         | Learning                         |
        | R03/Current Academic Year | R01/Current Academic Year | 57.6                   | 0                           | 0                         | Learning                         |
        | R03/Current Academic Year | R02/Current Academic Year | 57.6                   | 0                           | 0                         | Learning                         |
        | R03/Current Academic Year | R03/Current Academic Year | 57.6                   | 0                           | 0                         | Learning                         |
        | R04/Current Academic Year | R03/Current Academic Year | -57.6                  | 0                           | 0                         | Learning                         |
        | R03/Current Academic Year | R01/Current Academic Year | 0                      | 0                           | 500                       | Second16To18EmployerIncentive    |
        | R04/Current Academic Year | R01/Current Academic Year | 0                      | 0                           | -500                      | Second16To18EmployerIncentive    |
        | R03/Current Academic Year | R01/Current Academic Year | 0                      | 0                           | 500                       | Second16To18ProviderIncentive    |
        | R04/Current Academic Year | R01/Current Academic Year | 0                      | 0                           | -500                      | Second16To18ProviderIncentive    |
        | R03/Current Academic Year | R01/Current Academic Year | 0                      | 0                           | 10                        | OnProgramme16To18FrameworkUplift |
        | R03/Current Academic Year | R02/Current Academic Year | 0                      | 0                           | 10                        | OnProgramme16To18FrameworkUplift |
        | R03/Current Academic Year | R03/Current Academic Year | 0                      | 0                           | 10                        | OnProgramme16To18FrameworkUplift |
        | R04/Current Academic Year | R01/Current Academic Year | 0                      | 0                           | -10                       | OnProgramme16To18FrameworkUplift |
        | R04/Current Academic Year | R02/Current Academic Year | 0                      | 0                           | -10                       | OnProgramme16To18FrameworkUplift |
        | R04/Current Academic Year | R03/Current Academic Year | 0                      | 0                           | -10                       | OnProgramme16To18FrameworkUplift |
        | R04/Current Academic Year | R03/Current Academic Year | 0                      | 0                           | -10                       | OnProgramme16To18FrameworkUplift |                   
        
When the learner submits in R12
