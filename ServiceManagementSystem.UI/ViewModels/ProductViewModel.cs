using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using ServiceManagementSystem.Core.Entities;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ServiceManagementSystem.UI.ViewModels
{
    public class ProductViewModel
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Creation date is required")]
        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; }

        [Required(ErrorMessage = "Service provider is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid service provider")]
        [Display(Name = "Service Provider")]
        public int ServiceProviderId { get; set; }
        [ValidateNever]
        public IEnumerable<Core.Entities.ServiceProvider> ServiceProviders { get; set; }
    }
}
