using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
//
using BusinessLogic.Admin.Models.Products;
using BusinessLogic.Admin.Models.Category;

using DataAcess.Data.Entities;

using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using BusinessLogic.Admin.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Security.Claims;
using DataAcess.Interfaces;
using DataAcess.Data;

namespace BusinessLogic.Admin.Services
{
    public class ProductServiceAdmin : IProductServiceAdmin
    {
        private readonly Cloudinary cloudinary;
    
        private readonly IMapper _mapper;
        private readonly IImageWorker _worker;
        private readonly IRepository<Product> _Productrepository;
        private readonly IRepository<CategoryEntity> _categoryRepository;
        private readonly IRepository<ProductImage> _ProductImageRepository;
        public ProductServiceAdmin(HulkDbContext context,IMapper mapper, Cloudinary _cloudinary, IImageWorker worker, IRepository<Product> productrepo, IRepository<CategoryEntity> categoryrepo, IRepository<ProductImage> ProductImagerepo)
        {
         
            cloudinary = _cloudinary;
            _Productrepository = productrepo;
            _categoryRepository = categoryrepo;
            _ProductImageRepository = ProductImagerepo;
            _mapper = mapper;
            _worker = worker;
        }

        public async Task<List<ProductItemViewModel>> GetAllProductsAsync()
        {
            var products = _Productrepository.Get(includeProperties: new[] {"Category","ProductImages"});
            return _mapper.Map<List<ProductItemViewModel>>(products);
        }

        public async Task<ProductCreateViewModel> GetCreateViewModelAsync()
        {

            var categories = _categoryRepository.Get()
              .Select(x => new { Value = x.Id, Text = x.Name }).ToList();

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
            _Productrepository.Insert(product);
            _Productrepository.Save();

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
                       _ProductImageRepository.Insert(imgEntity);
                    }
                }

                     _ProductImageRepository.Save();
                }
            }

        

        public async Task<ProductEditViewModel> GetEditViewModelAsync(int id)
        {
            var model = _Productrepository.Get(p => p.Id == id, includeProperties: new[] { "Category", "ProductImages" }).FirstOrDefault();

            if (model == null)
                throw new Exception("Product not found");

            var categories = _categoryRepository.Get()
               .Select(x => new { Value = x.Id, Text = x.Name })
               .ToList();

            var editViewModel = _mapper.Map<ProductEditViewModel>(model);
            editViewModel.CategoryList = new SelectList(categories, "Value", "Text");

            return editViewModel;
        }

        public async Task EditProductAsync(ProductEditViewModel model)
        {
            var product = _Productrepository.GetByID(model.Id ,includeProperties: new[] { "Category", "ProductImages" });

            if (product == null)
                throw new Exception("No product was found");

            _mapper.Map(model, product);

            if (model.NewImages != null)
            {
                foreach (var img in model.NewImages)
                {
                    if (img.Length > 0)
                    {
       
                        var res = await _worker.ImageSave(img);

                        if (res.StatusCode == System.Net.HttpStatusCode.OK)
                        {

                            var imgEntity = new ProductImage
                            {
                                Image = res.SecureUri.AbsoluteUri,
                                public_id = res.PublicId,
                                Product = product
                            };
                           _ProductImageRepository.Insert(imgEntity);
                           _ProductImageRepository.Save();
                        }
                        
                    }
                }
            }

            if (model.DeletedPhotoIds != null)
            {
                var photos = product.ProductImages.Where(pi => model.DeletedPhotoIds.Contains(pi.Id)).ToList();




                foreach (var photo in photos)
                {
                    await _worker.DeleteImageAsync(photo.public_id);
                    _ProductImageRepository.Delete(photo.Id);
                }
              
                _ProductImageRepository.Save();
            }

            _Productrepository.Save();
        }

        public async Task DeleteProductAsync(int id)
        {
           var product = _Productrepository.GetByID(id, includeProperties: new[] { "Category", "ProductImages" });

            if (product == null)
                throw new Exception("Product not found");

            foreach (var img in product.ProductImages)
            {
                await _worker.DeleteImageAsync(img.public_id);
                _ProductImageRepository.Delete(img.Id);
            }

            
           _Productrepository.Delete(product);
            _Productrepository.Save();
            _ProductImageRepository.Save();
        }
    }
    public class CategoryServiceAdmin : ICategoryServiceAdmin
    {
        private readonly IRepository<CategoryEntity> _CategoryRepo;
        private readonly Cloudinary cloudinary;
  
        private readonly IMapper _mapper;
        private readonly IImageWorker _worker;
       
        public CategoryServiceAdmin(HulkDbContext context, IMapper mapper, IImageWorker worker, Cloudinary _cloudinary, IRepository<CategoryEntity> categoryRepo)
        {
            cloudinary = _cloudinary;
            _worker = worker;

            _mapper = mapper;
            _CategoryRepo = categoryRepo;
        }

        public async Task<IEnumerable<CategoryItemViewModel>> GetAllCategoriesAsync()
        {
            var categories = _CategoryRepo.Get().ToList();
            return _mapper.Map<List<CategoryItemViewModel>>(categories); 
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
                _CategoryRepo.Insert(category);

                _CategoryRepo.Save();
            }
          
            
        }

        public async Task<CategoryEditViewModel> GetEditViewModelAsync(int id)
        {
            var category = _CategoryRepo.GetByID(id);

            if (category == null)
                throw new KeyNotFoundException($"Category with id={id} not found.");

            return _mapper.Map<CategoryEditViewModel>(category);
        }

        public async Task EditCategoryAsync(CategoryEditViewModel model)
        {
            var category =_CategoryRepo.GetByID(model.Id);
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
                    _CategoryRepo.Update(category);
                }
                
            }
            _CategoryRepo.Save();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = _CategoryRepo.GetByID(id);
            if (category == null)
                throw new KeyNotFoundException("Category not found.");

           await  _worker.DeleteImageAsync(category.public_id);   

            _CategoryRepo.Delete(category);
            _CategoryRepo.Save();
        }
    }
}