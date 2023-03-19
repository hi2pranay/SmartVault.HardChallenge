using SmartVault.DataGeneration.Models;
using System.Collections.Concurrent;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace SmartVault.DataGeneration.Repository
{
    public interface IDataGenerationRepository
    {
        Task InsertUsers(SQLiteCommand command, ConcurrentBag<User> users);

        Task<int> GetUsersCount(SQLiteConnection connection);

        Task InsertAccounts(SQLiteCommand command, ConcurrentBag<Account> accounts);

        Task<int> GetAccountsCount(SQLiteConnection connection);

        Task InsertDocuments(SQLiteCommand command, ConcurrentBag<Document> documents);

        Task<int> GetDocumentsCount(SQLiteConnection connection);
    }
}
