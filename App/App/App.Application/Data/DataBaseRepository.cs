using App.App.Domain.Entities;
using App.Application.Interfaces;

namespace App.Application.Data;

public class DataBaseRepository : IUserRepository
{
    public User? GetById(int userId)
    {
        return null;
    }

    public User? GetByEmail(string email)
    {
        return null;
    }
}