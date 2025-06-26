namespace eShop.Ordering.API.Application.Commands;

public class IdentifiedCommand<T, R>(T command, Guid id) : IRequest<R>
    where T : IRequest<R>
{
    public T Command { get; } = command;
    public Guid Id { get; } = id;
}
