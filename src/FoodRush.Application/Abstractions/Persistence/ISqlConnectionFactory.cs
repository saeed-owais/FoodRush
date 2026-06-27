using System.Data;

namespace FoodRush.Application.Abstractions.Persistence;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}
