using System;
using System.Data.SQLite;

namespace SmartVault.DataGeneration.Persistence
{
    public class SQLiteCommandWrapper : ISQLiteCommand
    {
        public SQLiteCommand GetSQLiteCommand(SQLiteConnection connection)
        {
            return connection.CreateCommand();
        }
    }
}
