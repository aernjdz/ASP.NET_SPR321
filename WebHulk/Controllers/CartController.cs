using BusinessLogic.Basic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebHulk.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService cartService;

        public CartController(ICartService service)
        {
            this.cartService = service;
        }

      
        public async Task<IActionResult> Index()
        {
            return View(cartService.GetProducts());
        }

      
        public IActionResult Add(int productId, string returnUrl)
        {
            cartService.Add(productId);
            return Redirect(returnUrl);
        }

        [HttpPost]
        public IActionResult Remove(int productId, string returnUrl)
        {
            cartService.Remove(productId);
            return Redirect(returnUrl);
        }

        [HttpGet("getCartCount")]
        public IActionResult GetCartCount()
        {
            var cartCount = cartService.GetCartCount();  // Get the count from service (e.g., from session)
            return Json(cartCount);
        }
    }
}
