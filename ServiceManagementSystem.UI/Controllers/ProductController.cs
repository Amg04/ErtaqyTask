using Microsoft.AspNetCore.Mvc;
using ServiceManagementSystem.Core.Entities;
using ServiceManagementSystem.Core.Interfaces;
using ServiceManagementSystem.UI.ViewModels;

namespace ServiceManagementSystem.UI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IServiceProviderRepository _serviceProviderRepository;

        public ProductController(IProductRepository productRepository,
                               IServiceProviderRepository serviceProviderRepository)
        {
            _productRepository = productRepository;
            _serviceProviderRepository = serviceProviderRepository;
        }

        public async Task<IActionResult> Index(ProductIndexViewModel model)
        {
            var filterModel = model.Filter;
            var serviceProviders = await _serviceProviderRepository.GetAllAsync();

            IEnumerable<Product> products;
            if (HasFilterCriteria(filterModel))
            {
                products = await _productRepository.GetFilteredProductsAsync(
                    filterModel.MinPrice, filterModel.MaxPrice,
                    filterModel.FromDate, filterModel.ToDate,
                    filterModel.ServiceProviderId);
            }
            else
            {
                products = await _productRepository.GetAllAsync();
            }

            var viewModel = new ProductIndexViewModel
            {
                Products = products,
                ServiceProviders = serviceProviders,
                Filter = filterModel
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            var serviceProviders = await _serviceProviderRepository.GetAllAsync();
            var model = new ProductViewModel
            {
                ServiceProviders = serviceProviders,
                CreationDate = DateTime.Now
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    ProductName = model.ProductName,
                    Price = model.Price,
                    ServiceProviderId = model.ServiceProviderId,
                    CreationDate = DateTime.Now
                };

                await _productRepository.AddAsync(product);
                return RedirectToAction(nameof(Index));
            }

            model.ServiceProviders = await _serviceProviderRepository.GetAllAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetFilteredProducts(decimal? minPrice, decimal? maxPrice,
            DateTime? fromDate, DateTime? toDate, int? serviceProviderId)
        {
            try
            {
                var products = await _productRepository.GetFilteredProductsAsync(
                    minPrice, maxPrice, fromDate, toDate, serviceProviderId);

                return PartialView("_ProductList", products);
            }
            catch (Exception ex)
            {
                return PartialView("_Error", ex.Message);
            }
        }

        private bool HasFilterCriteria(ProductFilterViewModel filter)
        {
            if (filter == null)
                return false;

            return filter.MinPrice.HasValue ||
                   filter.MaxPrice.HasValue ||
                   filter.FromDate.HasValue ||
                   filter.ToDate.HasValue ||
                   filter.ServiceProviderId.HasValue;
        }
    }
}