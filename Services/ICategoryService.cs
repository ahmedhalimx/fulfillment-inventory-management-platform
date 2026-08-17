using FulfillmentInventoryPlatform.API.Dtos.Category;

namespace FulfillmentInventoryPlatform.API.Services;

public interface ICategoryService
{
    Task<List<CategoryResponse>> GetAllAsync();
    Task<CategoryResponse> GetByIdAsync(int id);
    Task<CategoryResponse> CreateAsync(CategoryCreateRequest request);
    Task<CategoryResponse> UpdateAsync(int id, CategoryUpdateRequest request);
    Task SoftDeleteAsync(int id);
}
