using App.Domain.Entities;

namespace App.Application.Interfaces;

public interface IUserRepository
{
    public User? GetById(int userId);
    public User? GetByEmail(string email);
}