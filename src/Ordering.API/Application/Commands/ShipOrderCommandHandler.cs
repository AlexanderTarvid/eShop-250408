namespace eShop.Ordering.API.Application.Commands;

// Regular CommandHandler
public class ShipOrderCommandHandler(IOrderRepository orderRepository) : IRequestHandler<ShipOrderCommand, bool>
{
    /// <summary>
    /// Handler which processes the command when
    /// administrator executes ship order from app
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public async Task<bool> Handle(ShipOrderCommand command, CancellationToken cancellationToken)
    {
        var orderToUpdate = await orderRepository.GetAsync(command.OrderNumber);
        if (orderToUpdate == null)
        {
            return false;
        }

        orderToUpdate.SetShippedStatus();
        return await orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}


// Use for Idempotency in Command process
public class ShipOrderIdentifiedCommandHandler(
    IMediator mediator,
    IRequestManager requestManager,
    ILogger<IdentifiedCommandHandler<ShipOrderCommand, bool>> logger)
    : IdentifiedCommandHandler<ShipOrderCommand, bool>(mediator, requestManager, logger)
{

    protected override bool CreateResultForDuplicateRequest()
    {
        return true; // Ignore duplicate requests for processing order.
    }
}
