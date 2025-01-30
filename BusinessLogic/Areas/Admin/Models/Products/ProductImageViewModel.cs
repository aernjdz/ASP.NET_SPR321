namespace BusinessLogic.Admin.Models.Products
{
    public class ProductImageViewModel
    {
        public int Id { get; set; }

        public string public_id { get; set; } = string.Empty;

        public string? Name { get; set; }
        public int Priority { get; set; }
    }
}
