
using BusinessLogic.Basic.Models.Categories;
using BusinessLogic.Basic.Models.Products;

namespace BusinessLogic.Basic.Interfaces
{
    public interface IProductService
    {
        ProductHomeViewModel GetProducts(ProductSearchViewModel search, string sortBy);
    }
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryItemViewModel>> GetAllCategoriesAsync();
    }
}