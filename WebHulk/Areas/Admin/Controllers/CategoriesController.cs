using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebHulk.Areas.Admin.Interfaces;
using WebHulk.Areas.Admin.Models.Category;
using DataAcess.Constants;


namespace WebHulk.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Admin)]
    public class CategoriesController : Controller
    {
        private readonly ICategoryServiceAdmin _categoryService;


        public CategoriesController(ICategoryServiceAdmin categoryService)
        {
            _categoryService = categoryService;
        }
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();

            if (categories == null) Console.WriteLine("Categories is null..");
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CategoryCreateViewModel createModel)
        {
             await _categoryService.CreateCategoryAsync(createModel);
             return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await  _categoryService.GetEditViewModelAsync(id);
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryEditViewModel model)
        {
            await _categoryService.EditCategoryAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
