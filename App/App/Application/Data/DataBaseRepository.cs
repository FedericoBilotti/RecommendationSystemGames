using App.Application.Interfaces;
using App.Domain.Entities;

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