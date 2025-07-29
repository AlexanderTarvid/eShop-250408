namespace eShop.Ordering.API.Application.Commands;

public record IdentifiedCommand<T, R>(T Command, Guid Id) : IRequest<R>
    where T : IRequest<R>;
