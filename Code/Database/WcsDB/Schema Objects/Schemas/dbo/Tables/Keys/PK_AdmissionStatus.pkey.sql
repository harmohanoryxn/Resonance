﻿ALTER TABLE [dbo].[AdmissionStatus]
    ADD CONSTRAINT [PK_AdmissionStatus] PRIMARY KEY CLUSTERED ([admissionStatusId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

