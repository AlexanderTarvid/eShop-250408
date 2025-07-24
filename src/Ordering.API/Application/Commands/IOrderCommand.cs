namespace eShop.Ordering.API.Application.Commands;

public interface IOrderCommand : IRequest<bool>
{
    int OrderNumber { get; }
}
