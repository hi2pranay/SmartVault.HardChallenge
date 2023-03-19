using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SQLite;

namespace SmartVault.DataGeneration.Persistence
{
    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly IConfiguration _config;

        public SqlConnectionFactory(IConfiguration config)
        {
            _config = config;
        }

        public SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(string.Format(_config?["ConnectionStrings:DefaultConnection"] ?? "", _config?["DatabaseFileName"]));
        }
    }
}
