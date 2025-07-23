namespace eShop.Ordering.API.Application.DomainEventHandlers;

public partial class OrderCancelledDomainEventHandler(IOrderRepository orderRepository,
    ILogger<OrderCancelledDomainEventHandler> logger,
    IBuyerRepository buyerRepository,
    IOrderingIntegrationEventService orderingIntegrationEventService)
    : INotificationHandler<OrderCancelledDomainEvent>
{
    private readonly IOrderRepository _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    private readonly IBuyerRepository _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IOrderingIntegrationEventService _orderingIntegrationEventService = orderingIntegrationEventService;

    public async Task Handle(OrderCancelledDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        OrderingApiTrace.LogOrderStatusUpdated(_logger, domainEvent.Order.Id, OrderStatus.Cancelled);

        var order = await _orderRepository.GetAsync(domainEvent.Order.Id);
        var buyer = await _buyerRepository.FindByIdAsync(order.BuyerId.Value);

        var integrationEvent = new OrderStatusChangedToCancelledIntegrationEvent(order.Id, order.OrderStatus, buyer.Name, buyer.IdentityGuid);
        await _orderingIntegrationEventService.AddAndSaveEventAsync(integrationEvent);
    }
}
