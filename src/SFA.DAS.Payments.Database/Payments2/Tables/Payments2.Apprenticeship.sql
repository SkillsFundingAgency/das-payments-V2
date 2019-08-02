CREATE TABLE [Payments2].[Apprenticeship]
(	
	Id BIGINT NOT NULL  CONSTRAINT PK_Apprenticeship PRIMARY KEY,
	AccountId BIGINT NOT NULL,
	AgreementId CHAR(6) NULL, 
	AgreedOnDate Date not null,
	Uln BIGINT NOT NULL,
	Ukprn BIGINT NOT NULL,
	EstimatedStartDate Date NOT NULL,
	EstimatedEndDate Date NOT NULL,
    [Priority]      INT  NOT NULL,
	StandardCode BIGINT NULL,
	ProgrammeType INT NULL,
	FrameworkCode INT NULL,
	PathwayCode INT NULL,
	LegalEntityName NVARCHAR (100) NULL,
	TransferSendingEmployerAccountId BIGINT NULL,
	StopDate Date NULL, 
    [Status] TINYINT NOT NULL,
    [IsLevyPayer] BIT NOT NULL,
	CreationDate DATETIMEOFFSET NOT NULL CONSTRAINT DF_Apprenticeship__CreationDate DEFAULT (SYSDATETIMEOFFSET()), 
    [ApprenticeshipEmployerType] TINYINT NOT NULL CONSTRAINT DF_Apprenticeship_ApprenticeshipEmployerType DEFAULT (1),

)