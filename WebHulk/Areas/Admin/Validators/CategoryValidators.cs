using FluentValidation;
using WebHulk.Areas.Admin.Models.Category;

namespace WebHulk.Areas.Admin.Validators { 

    public class CategoryCreateVaalidators : AbstractValidator<CategoryCreateViewModel>
    {

        bool BeAValidImage(IFormFile file)
        {
            if (file == null) return false;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            return allowedExtensions.Contains(extension);
        }
        public CategoryCreateVaalidators()
        {

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required");


            RuleFor(x => x.Image)
                .NotNull().WithMessage("Photo is required")
                .Must(BeAValidImage).WithMessage("Please select a valid image file");
        }

         
    }

    public class CategoryEditValidators : AbstractValidator<CategoryEditViewModel> 
    {

        bool BeAValidImage(IFormFile file)
        {
            if (file == null) return false;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            return allowedExtensions.Contains(extension);
        }
        public CategoryEditValidators() 
        {
          
                // Validate the Name property
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Category name is required");

                // Validate the NewImage property (optional)
                RuleFor(x => x.NewImage)
                    .Must(BeAValidImage).When(x => x.NewImage != null)
                    .WithMessage("Please select a valid image file (.jpg, .jpeg, .png, .gif)");
        }

           
    }
}

