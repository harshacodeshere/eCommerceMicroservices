namespace BusinessLogicLayer.DTO;

public record OrderItemAddRequest(Guid ProductID, string? ProductName, string? Category, decimal UnitPrice, int Quantity)
{
    public OrderItemAddRequest() : this(default, default, default, default, default)
    {
        
    }
}
