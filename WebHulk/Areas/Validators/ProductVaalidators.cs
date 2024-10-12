using FluentValidation;
using WebHulk.Areas.Admin.Models.Products;

namespace WebHulk.Areas.Validators
{
    public class ProductVaalidators : AbstractValidator<ProductCreateViewModel>
    {

        public ProductVaalidators() {

        /*   RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull()
                .MaximumLength(500).MinimumLength(3);

            RuleFor(x => x.Price)
                .NotNull()
                .NotEmpty()
                .GreaterThanOrEqualTo(0);*/


        }

    }
}
