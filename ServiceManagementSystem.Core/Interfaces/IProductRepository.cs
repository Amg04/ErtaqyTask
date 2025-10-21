using ServiceManagementSystem.Core.Entities;

namespace ServiceManagementSystem.Core.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task AddAsync(Product product);
        Task<IEnumerable<Product>> GetFilteredProductsAsync(decimal? minPrice, decimal? maxPrice,
            DateTime? fromDate, DateTime? toDate, int? serviceProviderId);
    }
}
