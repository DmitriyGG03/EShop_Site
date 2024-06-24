using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EShop_Site.Models;

[Serializable]
public class SellerForm
{
    public Guid SellerId { get; set; } = Guid.Empty;

    [Required(ErrorMessage = "Name is required")]
    [DataType(DataType.Text)]
    [StringLength(30, MinimumLength = 2,
        ErrorMessage = "The name must be a minimum of 2 and a maximum of 30 characters")]
    public string CompanyName { get; set; } = "";
    
    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number")]
    public string ContactNumber { get; set; }  = "";
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string EmailAddress { get; set; }  = "";

    [Required(ErrorMessage = "Description is required")]
    [DataType(DataType.Text)]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "The company description must be a minimum of 2 and a maximum of 200 characters")]
    public string CompanyDescription { get; set; }  = "";
    
    [MaybeNull]
    [DataType(DataType.Text)]
    [StringLength(300, MinimumLength = 2, ErrorMessage = "The URL must be a minimum of 2 and a maximum of 300 characters")]
    public string? ImageUrl { get; set; }
    
    [MaybeNull]
    [Phone(ErrorMessage = "Invalid phone number")]
    public string? AdditionNumber { get; set; } = null;
}