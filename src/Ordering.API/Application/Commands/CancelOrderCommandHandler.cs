namespace eShop.Ordering.API.Application.Commands;

// Regular CommandHandler
public class CancelOrderCommandHandler(IOrderRepository orderRepository) : IRequestHandler<CancelOrderCommand, bool>
{
    /// <summary>
    /// Handler which processes the command when
    /// customer executes cancel order from app
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public async Task<bool> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
    {
        var orderToUpdate = await orderRepository.GetAsync(command.OrderNumber);
        if (orderToUpdate == null)
        {
            return false;
        }

        orderToUpdate.SetCancelledStatus();
        return await orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}

// Use for Idempotency in Command process
public class CancelOrderIdentifiedCommandHandler(
    IMediator mediator,
    IRequestManager requestManager,
    ILogger<IdentifiedCommandHandler<CancelOrderCommand, bool>> logger) : IdentifiedCommandHandler<CancelOrderCommand, bool>(mediator, requestManager, logger)
{
    protected override bool CreateResultForDuplicateRequest()
    {
        return true; // Ignore duplicate requests for processing order.
    }
}
