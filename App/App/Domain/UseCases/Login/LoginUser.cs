using App.Domain.Entities.User;
using App.Domain.Interfaces;

namespace App.Domain.UseCases.Login;

public class LoginUser : ILogin
{
    private readonly IUserRepository _userRepository;

    public LoginUser(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task<UserOut>? Login(UserIn userIn)
    {
        return null;
    }
}