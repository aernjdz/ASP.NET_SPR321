using System.ComponentModel.DataAnnotations;

namespace WebHulk.Areas.Admin.Models.Category
{
    public class CategoryCreateViewModel
    {
        [Required(ErrorMessage = "Category name is required")]
        //[Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Select a photo on your PC")]
        [Required(ErrorMessage = "Photo is required")]
        [DataType(DataType.Upload)]
        public required IFormFile Image { get; set; }
    }
}
