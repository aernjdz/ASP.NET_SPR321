using AutoMapper;
using System.Globalization;
using WebHulk.Areas.Admin.Models.Category;
using WebHulk.Areas.Admin.Models.Products;
using DataAcess.Data.Entities;
using DataAcess.Data.Entities.Identity;
using WebHulk.Models.Account;


public class AppMapProfile : Profile
{
    public AppMapProfile()
    {
        CreateMap<CategoryEntity, CategoryItemViewModel>();
        CreateMap<CategoryEntity, WebHulk.Models.Categories.CategoryItemViewModel>();
        CreateMap<CategoryEntity, CategoryEditViewModel>();

        CreateMap<Product, WebHulk.Models.Products.ProductItemViewModel>()
            .ForMember(x => x.Images, opt => opt.MapFrom(x => x.ProductImages.Select(p => p.Image).ToArray()));

        CreateMap<Product, WebHulk.Areas.Admin.Models.Products.ProductItemViewModel>()
            .ForMember(x => x.Images, opt => opt.MapFrom(x => x.ProductImages.Select(p => p.Image).ToArray()));

        CreateMap<Product, ProductEditViewModel>()
          .ForMember(x => x.Images, opt =>
          opt.MapFrom(src => src.ProductImages
          .Select(pi => new WebHulk.Areas.Admin.Models.Products.ProductImageViewModel
          {
              Id = pi.Id,
              Name = "/images/" + pi.Image,
              Priority = pi.Priotity
          }).ToList()))
                .ForMember(x => x.Price, opt => opt.MapFrom(x => x.Price.ToString(new CultureInfo("uk-UA"))));

        CreateMap<ProductEditViewModel, Product>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.Price, opt => opt.MapFrom(x => Decimal.Parse(x.Price, new CultureInfo("uk-UA"))));

        CreateMap<UserEntity, ProfileViewModel>()
            .ForMember(x => x.FullName, opt => opt.MapFrom(x => $"{x.FirstName} {x.LastName}"));

    }
}