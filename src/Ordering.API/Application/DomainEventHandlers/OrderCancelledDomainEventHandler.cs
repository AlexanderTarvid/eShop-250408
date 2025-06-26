namespace eShop.Ordering.API.Application.DomainEventHandlers;

public class OrderCancelledDomainEventHandler(
    IOrderRepository orderRepository,
    IBuyerRepository buyerRepository,
    ILogger<OrderCancelledDomainEventHandler> logger,
    IOrderingIntegrationEventService orderingIntegrationEventService)
    : INotificationHandler<OrderCancelledDomainEvent>
{
    public async Task Handle(OrderCancelledDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(orderRepository);
        ArgumentNullException.ThrowIfNull(buyerRepository);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(orderingIntegrationEventService);

        OrderingApiTrace.LogOrderStatusUpdated(logger, domainEvent.Order.Id, OrderStatus.Cancelled);

        var order = await orderRepository.GetAsync(domainEvent.Order.Id);
        var buyer = await buyerRepository.FindByIdAsync(order.BuyerId.Value);

        var integrationEvent = new OrderStatusChangedToCancelledIntegrationEvent(domainEvent.Order.Id, order.OrderStatus, buyer.Name, buyer.IdentityGuid);
        await orderingIntegrationEventService.AddAndSaveEventAsync(integrationEvent);
    }
}
