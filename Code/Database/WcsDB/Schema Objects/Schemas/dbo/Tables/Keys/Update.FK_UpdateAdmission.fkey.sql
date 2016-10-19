ALTER TABLE [dbo].[Update]
	ADD CONSTRAINT [FK_UpdateAdmission] 
	FOREIGN KEY (Admission_admissionId)
	REFERENCES Admission (admissionId)	

