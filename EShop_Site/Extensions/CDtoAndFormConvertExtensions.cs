using EShop_Site.Models;
using SharedLibrary.Models.ClientDtoModels.MainModels;
using SharedLibrary.Models.DtoModels.MainModels;

namespace EShop_Site.Extensions;

public static class CDtoAndFormConvertExtensions
{
    #region User convertor

    public static UserCDTO ToUserCDto(this UserForm userForm)
    {
        return new UserCDTO(
            userForm.Name,
            userForm.LastName,
            userForm.PhoneNumber,
            userForm.Email,
            userForm.RoleId,
            patronymic: userForm.Patronymic,
            password: userForm.Password
        )
        {
            UserCDtoId = userForm.UserId,
            SellerCDtoId = userForm.SellerId,
        };
    }

    public static UserForm ToUserForm(this UserCDTO userCDto)
    {
        return new UserForm()
        {
            UserId = userCDto.UserCDtoId,

            Name = userCDto.Name,
            LastName = userCDto.LastName,
            Patronymic = userCDto.Patronymic,

            PhoneNumber = userCDto.PhoneNumber,
            Email = userCDto.Email,

            RoleId = userCDto.RoleCDtoId,
            SellerId = userCDto.SellerCDtoId,
        };
    }

    #endregion

    #region Product convertion

    public static ProductCDTO ToProductCDto(this ProductForm productForm)
    {
        return new ProductCDTO(
            productForm.Name,
            productForm.Description,
            (decimal)productForm.PricePerUnit!,
            (int)productForm.WeightInGrams!,
            productForm.SellerId,
            imageUrl: productForm.ImageUrl
        )
        {
            ProductCDtoId = productForm.ProductId,
            InStock = productForm.InStock,
        };
    }

    public static ProductForm ToProductForm(this ProductCDTO productCDto)
    {
        return new ProductForm()
        {
            ProductId = productCDto.ProductCDtoId,

            Name = productCDto.Name,
            Description = productCDto.Description,
            PricePerUnit = productCDto.PricePerUnit,
            WeightInGrams = productCDto.WeightInGrams,
            ImageUrl = productCDto.ImageUrl,
            InStock = productCDto.InStock,

            SellerId = productCDto.SellerCDtoId,
        };
    }

    #endregion

    #region Seller convertion

    public static SellerCDTO ToSellerCDto(this SellerForm sellerForm)
    {
        return new SellerCDTO(
            sellerForm.CompanyName,
            sellerForm.ContactNumber,
            sellerForm.EmailAddress,
            sellerForm.CompanyDescription,
            imageUrl: sellerForm.ImageUrl,
            additionNumber: sellerForm.AdditionNumber
        )
        {
            SellerCDtoId = sellerForm.SellerId
        };
    }

    public static SellerForm ToSellerForm(this SellerCDTO sellerCDto)
    {
        return new SellerForm()
        {
            SellerId = sellerCDto.SellerCDtoId,
            
            CompanyName = sellerCDto.CompanyName,
            ContactNumber = sellerCDto.ContactNumber,
            EmailAddress = sellerCDto.EmailAddress,
            ImageUrl = sellerCDto.ImageUrl,
            CompanyDescription = sellerCDto.CompanyDescription,
            AdditionNumber = sellerCDto.AdditionNumber,
        };
    }

    #endregion
}