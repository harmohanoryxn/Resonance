ALTER TABLE [dbo].[Doctor]
	ADD CONSTRAINT [FK_ExternalSourceDoctor] 
	FOREIGN KEY (externalSourceId)
	REFERENCES ExternalSource (externalSourceId)	

