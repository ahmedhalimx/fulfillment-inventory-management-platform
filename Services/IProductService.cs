using FulfillmentInventoryPlatform.API.Dtos.Common;
using FulfillmentInventoryPlatform.API.Dtos.Product;

namespace FulfillmentInventoryPlatform.API.Services;

public interface IProductService
{
    Task<PagedResult<ProductResponse>> GetPagedAsync(
        int page,
        int size,
        string? sort,
        string? order,
        int? categoryId,
        string? search);

    Task<ProductResponse> GetByIdAsync(int id);
    Task<ProductResponse> CreateAsync(ProductCreateRequest request);
    Task<ProductResponse> UpdateAsync(int id, ProductUpdateRequest request);
    Task SoftDeleteAsync(int id);
}
