namespace eShop.Ordering.API.Application.Models;

public class CustomerBasket
{
    public string BuyerId { get; set; } = string.Empty;
    public List<BasketItem> Items { get; set; } = new();

    public CustomerBasket(string buyerId, List<BasketItem> items)
    {
        BuyerId = buyerId;
        Items = items;
    }
}
