﻿ALTER TABLE [dbo].[AdmissionDoctor]
    ADD CONSTRAINT [PK_AdmissionDoctor] PRIMARY KEY CLUSTERED ([admissionDoctorId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

