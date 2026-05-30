using eCommerce.Core.DTO;
using eCommerce.Core.Entities;
using System.Linq.Expressions;

namespace eCommerce.Core.ServiceContracts;

public interface IProductService
{
    Task<ProductResponse> AddProductAsync(ProductAddRequest? productAddRequest);
    Task<ProductResponse?> GetProductByConditionAsync(Expression<Func<Product, bool>> conditionExpression);
    Task<List<ProductResponse?>> GetProductsByConditionAsync(Expression<Func<Product, bool>> conditionExpression);

    Task<List<ProductResponse>> GetAllProductsAsync();
    Task<ProductResponse?> UpdateProductAsync(ProductUpdateRequest? productUpdateRequest);
    Task<bool> DeleteProductAsync(Guid productId);
}
