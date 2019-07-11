CREATE TABLE [DataLock].[DataLockEventCommitmentVersions]
(
	DataLockEventId				uniqueidentifier			NOT NULL,
	CommitmentVersion			VARCHAR(25)			NOT NULL,
	CommitmentStartDate			date			NOT NULL,
	CommitmentStandardCode		bigint			NULL,
	CommitmentProgrammeType		int				NULL,
	CommitmentFrameworkCode		int				NULL,
	CommitmentPathwayCode		int				NULL,
	CommitmentNegotiatedPrice	decimal(12,5)	NOT NULL,
	CommitmentEffectiveDate		date			NOT NULL
)
