﻿ALTER TABLE [dbo].[ExternalSource]
    ADD CONSTRAINT [PK_ExternalSource] PRIMARY KEY CLUSTERED ([externalSourceId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

