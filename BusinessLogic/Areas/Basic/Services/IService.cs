using AutoMapper;
using System.Diagnostics;
using DataAcess.Data;
using BusinessLogic.Basic.Models.Products;
using BusinessLogic.Basic.Interfaces;
using AutoMapper.QueryableExtensions;
using BusinessLogic.Basic.Models.Categories;
using Microsoft.EntityFrameworkCore;
using DataAcess.Data.Entities;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using System.Net.Http;



namespace BusinessLogic.Basic.Services
{
    public class ProductService : IProductService
    {
        private readonly HulkDbContext _context;
        private readonly IMapper _mapper;

        public ProductService(HulkDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ProductItemViewModel> GetInfoViewModelAsync(int id)
        {

            var productViewModel = await _context.Products
              .Where(p => p.Id == id)
              .ProjectTo<ProductItemViewModel>(_mapper.ConfigurationProvider)
              .SingleOrDefaultAsync();

            return productViewModel!;
        }
        public ProductHomeViewModel GetProducts(ProductSearchViewModel search, string sortBy)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(search.Name))
                query = query.Where(x => x.Name.ToLower().Contains(search.Name.ToLower()));

            if (search.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == search.CategoryId.Value);

            switch (sortBy)
            {
                case "Name":
                    query = query.OrderBy(p => p.Name);
                    break;
                case "PriceAsc":
                    query = query.OrderBy(p => p.Price);
                    break;
                case "PriceDesc":
                    query = query.OrderByDescending(p => p.Price);
                    break;
                default:
                    query = query.OrderBy(p => p.Name);
                    break;
            }

            int count = query.Count();
            int page = search.Page ?? 1;
            int pageSize = search.PageSize;

            var products = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize).ProjectTo<ProductItemViewModel>(_mapper.ConfigurationProvider)
                .ToList() ?? throw new Exception("Failed to get products");

            var categories = _context.Categories
                .ProjectTo<CategoryItemViewModel>(_mapper.ConfigurationProvider)
                .ToList() ?? throw new Exception("Failed to get categories");

            search.Categories = categories;

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            Console.WriteLine("RunTime ProductService GetProducts " + elapsedTime);

            return new ProductHomeViewModel
            {
                Search = search,
                Products = products,
                Count = count,
                Pagination = new PaginationViewModel
                {
                    PageSize = pageSize,
                    TotalItems = count,
                    CurrentPage = page,
                },
                Categories = categories,
            };
        }

        public async Task<List<ProductItemViewModel>> GetProductsAsyncIds(List<int> ids)
        {

            return await _context.Products
          .Where(p => ids.Contains(p.Id))
          .ProjectTo<ProductItemViewModel>(_mapper.ConfigurationProvider)
          .ToListAsync();

        }
    }
    public class CategoryService : ICategoryService
    {
        private readonly HulkDbContext _context;
        private readonly IMapper _mapper;

        public CategoryService(HulkDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IEnumerable<CategoryItemViewModel>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ProjectTo<CategoryItemViewModel>(_mapper.ConfigurationProvider).ToListAsync();
        }
    }

    public class CartService : ICartService
    {
        private readonly HulkDbContext _hulkDbContext;
        private readonly HttpContext? _httpContext;
        private readonly IMapper _mapper;
        private readonly IProductService service;

        public CartService(HulkDbContext context,IHttpContextAccessor httpContextAccessor, IMapper mapper, IProductService service)
        {
            this._hulkDbContext = context;
            this._httpContext = httpContextAccessor.HttpContext;
            this._mapper = mapper;
            this.service = service;
        }
        public void Add(int productId)
        {
            var productIds = _httpContext.Session.GetObject<List<int>>("cart");

            if (productIds == null)  productIds = new List<int>(); 
            
            productIds.Add(productId);
            _httpContext.Session.SetObject("cart", productIds);
        }

        public async Task<List<ProductItemViewModel>> GetProducts()
        {
            var productIds = _httpContext.Session.GetObject<List<int>>("cart");
            List<ProductItemViewModel> products = new List<ProductItemViewModel>();

            if (productIds != null) products =  await service.GetProductsAsyncIds(productIds);
            
            return products;
        }

        public bool IsInCart(int productId)
        {
            var productIds = _httpContext.Session.GetObject<List<int>>("cart");
            if (productIds == null) { 
                return false; 
            }
            return productIds.Contains(productId);
        }


        public void Remove(int productId)
        {
            var productIds = _httpContext.Session.GetObject<List<int>>("cart");
            if (productIds == null)  productIds = new List<int>(); 
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