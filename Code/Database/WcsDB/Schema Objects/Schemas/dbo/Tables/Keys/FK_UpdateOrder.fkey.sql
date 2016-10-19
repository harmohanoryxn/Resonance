ALTER TABLE [dbo].[Update]
    ADD CONSTRAINT [FK_UpdateOrder] FOREIGN KEY ([Order_orderId]) REFERENCES [dbo].[Order] ([orderId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

