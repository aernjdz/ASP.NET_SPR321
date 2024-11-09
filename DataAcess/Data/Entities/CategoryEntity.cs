using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAcess.Data.Entities
{

    public class CategoryEntity
    {

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Image { get; set; } = String.Empty;
        public string public_id { get; set; } = string.Empty;
    }
}
