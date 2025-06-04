using System.Data;

namespace App.Interfaces;

public interface IDbConnectionFactory
{
    Task<IDbConnection> GetConnectionAsync();
}