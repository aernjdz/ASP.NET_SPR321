using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
//
using BusinessLogic.Admin.Models.Products;
using BusinessLogic.Admin.Models.Category;
//
using DataAcess.Data.Entities;
using DataAcess.Data;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using BusinessLogic.Admin.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Security.Claims;
using DataAcess.Interfaces;

namespace BusinessLogic.Admin.Services
{
    public class ProductServiceAdmin : IProductServiceAdmin
    {
        private readonly Cloudinary cloudinary;
        private readonly HulkDbContext _context;
        private readonly IMapper _mapper;
        private readonly IImageWorker _worker;
        public ProductServiceAdmin(HulkDbContext context, IMapper mapper, Cloudinary _cloudinary, IImageWorker worker)
        {
            cloudinary = _cloudinary;
            _context = context;
            _mapper = mapper;
            _worker = worker;
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
                    

                    var uploadResult = await _worker.ImageSave(img);
                    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Console.WriteLine(uploadResult.SecureUrl.AbsoluteUri);
                        var imgEntity = new ProductImage
                        {
                            Image = uploadResult.SecureUrl.AbsoluteUri,
                            Priotity = i++,
                            public_id =uploadResult.PublicId,
                            Product = product,
                        };
                        _context.ProductImages.Add(imgEntity);
                    }
                }

                    await _context.SaveChangesAsync();
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

                        var res = await _worker.ImageSave(img);

                        if (res.StatusCode == System.Net.HttpStatusCode.OK)
                        {

                            var imgEntity = new ProductImage
                            {
                                Image = res.SecureUri.AbsoluteUri,
                                public_id = res.PublicId,
                                Product = product
                            };
                            _context.ProductImages.Add(imgEntity);
                        }
                        
                    }
                }
            }

            if (model.DeletedPhotoIds != null)
            {
                var photos = _context.ProductImages
                    .Where(pi => model.DeletedPhotoIds.Contains(pi.Id))
                    .ToList();

               

                foreach (var photo in photos)
                {
                    await _worker.DeleteImageAsync(photo.public_id);
                }
                _context.ProductImages.RemoveRange(photos);

                await _context.SaveChangesAsync();
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
               

                await _worker.DeleteImageAsync(img.public_id);
            }

            _context.ProductImages.RemoveRange(product.ProductImages);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
    public class CategoryServiceAdmin : ICategoryServiceAdmin
    {
        private readonly Cloudinary cloudinary;
        private readonly HulkDbContext _context;
        private readonly IMapper _mapper;
        private readonly IImageWorker _worker;
       
        public CategoryServiceAdmin(HulkDbContext context, IMapper mapper, IImageWorker worker, Cloudinary _cloudinary)
        {
            cloudinary = _cloudinary;
            _worker= worker;
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
           
           
            var res = await _worker.ImageSave(createModel.Image);
            Console.WriteLine(res.SecureUrl.AbsoluteUri);
            if ( res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var category = new CategoryEntity
                {
                    Name = createModel.Name,
                    Image = res.SecureUri.AbsoluteUri,
                    public_id = res.PublicId
                };
                _context.Categories.Add(category);

                await _context.SaveChangesAsync();
            }
          
            
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


                await _worker.DeleteImageAsync(category.public_id);
                var res = await _worker.ImageSave(model.NewImage);

                if (res.StatusCode == System.Net.HttpStatusCode.OK) {
                    category.Image = res.SecureUri.AbsoluteUri;
                    category.public_id = res.PublicId;
                    _context.Categories.Update(category);
                }
                
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                throw new KeyNotFoundException("Category not found.");

           await  _worker.DeleteImageAsync(category.public_id);   

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}