using BusinessLogic.Basic.Models.Categories;

namespace BusinessLogic.Basic.Models.Products
{
    public class ProductHomeViewModel
    {
        public List<ProductItemViewModel> Products { get; set; }
        public List<CategoryItemViewModel> Categories { get; set; }
        public ProductSearchViewModel? Search { get; set; }
        public PaginationViewModel? Pagination { get; set; }
        public int Count { get; set; }
    }
}