using eCommerce.Core.DTO;
using eCommerce.Core.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.API.APIEndpoints;

public static class ProductApiEndpoints
{
    public static IEndpointRouteBuilder MapProductAPIEndpoints(this IEndpointRouteBuilder app)
    {
        //GET /api/products
        app.MapGet("/api/products", async (IProductService productService) =>
        {
            var products = await productService.GetAllProductsAsync();
            return Results.Ok(products);
        });

        // /api/products/search/product-id
        app.MapGet("/api/products/search/product-id/{ProductID}", async (IProductService productService, Guid ProductID) =>
        {
            ProductResponse? products = await productService.GetProductByConditionAsync(temp =>

                temp.ProductId == ProductID
            );
            return Results.Ok(products);
        });

        // /api/products/search/searchString
        app.MapGet("/api/products/search/{searchString}", async (IProductService productService, string searchString) =>
        {
            List<ProductResponse?> products = await productService.GetProductsByConditionAsync(temp =>

                temp.ProductName.StartsWith(searchString) || temp.Category.StartsWith(searchString)
            );
            if (products != null)
            {
                return Results.Ok(products);
            }
            return Results.NotFound();
        });

        // /api/products
        app.MapPost("/api/products", async(IProductService productService, ProductAddRequest productAddRequest) =>
        {
            var product = await productService.AddProductAsync(productAddRequest);
            if (product != null)
            {
                return Results.Created($"/api/products/search/product-id/{product.ProductId}", product);
            }
            return Results.BadRequest("Product could not be added");
        });

        // /api/products
        app.MapPut("/api/products", async (IProductService productService, [FromBody] ProductUpdateRequest productUpdateRequest) =>
        {
            var products = await productService.UpdateProductAsync(productUpdateRequest);
            if (products != null)
            {
                return Results.Ok(products);
            }
            return Results.BadRequest("Product could not be updated");
        });

        // /api/products/productId
        app.MapDelete("/api/products/{productId}", async (IProductService productService, Guid productId) =>
        {
            bool isProductDeleted = await productService.DeleteProductAsync(productId);
            if (isProductDeleted)
            {
                return Results.Ok($"Product with id {productId} deleted successfully");
            }
            return Results.Problem($"Product with id {productId} could not be deleted");
        });


        return app;
    }

}
