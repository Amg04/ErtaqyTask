using ServiceManagementSystem.Core.Entities;

namespace ServiceManagementSystem.Core.Interfaces
{
    public interface IServiceProviderRepository
    {
        Task<IEnumerable<ServiceProvider>> GetAllAsync();
        Task AddAsync(ServiceProvider serviceProvider);
    }
}
