-- Creating non-clustered index for FOREIGN KEY 'FK_AdmissionAdmissionType'
CREATE INDEX [IX_FK_AdmissionAdmissionType]
ON [dbo].[Admission]
    ([AdmissionType_admissionTypeId]);
