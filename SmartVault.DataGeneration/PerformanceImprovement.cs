using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;
using SmartVault.DataGeneration.Models;
using SmartVault.DataGeneration.Persistence;
using SmartVault.DataGeneration.Repository;

namespace SmartVault.DataGeneration
{
    public class PerformanceImprovement : IPerformanceImprovement
    {
        private readonly IDataGenerationRepository _sqliteRepository;
        private readonly ISQLiteCommand _sqliteCommand;

        public PerformanceImprovement()
        {
            _sqliteRepository = new DataGenerationRepository();
            _sqliteCommand = new SQLiteCommandWrapper();
        }

        public async Task ImprovePerformanceAsync(SQLiteConnection connection)
        {
            int documentNumber = 0;
            var users = new ConcurrentBag<User>();
            var accounts = new ConcurrentBag<Account>();
            var documents = new ConcurrentBag<Document>();

            //Parallel.For(0, 100, i =>
            //{
            //    var randomDayIterator = RandomDay().GetEnumerator();
            //    randomDayIterator.MoveNext();

            //    // Users
            //    var user = new User
            //    {
            //        Id = i,
            //        FirstName = $"FName{i}",
            //        LastName = $"LName{i}",
            //        DateOfBirth = randomDayIterator.Current,
            //        AccountId = i,
            //        Username = $"UserName-{i}",
            //        Password = "e10adc3949ba59abbe56e057f20f883e"
            //    };
            //    users.Add(user);

            //    // Accounts
            //    var account = new Account
            //    {
            //        Id = i,
            //        Name = $"Account{i}"
            //    };
            //    accounts.Add(account);

            //    // Documents
            //    Parallel.For(0, 10000, d =>
            //    {
            //        int documentNumber = i * 10000 + d;

            //        var documentPath = new FileInfo("TestDoc.txt").FullName;
            //        var document = new Document
            //        {
            //            Id = documentNumber,
            //            Name = $"Document{i}-{d}.txt",
            //            FilePath = documentPath,
            //            Length = new FileInfo(documentPath).Length,
            //            AccountId = i
            //        };
            //        documents.Add(document);
            //    });
            //});

            //var documentNumber = 0;
            for (int i = 0; i < 100; i++)
            {
                var randomDayIterator = RandomDay().GetEnumerator();
                randomDayIterator.MoveNext();

                // Users
                var user = new User
                {
                    Id = i,
                    FirstName = $"FName{i}",
                    LastName = $"LName{i}",
                    DateOfBirth = randomDayIterator.Current,
                    AccountId = i,
                    Username = $"UserName-{i}",
                    Password = "e10adc3949ba59abbe56e057f20f883e"
                };
                users.Add(user);

                // Accounts
                var account = new Account
                {
                    Id = i,
                    Name = $"Account{i}"
                };
                accounts.Add(account);

                for (int d = 0; d < 10000; d++, documentNumber++)
                {
                    var documentPath = new FileInfo("TestDoc.txt").FullName;
                    var document = new Document
                    {
                        Id = documentNumber,
                        Name = $"Document{i}-{d}.txt",
                        FilePath = documentPath,
                        Length = new FileInfo(documentPath).Length,
                        AccountId = i
                    };
                    documents.Add(document);
                }
            }

            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                using (var command = _sqliteCommand.GetSQLiteCommand(connection))
                {
                    command.Transaction = transaction;

                    // User
                    await _sqliteRepository.InsertUsers(command, users);

                    // Account
                    await _sqliteRepository.InsertAccounts(command, accounts);

                    // Documents
                    await _sqliteRepository.InsertDocuments(command, documents);
                }
                transaction.Commit();
            }
            connection.Close();
        }

        static IEnumerable<DateTime> RandomDay()
        {
            DateTime start = new DateTime(1985, 1, 1);
            Random gen = new Random();
            int range = (DateTime.Today - start).Days;
            while (true)
                yield return start.AddDays(gen.Next(range));
        }
    }
}
