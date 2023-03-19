using System.Data.SQLite;

namespace SmartVault.DataGeneration.Persistence
{
    public interface ISQLiteCommand
    {
        SQLiteCommand GetSQLiteCommand(SQLiteConnection connection);
    }
}
