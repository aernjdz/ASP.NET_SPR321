using System.ComponentModel.DataAnnotations;

namespace DataAcess.Data.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public CategoryEntity Category { get; set; } 
        public ICollection<ProductImage> ProductImages { get; set; } 
    }

}
