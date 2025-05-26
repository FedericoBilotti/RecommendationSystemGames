using App.Domain.Entities.User;

namespace App.Domain.Interfaces;

public interface IUserRepository
{
    public User? GetById(int userId);
    public User? GetByEmail(string email);
}