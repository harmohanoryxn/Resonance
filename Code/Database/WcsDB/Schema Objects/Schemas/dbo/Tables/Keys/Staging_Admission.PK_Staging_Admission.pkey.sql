ALTER TABLE [dbo].[Staging_Admission]
	ADD CONSTRAINT [PK_Staging_Admission]
	PRIMARY KEY (admissionExternalSource, admissionExternalId)