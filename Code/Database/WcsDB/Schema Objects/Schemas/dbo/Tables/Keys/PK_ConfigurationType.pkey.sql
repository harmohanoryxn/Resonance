﻿ALTER TABLE [dbo].[ConfigurationType]
    ADD CONSTRAINT [PK_ConfigurationType] PRIMARY KEY CLUSTERED ([ConfigurationTypeId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

