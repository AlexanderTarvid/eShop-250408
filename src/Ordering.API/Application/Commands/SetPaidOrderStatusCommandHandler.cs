namespace eShop.Ordering.API.Application.Commands;

// Regular CommandHandler
public class SetPaidOrderStatusCommandHandler(IOrderRepository orderRepository) : IRequestHandler<SetPaidOrderStatusCommand, bool>
{
    /// <summary>
    /// Handler which processes the command when
    /// Shipment service confirms the payment
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public async Task<bool> Handle(SetPaidOrderStatusCommand command, CancellationToken cancellationToken)
    {
        // Simulate a work time for validating the payment
        await Task.Delay(10000, cancellationToken);

        var orderToUpdate = await orderRepository.GetAsync(command.OrderNumber);
        if (orderToUpdate == null)
        {
            return false;
        }

        orderToUpdate.SetPaidStatus();
        return await orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}


// Use for Idempotency in Command process
public class SetPaidIdentifiedOrderStatusCommandHandler(
    IMediator mediator,
    IRequestManager requestManager,
    ILogger<IdentifiedCommandHandler<SetPaidOrderStatusCommand, bool>> logger)
    : IdentifiedCommandHandler<SetPaidOrderStatusCommand, bool>(mediator, requestManager, logger)
{
    protected override bool CreateResultForDuplicateRequest()
    {
        return true; // Ignore duplicate requests for processing order.
    }
}
