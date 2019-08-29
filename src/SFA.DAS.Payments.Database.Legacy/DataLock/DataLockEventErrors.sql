CREATE TABLE [DataLock].[DataLockEventErrors]
(
	DataLockEventId			uniqueidentifier			NOT NULL,
	ErrorCode				varchar(15)		NOT NULL,
	SystemDescription		nvarchar(255)	NOT NULL,
	PRIMARY KEY (DataLockEventId, ErrorCode)
)
