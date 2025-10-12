namespace eCommerce.Core.DTO;

public record ProductResponse(Guid ProductId, string? ProductName, string? Category, double? UnitPrice, int? QuantityInStock)
{
    public ProductResponse() : this(default, default, default, default, default)
    {
    }
}
