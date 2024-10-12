using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Admin.Models.Category
{ 
    public class CategoryEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Image { get; set; }

        [Display(Name = "Choose an image from the PC")]
        public IFormFile? NewImage { get; set; } 
    }


}
