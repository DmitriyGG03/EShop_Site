namespace EShop_Site.Models;

public class UserData
{
    public Guid? UserId { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public string? Patronymic { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public string? Message { get; set; }
}