using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EShop_Site.Models;

[Serializable]
public class ProductForm
{
    public Guid ProductId { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [DataType(DataType.Text)]
    [StringLength(30, MinimumLength = 2,
        ErrorMessage = "The name must be a minimum of 2 and a maximum of 30 characters")]
    public string Name { get; set; } = "";
    
    [Required(ErrorMessage = "Weight is required")]
    [Range(1, Int32.MaxValue, ErrorMessage = "Weight must be greater than zero")]
    public int? WeightInGrams { get; set; }
    
    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, Double.MaxValue, ErrorMessage = "Price must be greater than zero")]
    public decimal? PricePerUnit { get; set; }

    [Required(ErrorMessage = "Description is required")]
    [DataType(DataType.Text)]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "The company description must be a minimum of 2 and a maximum of 200 characters")]
    public string Description { get; set; }  = "";
    
    [MaybeNull]
    [DataType(DataType.Text)]
    [StringLength(300, MinimumLength = 2, ErrorMessage = "The URL must be a minimum of 2 and a maximum of 300 characters")]
    public string? ImageUrl { get; set; }
    
    [MaybeNull]
    [Range(1, Double.MaxValue, ErrorMessage = "In stock number must be greater than zero")]
    public int? InStock { get; set; }

    public Guid SellerId { get; set; }
}