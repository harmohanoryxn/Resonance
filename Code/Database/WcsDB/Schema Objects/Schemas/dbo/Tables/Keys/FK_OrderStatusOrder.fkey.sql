ALTER TABLE [dbo].[Order]
    ADD CONSTRAINT [FK_OrderStatusOrder] FOREIGN KEY ([orderStatusId]) REFERENCES [dbo].[OrderStatus] ([orderStatusId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

