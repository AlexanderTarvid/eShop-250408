namespace eShop.Ordering.API.Application.DomainEventHandlers;

public class UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler(
    IOrderRepository orderRepository,
    ILogger<UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler> logger) : INotificationHandler<BuyerAndPaymentMethodVerifiedDomainEvent>
{

    // Domain Logic comment:
    // When the Buyer and Buyer's payment method have been created or verified that they existed, 
    // then we can update the original Order with the BuyerId and PaymentId (foreign keys)
    public async Task Handle(BuyerAndPaymentMethodVerifiedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var orderToUpdate = await orderRepository.GetAsync(domainEvent.OrderId);
        orderToUpdate.SetPaymentMethodVerified(domainEvent.Buyer.Id, domainEvent.Payment.Id); 
        OrderingApiTrace.LogOrderPaymentMethodUpdated(logger, domainEvent.OrderId, nameof(domainEvent.Payment), domainEvent.Payment.Id);
    }
}
