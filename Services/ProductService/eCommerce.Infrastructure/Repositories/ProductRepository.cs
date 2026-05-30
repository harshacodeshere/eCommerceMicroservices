using eCommerce.Core.DTO;
using eCommerce.Core.Entities;
using eCommerce.Core.RepositoryContracts;
using eCommerce.Infrastructure.ProductDbContext;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace eCommerce.Infrastructure.Repositories;

public class ProductRepository : IProductsRepository
{
    private readonly ApplicationDbContext _context;
    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product> AddProductAsync(Product product)
    {
        product.ProductId = Guid.NewGuid();

        var addedProduct = await _context.Products.AddAsync(product);
        if (addedProduct == null)
            return null;
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<bool> DeleteProductAsync(Guid productId)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
        if (product == null)
            return false;
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        var products = await _context.Products.ToListAsync();
        if (products == null)
            return new List<Product>();
        return products;
    }

    public Task<Product?> GetProductByConditionAsync(Expression<Func<Product, bool>> conditionExpression)
    {
        return _context.Products.FirstOrDefaultAsync(conditionExpression);
    }

    public async Task<Product?> GetProductByIdAsync(Guid productId)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
        if (product == null)
            return null;
        return product;
    }

    public async Task<IEnumerable<Product?>> GetProductsByConditionAsync(Expression<Func<Product, bool>> conditionExpression)
    {
        return await _context.Products.Where(conditionExpression).ToListAsync();
    }

    public async Task<Product?> UpdateProductAsync(Product product)
    {
        var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == product.ProductId);
        if (existingProduct == null)
            return null;
        existingProduct.ProductName = product.ProductName;
        existingProduct.Category = product.Category;
        existingProduct.UnitPrice = product.UnitPrice;
        existingProduct.QuantityInStock = product.QuantityInStock;
        _context.Products.Update(existingProduct);
        _context.SaveChanges();
        return existingProduct;
    }

}
