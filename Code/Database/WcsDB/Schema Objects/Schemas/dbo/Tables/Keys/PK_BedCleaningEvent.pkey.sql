﻿ALTER TABLE [dbo].[BedCleaningEvent]
    ADD CONSTRAINT [PK_BedCleaningEvent] PRIMARY KEY CLUSTERED ([bedCleaningEventId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

