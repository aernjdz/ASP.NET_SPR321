using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using DataAcess.Data;
using BusinessLogic.Basic.Models.Products;
using AutoMapper.QueryableExtensions;
using System.Diagnostics;
using BusinessLogic.Basic.Models.Categories;
using BusinessLogic.Basic.Interfaces;
namespace WebHulk.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        public IActionResult Index(ProductSearchViewModel search, string sortBy)
        {
            var model = _productService.GetProducts(search, sortBy);
            return View(model);
        }
    }
}


