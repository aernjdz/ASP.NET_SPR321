using Microsoft.AspNetCore.Mvc;
using WebHulk.Models.Categories;
using WebHulk.Models.Products;

namespace WebHulk.Interfaces
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