namespace eShop.Ordering.API.Application.DomainEventHandlers;

public partial class OrderCancelledDomainEventHandler(
    IOrderRepository orderRepository,
    ILogger<OrderCancelledDomainEventHandler> logger,
    IBuyerRepository buyerRepository,
    IOrderingIntegrationEventService orderingIntegrationEventService)
                : INotificationHandler<OrderCancelledDomainEvent>
{
    public async Task Handle(OrderCancelledDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        OrderingApiTrace.LogOrderStatusUpdated(logger, domainEvent.Order.Id, OrderStatus.Cancelled);

        var order = await orderRepository.GetAsync(domainEvent.Order.Id);
        var buyer = await buyerRepository.FindByIdAsync(order.BuyerId.Value);

        var integrationEvent = new OrderStatusChangedToCancelledIntegrationEvent(order.Id, order.OrderStatus, buyer.Name, buyer.IdentityGuid);
        await orderingIntegrationEventService.AddAndSaveEventAsync(integrationEvent);
    }
}
