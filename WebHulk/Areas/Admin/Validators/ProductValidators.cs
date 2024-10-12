using FluentValidation;
using WebHulk.Areas.Admin.Models.Products;

namespace WebHulk.Areas.Admin.Validators
{
    public class ProductCreatelValidator : AbstractValidator<ProductCreateViewModel>
    {
        public ProductCreatelValidator()
        {

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required")
                .MaximumLength(500).WithMessage("Product name should not exceed 500 characters");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero");


            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Choose a category");


            RuleFor(x => x.Photos)
                .Must(photos => BeValidImages(photos)).When(x => x.Photos != null && x.Photos.Any())
                .WithMessage("All files must be valid image files (.jpg, .jpeg, .png, .gif)");
        }


        private bool BeValidImages(List<IFormFile>? photos)
        {
            if (photos == null) return true;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            foreach (var photo in photos)
            {
                var extension = Path.GetExtension(photo.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                {
                    return false;
                }
            }
            return true;
        }
    }

    public class ProductEditValidator : AbstractValidator<ProductEditViewModel>
    {
        public ProductEditValidator()
        {
          
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required");

          
            RuleFor(x => x.Price)
                .Matches(@"^\d+([\,\.]\d{1,})?$").When(x => !string.IsNullOrEmpty(x.Price))
                .WithMessage("Provide valid price");

        
            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Choose a category");

         
            RuleFor(x => x.NewImages)
                .Must(BeValidImages).When(x => x.NewImages != null && x.NewImages.Any())
                .WithMessage("All files must be valid image files (.jpg, .jpeg, .png, .gif)");
        }

        private bool BeValidImages(List<IFormFile>? newImages)
        {
            if (newImages == null) return true;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            foreach (var image in newImages)
            {
                var extension = Path.GetExtension(image.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                {
                    return false;
                }
            }
            return true;
        }
    }
}