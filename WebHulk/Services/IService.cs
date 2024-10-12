using AutoMapper;
using System.Diagnostics;
using DataAcess.Data;
using WebHulk.Models.Products;
using WebHulk.Interfaces;
using AutoMapper.QueryableExtensions;
using WebHulk.Models.Categories;
using Microsoft.EntityFrameworkCore;

namespace WebHulk.Services
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
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
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
        public async  Task<IEnumerable<CategoryItemViewModel>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ProjectTo<CategoryItemViewModel>(_mapper.ConfigurationProvider).ToListAsync();
        }
    }
}