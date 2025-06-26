namespace eShop.Ordering.API.Application.Commands;

// Regular CommandHandler
public class SetStockConfirmedOrderStatusCommandHandler(IOrderRepository orderRepository) : IRequestHandler<SetStockConfirmedOrderStatusCommand, bool>
{
    /// <summary>
    /// Handler which processes the command when
    /// Stock service confirms the request
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public async Task<bool> Handle(SetStockConfirmedOrderStatusCommand command, CancellationToken cancellationToken)
    {
        // Simulate a work time for confirming the stock
        await Task.Delay(10000, cancellationToken);

        var orderToUpdate = await orderRepository.GetAsync(command.OrderNumber);
        if (orderToUpdate == null)
        {
            return false;
        }

        orderToUpdate.SetStockConfirmedStatus();
        return await orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}


// Use for Idempotency in Command process
public class SetStockConfirmedOrderStatusIdentifiedCommandHandler(
    IMediator mediator,
    IRequestManager requestManager,
    ILogger<IdentifiedCommandHandler<SetStockConfirmedOrderStatusCommand, bool>> logger) : IdentifiedCommandHandler<SetStockConfirmedOrderStatusCommand, bool>(mediator, requestManager, logger)
{

    protected override bool CreateResultForDuplicateRequest()
    {
        return true; // Ignore duplicate requests for processing order.
    }
}
