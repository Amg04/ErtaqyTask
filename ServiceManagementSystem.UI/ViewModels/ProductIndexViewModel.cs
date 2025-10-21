using ServiceManagementSystem.Core.Entities;

namespace ServiceManagementSystem.UI.ViewModels
{
    public class ProductIndexViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Core.Entities.ServiceProvider> ServiceProviders { get; set; }
        public ProductFilterViewModel Filter { get; set; }
    }
}
