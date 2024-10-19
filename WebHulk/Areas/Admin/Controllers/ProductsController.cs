
using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Admin.Models.Products;
using Microsoft.AspNetCore.Authorization;
using DataAcess.Constants;
using BusinessLogic.Admin.Interfaces;

namespace WebHulk.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Admin)]
    public class ProductsController : Controller
    {
        private readonly IProductServiceAdmin _productService;

        public ProductsController(IProductServiceAdmin productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _productService.GetAllProductsAsync();
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = await _productService.GetCreateViewModelAsync();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _productService.CreateProductAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var viewModel = await _productService.GetEditViewModelAsync(id);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _productService.EditProductAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}