namespace eShop.Ordering.API.Application.DomainEventHandlers;

public class OrderShippedDomainEventHandler(
    IOrderRepository orderRepository,
    IBuyerRepository buyerRepository,
    IOrderingIntegrationEventService orderingIntegrationEventService,
    ILogger<OrderShippedDomainEventHandler> logger)
    : INotificationHandler<OrderShippedDomainEvent>
{
    public async Task Handle(OrderShippedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(orderRepository);
        ArgumentNullException.ThrowIfNull(buyerRepository);
        ArgumentNullException.ThrowIfNull(orderingIntegrationEventService);
        ArgumentNullException.ThrowIfNull(logger);

        OrderingApiTrace.LogOrderStatusUpdated(logger, domainEvent.Order.Id, OrderStatus.Shipped);

        var order = await orderRepository.GetAsync(domainEvent.Order.Id);
        var buyer = await buyerRepository.FindByIdAsync(order.BuyerId.Value);

        var integrationEvent = new OrderStatusChangedToShippedIntegrationEvent(domainEvent.Order.Id, order.OrderStatus, buyer.Name, buyer.IdentityGuid);
        await orderingIntegrationEventService.AddAndSaveEventAsync(integrationEvent);
    }
}
