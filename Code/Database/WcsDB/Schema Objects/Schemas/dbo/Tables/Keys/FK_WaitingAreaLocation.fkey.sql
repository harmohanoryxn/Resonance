ALTER TABLE [dbo].[Location]
ADD CONSTRAINT [FK_WaitingAreaLocation]
    FOREIGN KEY ([WaitingArea_waitingAreaId])
    REFERENCES [dbo].[WaitingArea]
        ([waitingAreaId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
