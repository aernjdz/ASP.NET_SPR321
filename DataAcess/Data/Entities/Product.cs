using System.ComponentModel.DataAnnotations;

namespace DataAcess.Data.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(255)]
        public string Name { get; set; } = String.Empty;
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public CategoryEntity Category { get; set; } 
        public ICollection<ProductImage> ProductImages { get; set; } 
    }

}
