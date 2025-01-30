

using BusinessLogic.Basic.Models.Categories;
using BusinessLogic.Basic.Models.Products;
using DataAcess.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Basic.Interfaces
{
    public interface IProductService
    {
        ProductHomeViewModel GetProducts(ProductSearchViewModel search, string sortBy);

        Task<ProductItemViewModel> GetInfoViewModelAsync(int id);

        List<ProductItemViewModel> GetProductsIds(List<int> ids);
       
    }
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryItemViewModel>> GetAllCategoriesAsync();
    }
    public interface ICartService
    {
        List<ProductItemViewModel> GetProducts();
        void Add(int productId);
        void Remove(int productId);
        bool IsInCart(int productId);

        int GetCartCount();
    }

}