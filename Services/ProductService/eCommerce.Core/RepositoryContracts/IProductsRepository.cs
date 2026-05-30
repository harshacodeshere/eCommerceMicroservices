using eCommerce.Core.DTO;
using eCommerce.Core.Entities;
using System.Linq.Expressions;

namespace eCommerce.Core.RepositoryContracts;

public interface IProductsRepository
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product> AddProductAsync(Product product);
    Task<IEnumerable<Product?>> GetProductsByConditionAsync(Expression<Func<Product, bool>> conditionExpression);
    Task<Product?> GetProductByConditionAsync(Expression<Func<Product, bool>> conditionExpression);
    Task<Product?> UpdateProductAsync(Product product);
    Task<bool> DeleteProductAsync(Guid productId);
}
