using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EShop_Site.Models.PageModels;

public class LoginModel : PageModel
{
    [BindProperty]
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [BindProperty]
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Логика аутентификации пользователя
        if (Email == "admin" && Password == "password")
        {
            // Пример успешного входа
            return RedirectToPage("Index");
        }

        // Если аутентификация не прошла, остаемся на странице логина
        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return Page();
    }
}