namespace DataAcess.Data.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string Image { get; set; } = string.Empty;

        public string public_id { get; set; } = string.Empty ;
        public int Priotity { get; set; }
        public int ProductId { get; set; }
        public required Product Product { get; set; }

    }
}
