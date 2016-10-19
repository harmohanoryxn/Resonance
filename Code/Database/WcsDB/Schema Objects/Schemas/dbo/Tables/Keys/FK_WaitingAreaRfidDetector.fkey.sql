ALTER TABLE [dbo].[RfidDetector]
ADD CONSTRAINT [FK_WaitingAreaRfidDetector]
    FOREIGN KEY ([WaitingArea_waitingAreaId])
    REFERENCES [dbo].[WaitingArea]
        ([waitingAreaId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
