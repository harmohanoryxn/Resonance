﻿ALTER TABLE [dbo].[ConfigurationLocation]
    ADD CONSTRAINT [PK_ConfigurationLocation] PRIMARY KEY CLUSTERED ([configurationLocationId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);
