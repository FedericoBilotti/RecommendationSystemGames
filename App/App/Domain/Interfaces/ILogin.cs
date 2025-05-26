using App.Domain.Entities.User;

namespace App.Domain.Interfaces;

public interface ILogin
{
    Task<UserOut>? Login(UserIn userIn);
}