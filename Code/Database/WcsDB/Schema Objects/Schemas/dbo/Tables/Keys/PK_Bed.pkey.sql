﻿ALTER TABLE [dbo].[Bed]
    ADD CONSTRAINT [PK_Bed] PRIMARY KEY CLUSTERED ([bedId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

