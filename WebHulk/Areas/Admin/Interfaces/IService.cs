using WebHulk.Areas.Admin.Models.Category;
using WebHulk.Areas.Admin.Models.Products;
using DataAcess.Data.Entities;


namespace WebHulk.Areas.Admin.Interfaces
{
    public interface IProductServiceAdmin
    {
        Task<List<ProductItemViewModel>> GetAllProductsAsync();
        Task<ProductCreateViewModel> GetCreateViewModelAsync();
        Task CreateProductAsync(ProductCreateViewModel model);
        Task<ProductEditViewModel> GetEditViewModelAsync(int id);
        Task EditProductAsync(ProductEditViewModel model);
        Task DeleteProductAsync(int id);

        /*        Task<WebHulk.Models.Products.ProductHomeViewModel> GetProducts(WebHulk.Models.Products.ProductSearchViewModel search, string sortBy);*/

    }
    public interface ICategoryServiceAdmin
    {
        Task<IEnumerable<CategoryItemViewModel>> GetAllCategoriesAsync();
        Task CreateCategoryAsync(CategoryCreateViewModel createModel);
        Task<CategoryEditViewModel> GetEditViewModelAsync(int id);
        Task EditCategoryAsync(CategoryEditViewModel model);
        Task DeleteCategoryAsync(int id);
    }
}
