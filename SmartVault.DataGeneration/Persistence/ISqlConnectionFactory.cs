using System.Data;
using System.Data.SQLite;

namespace SmartVault.DataGeneration.Persistence
{
    public interface ISqlConnectionFactory
    {
        SQLiteConnection GetConnection();
    }
}
