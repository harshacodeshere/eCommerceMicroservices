using AutoMapper;
using eCommerce.Core.DTO;
using eCommerce.Core.Entities;
using eCommerce.Core.RepositoryContracts;
using eCommerce.Core.ServiceContracts;
using FluentValidation;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace eCommerce.Core.Services;

public class ProductService : IProductService
{
    private readonly IValidator<ProductAddRequest> _productAddRequestValidator;
    private readonly IValidator<ProductUpdateRequest> _productUpdateRequestValidator;
    private readonly IMapper _mapper;
    private readonly IProductsRepository _productsRepository;

    public ProductService(IValidator<ProductAddRequest> productAddRequestValidator, 
        IValidator<ProductUpdateRequest> productUpdateRequestValidator, IMapper mapper, IProductsRepository productsRepository)
    {
        _productAddRequestValidator = productAddRequestValidator;
        _productUpdateRequestValidator = productUpdateRequestValidator;
        _mapper = mapper;
        _productsRepository = productsRepository;
    }
    public async Task<ProductResponse> AddProductAsync(ProductAddRequest? productAddRequest)
    {
        if (productAddRequest == null)
        {
            throw new ArgumentNullException(nameof(productAddRequest));
        }
        var validationResult = await _productAddRequestValidator.ValidateAsync(productAddRequest);

        if (!validationResult.IsValid)
        {
            string errors = string.Join(", ", validationResult.Errors.Select(err => err.ErrorMessage));
            throw new ArgumentException(errors);
        }

        Product product = _mapper.Map<Product>(productAddRequest);
        Product addedProduct = await _productsRepository.AddProductAsync(product);
        ProductResponse addedProductResponse = _mapper.Map<ProductResponse>(addedProduct);
        
        return addedProductResponse;
    }

    public async Task<bool> DeleteProductAsync(Guid productId)
    {
        if(productId == Guid.Empty)
        {
            throw new ArgumentException("ProductId cannot be empty", nameof(productId));
        }

        bool isProductDeleted =  await _productsRepository.DeleteProductAsync(productId);
        if (!isProductDeleted)
        {
            throw new ArgumentException($"Product with id {productId} does not exist");
        }
        else
        {
            return isProductDeleted;
        }
    }

    public async Task<List<ProductResponse>> GetAllProductsAsync()
    {
        var products = await _productsRepository.GetAllProductsAsync();
        return _mapper.Map<List<ProductResponse>>(products);
    }

    public async Task<ProductResponse?> UpdateProductAsync(ProductUpdateRequest? productUpdateRequest)
    {
        if (productUpdateRequest == null)
        {
            throw new ArgumentNullException(nameof(productUpdateRequest));
        }
        var validationResult =  _productUpdateRequestValidator.Validate(productUpdateRequest);
        if (!validationResult.IsValid)
        {
            string errors = string.Join(", ", validationResult.Errors.Select(err => err.ErrorMessage));
            throw new ArgumentException(errors);
        }
        var product = await _productsRepository.UpdateProductAsync(_mapper.Map<Product>(productUpdateRequest));
        var updatedProductResponse = _mapper.Map<ProductResponse>(product);
        
        return updatedProductResponse;
    }

    public async Task<ProductResponse?> GetProductByConditionAsync(Expression<Func<Product, bool>> conditionExpression)
    {
        var product = await _productsRepository.GetProductByConditionAsync(conditionExpression);
        if (product == null)
        {
            return null;
        }
        return _mapper.Map<ProductResponse>(product);
    }

    public async Task<List<ProductResponse?>> GetProductsByConditionAsync(Expression<Func<Product, bool>> conditionExpression)
    {
        var product = await _productsRepository.GetProductsByConditionAsync(conditionExpression);
        if (product == null)
        {
            return null;
        }
        return _mapper.Map<List<ProductResponse?>>(product);
    }
}
