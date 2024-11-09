using AutoMapper;
using System.Diagnostics;
using DataAcess.Data;
using BusinessLogic.Basic.Models.Products;
using BusinessLogic.Basic.Interfaces;

using BusinessLogic.Basic.Models.Categories;

using DataAcess.Data.Entities;

using Microsoft.AspNetCore.Http;

using DataAcess.Interfaces;



namespace BusinessLogic.Basic.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<CategoryEntity> _categoryRepository;
        private readonly IMapper _mapper;

        public ProductService(IRepository<Product> productRepository, IRepository<CategoryEntity> categoryRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }



        public async Task<ProductItemViewModel> GetInfoViewModelAsync(int id)
        {
            var product = _productRepository.GetByID(id, includeProperties: new[] { "Category", "ProductImages" });
            return _mapper.Map<ProductItemViewModel>(product);

        }

        public ProductHomeViewModel GetProducts(ProductSearchViewModel search, string sortBy)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var query = _productRepository.Get(x =>
                (string.IsNullOrEmpty(search.Name) || x.Name.ToLower().Contains(search.Name.ToLower())) &&
                (!search.CategoryId.HasValue || x.CategoryId == search.CategoryId.Value),
                includeProperties: new[] { "Category", "ProductImages" }
            );

            // Sorting logic
            query = sortBy switch
            {
                "Name" => query.OrderBy(p => p.Name),
                "PriceAsc" => query.OrderBy(p => p.Price),
                "PriceDesc" => query.OrderByDescending(p => p.Price),
                _ => query.OrderBy(p => p.Name),
            };

            int count = query.Count();
            int page = search.Page ?? 1;
            int pageSize = search.PageSize;

            var products = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var categories = _categoryRepository.Get().ToList();

            search.Categories = _mapper.Map<List<CategoryItemViewModel>>(categories);

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("RunTime ProductService GetProducts " + elapsedTime);

            return new ProductHomeViewModel
            {
                Search = search,
                Products = _mapper.Map<List<ProductItemViewModel>>(products),
                Count = count,
                Pagination = new PaginationViewModel
                {
                    PageSize = pageSize,
                    TotalItems = count,
                    CurrentPage = page,
                },
                Categories = _mapper.Map<List<CategoryItemViewModel>>(categories),
            };
        }

        public List<ProductItemViewModel> GetProductsIds(List<int> ids)
        {
            var products = _productRepository.Get(p => ids.Contains(p.Id),includeProperties: new[] { "Category", "ProductImages" }).ToList();
            return _mapper.Map<List<ProductItemViewModel>>(products);
        }
    }
    public class CategoryService : ICategoryService
    {

        private readonly IMapper _mapper;
        private readonly IRepository<CategoryEntity> _categoryRepository;
        public CategoryService(IRepository<CategoryEntity> categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryItemViewModel>> GetAllCategoriesAsync()
        {
            var categories = _categoryRepository.Get();
            return _mapper.Map<List<CategoryItemViewModel>>(categories);
        }
    }

 public class CartService : ICartService
    {
        private readonly HulkDbContext _hulkDbContext;
        private readonly HttpContext? _httpContext;
        private readonly IMapper _mapper;
        private readonly IProductService service;

        public CartService(HulkDbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper, IProductService service)
        {
            this._hulkDbContext = context;
            this._httpContext = httpContextAccessor.HttpContext;
            this._mapper = mapper;
            this.service = service;
        }
        public void Add(int productId)
        {
            var productIds = _httpContext.Session.GetObject<List<int>>("cart");

            if (productIds == null) productIds = new List<int>();

            productIds.Add(productId);
            _httpContext.Session.SetObject("cart", productIds);
        }

        public List<ProductItemViewModel> GetProducts()
        {
            var productIds = _httpContext.Session.GetObject<List<int>>("cart");
            List<ProductItemViewModel> products = new List<ProductItemViewModel>();

            if (productIds != null) products = service.GetProductsIds(productIds);

            return products;
        }

        public bool IsInCart(int productId)
        {
            var productIds = _httpContext.Session.GetObject<List<int>>("cart");
            if (productIds == null)
            {
                return false;
            }
            return productIds.Contains(productId);
        }


        public void Remove(int productId)
        {
            var productIds = _httpContext.Session.GetObject<List<int>>("cart");
            if (productIds == null) productIds = new List<int>();
            productIds.Remove(productId);
            _httpContext.Session.SetObject("cart", productIds);
        }

        public int GetCartCount()
        {
            var cart = _httpContext.Session.GetObject<List<int>>("cart");
            return cart?.Count ?? 0;
        }
    }
}