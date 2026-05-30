using eCommerce.Core.DTO;

namespace eCommerce.Core.Entities;

public class Product
{
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? Category { get; set; }
    public double? UnitPrice { get; set; }
    public int? QuantityInStock { get; set; }
}
