namespace eShop.Ordering.API.Application.Commands;

// DDD and CQRS patterns comment: Note that it is recommended to implement immutable Commands
// In this case, its immutability is achieved by having all the setters as private
// plus only being able to update the data just once, when creating the object through its constructor.
// References on Immutable Commands:  
// http://cqrs.nu/Faq
// https://docs.spine3.org/motivation/immutability.html 
// http://blog.gauffin.org/2012/06/griffin-container-introducing-command-support/
// https://docs.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/how-to-implement-a-lightweight-class-with-auto-implemented-properties

using eShop.Ordering.API.Application.Models;
using eShop.Ordering.API.Extensions;

[DataContract]
public class CreateOrderCommand(List<BasketItem> basketItems, string userId, string userName, string city, string street, string state, string country, string zipcode,
    string cardNumber, string cardHolderName, DateTime cardExpiration,
    string cardSecurityNumber, int cardTypeId)
    : IRequest<bool>
{
    [DataMember]
    private readonly List<OrderItemDTO> _orderItems = basketItems.ToOrderItemsDTO().ToList();

    [DataMember]
    public string UserId { get; private set; } = userId;

    [DataMember]
    public string UserName { get; private set; } = userName;

    [DataMember]
    public string City { get; private set; } = city;

    [DataMember]
    public string Street { get; private set; } = street;

    [DataMember]
    public string State { get; private set; } = state;

    [DataMember]
    public string Country { get; private set; } = country;

    [DataMember]
    public string ZipCode { get; private set; } = zipcode;

    [DataMember]
    public string CardNumber { get; private set; } = cardNumber;

    [DataMember]
    public string CardHolderName { get; private set; } = cardHolderName;

    [DataMember]
    public DateTime CardExpiration { get; private set; } = cardExpiration;

    [DataMember]
    public string CardSecurityNumber { get; private set; } = cardSecurityNumber;

    [DataMember]
    public int CardTypeId { get; private set; } = cardTypeId;

    [DataMember]
    public IEnumerable<OrderItemDTO> OrderItems => _orderItems;
}

