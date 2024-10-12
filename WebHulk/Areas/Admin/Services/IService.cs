using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
//
using WebHulk.Areas.Admin.Models.Products;
using WebHulk.Areas.Admin.Models.Category;
//
using DataAcess.Data.Entities;
using DataAcess.Data;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using WebHulk.Areas.Admin.Interfaces;

namespace WebHulk.Areas.Admin.Services
{
    public class ProductServiceAdmin : IProductServiceAdmin
    {
        private readonly HulkDbContext _context;
        private readonly IMapper _mapper;

        public ProductServiceAdmin(HulkDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ProductItemViewModel>> GetAllProductsAsync()
        {
            return await _context.Products.ProjectTo<ProductItemViewModel>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<ProductCreateViewModel> GetCreateViewModelAsync()
        {
            var categories = await _context.Categories.Select(x => new { Value = x.Id, Text = x.Name }).ToListAsync();

            return new ProductCreateViewModel
            {
                CategoryList = new SelectList(categories, "Value", "Text")
            };
        }

        public async Task CreateProductAsync(ProductCreateViewModel model)
        {
            var product = new Product
            {
                Name = model.Name,
                Price = model.Price,
                CategoryId = model.CategoryId,
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            if (model.Photos != null)
            {
                int i = 0;
                foreach (var img in model.Photos)
                {
                    string ext = Path.GetExtension(img.FileName);
                    string fName = Guid.NewGuid().ToString() + ext;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "images", fName);

                    using (var fs = new FileStream(path, FileMode.Create))
                        await img.CopyToAsync(fs);

                    var imgEntity = new ProductImage
                    {
                        Image = fName,
                        Priotity = i++,
                        Product = product,
                    };
                    _context.ProductImages.Add(imgEntity);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task<ProductEditViewModel> GetEditViewModelAsync(int id)
        {
            var model = await _context.Products
                .ProjectTo<ProductEditViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (model == null)
                throw new Exception("Product not found");

            var categories = await _context.Categories
                .Select(x => new { Value = x.Id, Text = x.Name })
                .ToListAsync();

            model.CategoryList = new SelectList(categories, "Value", "Text");

            return model;
        }

        public async Task EditProductAsync(ProductEditViewModel model)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == model.Id);

            if (product == null)
                throw new Exception("No product was found");

            _mapper.Map(model, product);

            if (model.NewImages != null)
            {
                foreach (var img in model.NewImages)
                {
                    if (img.Length > 0)
                    {
                        string ext = Path.GetExtension(img.FileName);
                        string fName = Guid.NewGuid().ToString() + ext;
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "images", fName);

                        using (var fs = new FileStream(path, FileMode.Create))
                            await img.CopyToAsync(fs);

                        var imgEntity = new ProductImage
                        {
                            Image = fName,
                            Product = product
                        };
                        _context.ProductImages.Add(imgEntity);
                    }
                }
            }

            if (model.DeletedPhotoIds != null)
            {
                var photos = _context.ProductImages
                    .Where(pi => model.DeletedPhotoIds.Contains(pi.Id))
                    .ToList();

                _context.ProductImages.RemoveRange(photos);

                foreach (var photo in photos)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "images", photo.Image);
                    if (File.Exists(path)) File.Delete(path);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .SingleOrDefaultAsync(p => p.Id == id);

            if (product == null)
                throw new Exception("Product not found");

            foreach (var img in product.ProductImages)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "images", img.Image);
                if (File.Exists(path)) File.Delete(path);
            }

            _context.ProductImages.RemoveRange(product.ProductImages);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
    public class CategoryServiceAdmin : ICategoryServiceAdmin
    {
        private readonly HulkDbContext _context;
        private readonly IMapper _mapper;

        public CategoryServiceAdmin(HulkDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryItemViewModel>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .ProjectTo<CategoryItemViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

      

        public async Task CreateCategoryAsync(CategoryCreateViewModel createModel)
        {
            string ext = Path.GetExtension(createModel.Image.FileName);
            string fName = Guid.NewGuid().ToString() + ext;

            var path = Path.Combine(Directory.GetCurrentDirectory(), "images", fName);

            using (var stream = new FileStream(path, FileMode.Create))
                await createModel.Image.CopyToAsync(stream);

            var category = new CategoryEntity
            {
                Name = createModel.Name,
                Image = fName
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task<CategoryEditViewModel> GetEditViewModelAsync(int id)
        {
            var category = await _context.Categories
                .ProjectTo<CategoryEditViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                throw new KeyNotFoundException($"Category with id={id} not found.");

            return category;
        }

        public async Task EditCategoryAsync(CategoryEditViewModel model)
        {
            var category = await _context.Categories.FindAsync(model.Id);
            if (category == null)
                throw new KeyNotFoundException("Category not found.");

            category.Name = model.Name;

            if (model.NewImage != null)
            {
                var currentImgPath = Path.Combine(Directory.GetCurrentDirectory(), "images", category.Image);
                if (File.Exists(currentImgPath))
                    File.Delete(currentImgPath);

                var newImgName = Guid.NewGuid().ToString() + Path.GetExtension(model.NewImage.FileName);
                var newImgPath = Path.Combine(Directory.GetCurrentDirectory(), "images", newImgName);

                using (var stream = new FileStream(newImgPath, FileMode.Create))
                    await model.NewImage.CopyToAsync(stream);

                category.Image = newImgName;
            }

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                throw new KeyNotFoundException("Category not found.");

            var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "images", category.Image);
            if (File.Exists(imgPath))
                File.Delete(imgPath);

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}
