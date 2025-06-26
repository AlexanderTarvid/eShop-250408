namespace eShop.Ordering.API.Application.DomainEventHandlers;

public class OrderStatusChangedToStockConfirmedDomainEventHandler(
    IOrderRepository orderRepository,
    IBuyerRepository buyerRepository,
    ILogger<OrderStatusChangedToStockConfirmedDomainEventHandler> logger,
    IOrderingIntegrationEventService orderingIntegrationEventService)
    : INotificationHandler<OrderStatusChangedToStockConfirmedDomainEvent>
{
    public async Task Handle(OrderStatusChangedToStockConfirmedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(orderRepository);
        ArgumentNullException.ThrowIfNull(buyerRepository);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(orderingIntegrationEventService);

        OrderingApiTrace.LogOrderStatusUpdated(logger, domainEvent.OrderId, OrderStatus.StockConfirmed);

        var order = await orderRepository.GetAsync(domainEvent.OrderId);
        var buyer = await buyerRepository.FindByIdAsync(order.BuyerId.Value);

        var integrationEvent = new OrderStatusChangedToStockConfirmedIntegrationEvent(domainEvent.OrderId, order.OrderStatus, buyer.Name, buyer.IdentityGuid);
        await orderingIntegrationEventService.AddAndSaveEventAsync(integrationEvent);
    }
}
