﻿ALTER TABLE [dbo].[Connection]
    ADD CONSTRAINT [PK_Connection] PRIMARY KEY CLUSTERED ([connectionId] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

