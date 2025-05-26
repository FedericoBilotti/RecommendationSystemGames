namespace App.Domain.Entities.User;

public class UserIn(int userId, string userName, string email, string password) : User(userId, userName, email)
{
    public string HashedPassword { get; set; } = password;
}