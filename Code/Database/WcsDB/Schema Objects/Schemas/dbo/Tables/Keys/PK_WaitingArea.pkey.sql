﻿ALTER TABLE [dbo].[WaitingArea]
    ADD CONSTRAINT [PK_WaitingArea] PRIMARY KEY CLUSTERED ([waitingAreaId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);