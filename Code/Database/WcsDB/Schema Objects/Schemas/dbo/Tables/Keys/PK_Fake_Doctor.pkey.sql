﻿ALTER TABLE [dbo].[Fake_Doctor]
    ADD CONSTRAINT [PK_Fake_Doctor] PRIMARY KEY CLUSTERED ([doctorId] ASC, [name] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);
