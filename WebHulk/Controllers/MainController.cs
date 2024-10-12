using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using DataAcess.Data;
using WebHulk.Interfaces;
using WebHulk.Models.Categories;

namespace WebHulk.Controllers
{
    public class MainController : Controller
    {
        private readonly ICategoryService _service;
      

        public MainController(ICategoryService service)
        {
           _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var categories  = await _service.GetAllCategoriesAsync();

            if (categories == null) Console.WriteLine("Categories is null..");
            return View(categories);

        }
    }
}
