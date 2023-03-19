using Dapper;
using SmartVault.DataGeneration.Models;
using System.Collections.Concurrent;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace SmartVault.DataGeneration.Repository
{
    public class DataGenerationRepository : IDataGenerationRepository
    {
        static readonly object _object = new object();

        public async Task InsertUsers(SQLiteCommand command, ConcurrentBag<User> users)
        {
            // User
            await Task.Run(() =>
            {
                command.CommandText = "INSERT INTO User (Id, FirstName, LastName, DateOfBirth, AccountId, Username, Password) VALUES (@Id, @FirstName, @LastName, @DateOfBirth, @AccountId, @Username, @Password)";
                Parallel.ForEach(users, user =>
                {
                    if (user != null)
                    {
                        lock (_object)
                        {
                            command.Parameters.AddWithValue("@Id", user.Id);
                            command.Parameters.AddWithValue("@FirstName", user.FirstName);
                            command.Parameters.AddWithValue("@LastName", user.LastName);
                            command.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);
                            command.Parameters.AddWithValue("@AccountId", user.AccountId);
                            command.Parameters.AddWithValue("@Username", user.Username);
                            command.Parameters.AddWithValue("@Password", user.Password);

                            command.ExecuteNonQueryAsync();
                            command.Parameters.Clear();
                        }
                    }
                });
            });
        }

        public async Task InsertAccounts(SQLiteCommand command, ConcurrentBag<Account> accounts)
        {
            // Account
            await Task.Run(() =>
            {
                command.CommandText = "INSERT INTO Account (Id, Name) VALUES (@Id, @Name)";
                Parallel.ForEach(accounts, account =>
                {
                    if (account != null)
                    {
                        lock (_object)
                        {
                            command.Parameters.AddWithValue("@Id", account.Id);
                            command.Parameters.AddWithValue("@Name", account.Name);

                            command.ExecuteNonQueryAsync();
                            command.Parameters.Clear();
                        }
                    }
                });
            });
        }

        public async Task InsertDocuments(SQLiteCommand command, ConcurrentBag<Document> documents)
        {
            await Task.Run(() =>
            {
                command.CommandText = "INSERT INTO Document (Id, Name, FilePath, Length, AccountId) VALUES (@Id, @Name, @FilePath, @Length, @AccountId)";
                Parallel.ForEach(documents, document =>
                {
                    if (document != null)
                    {
                        lock (_object)
                        {
                            command.Parameters.AddWithValue("@Id", document.Id);
                            command.Parameters.AddWithValue("@Name", document.Name);
                            command.Parameters.AddWithValue("@FilePath", document.FilePath);
                            command.Parameters.AddWithValue("@Length", document.Length);
                            command.Parameters.AddWithValue("@AccountId", document.AccountId);
                            command.ExecuteNonQueryAsync();
                            command.Parameters.Clear();
                        }
                    }
                });
            });
        }

        public async Task<int> GetUsersCount(SQLiteConnection connection)
        {
            var userCount = await connection.QuerySingleOrDefaultAsync<int>("SELECT COUNT(*) FROM User;");

            return userCount;
        }

        public async Task<int> GetAccountsCount(SQLiteConnection connection)
        {
            var accountCount = await connection.QuerySingleOrDefaultAsync<int>("SELECT COUNT(*) FROM Account;");

            return accountCount;
        }

        public async Task<int> GetDocumentsCount(SQLiteConnection connection)
        {
            var documentsCount = await connection.QuerySingleOrDefaultAsync<int>("SELECT COUNT(*) FROM Document;");

            return documentsCount;
        }
    }
}
