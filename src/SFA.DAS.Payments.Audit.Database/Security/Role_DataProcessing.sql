
CREATE ROLE [DataProcessing] AUTHORIZATION [dbo]
GO

-- Grant access rights to a specific schema in the database
GRANT 
	DELETE, 
	EXECUTE, 
	INSERT, 
	REFERENCES, 
	SELECT, 
	UPDATE, 
	ALTER,
	VIEW DEFINITION 
ON SCHEMA::[Payments2]
	TO [DataProcessing]
GO
