namespace eShop.Ordering.API.Application.DomainEventHandlers;

public class OrderShippedDomainEventHandler(
    IOrderRepository orderRepository,
    ILogger<OrderShippedDomainEventHandler> logger,
    IBuyerRepository buyerRepository,
    IOrderingIntegrationEventService orderingIntegrationEventService) : INotificationHandler<OrderShippedDomainEvent>
{

    public async Task Handle(OrderShippedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        OrderingApiTrace.LogOrderStatusUpdated(logger, domainEvent.Order.Id, OrderStatus.Shipped);

        var order = await orderRepository.GetAsync(domainEvent.Order.Id);
        var buyer = await buyerRepository.FindByIdAsync(order.BuyerId.Value);

        var integrationEvent = new OrderStatusChangedToShippedIntegrationEvent(order.Id, order.OrderStatus, buyer.Name, buyer.IdentityGuid);
        await orderingIntegrationEventService.AddAndSaveEventAsync(integrationEvent);
    }
}
