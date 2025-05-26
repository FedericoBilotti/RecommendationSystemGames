using System.ComponentModel.DataAnnotations;

namespace App.Domain.Entities.User;

public class User(int userId, string userName, string email)
{
    [Key]
    public int UserId { get; set; } = userId;
    public string UserName { get; set; } = userName;
    public string Email { get; set; } = email;
}