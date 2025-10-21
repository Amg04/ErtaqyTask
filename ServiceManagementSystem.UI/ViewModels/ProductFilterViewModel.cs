using ServiceManagementSystem.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace ServiceManagementSystem.UI.ViewModels
{
    public class ProductFilterViewModel
    {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        public int? ServiceProviderId { get; set; }

        public IEnumerable<Core.Entities.ServiceProvider> ServiceProviders { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
