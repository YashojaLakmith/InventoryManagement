using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Api.Features.Users;

public class User : IdentityUser<int>
{
    private User() { }

    public static User Create(string email, string userName)
        => new(email, userName);

    private User(string email, string userName)
    {
        Email = email;
        UserName = userName;
    }
}
