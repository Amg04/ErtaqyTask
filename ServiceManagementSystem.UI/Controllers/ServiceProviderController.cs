using Microsoft.AspNetCore.Mvc;
using ServiceManagementSystem.Core.Interfaces;
using ServiceManagementSystem.UI.ViewModels;

namespace ServiceManagementSystem.UI.Controllers
{
    public class ServiceProviderController : Controller
    {
        private readonly IServiceProviderRepository _serviceProviderRepository;

        public ServiceProviderController(IServiceProviderRepository serviceProviderRepository)
        {
            _serviceProviderRepository = serviceProviderRepository;
        }

        public async Task<IActionResult> Index()
        {
            var serviceProviders = await _serviceProviderRepository.GetAllAsync();
            return View(serviceProviders);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceProviderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var serviceProvider = new Core.Entities.ServiceProvider
                {
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    Address = model.Address,
                    CreatedDate = DateTime.Now
                };

                await _serviceProviderRepository.AddAsync(serviceProvider);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}
