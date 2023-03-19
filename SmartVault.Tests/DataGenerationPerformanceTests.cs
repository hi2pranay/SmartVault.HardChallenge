using Moq;
using SmartVault.DataGeneration;
using SmartVault.DataGeneration.Models;
using SmartVault.DataGeneration.Persistence;
using SmartVault.DataGeneration.Repository;
using System.Collections.Concurrent;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace SmartVault.Tests
{
    public class DataGenerationPerformanceTests
    {
        Mock<ISqlConnectionFactory> mockConnection = null;
        Mock<ISQLiteCommand> mockCommand = null;

        Mock<IDataGenerationRepository> dataGenerationReposity = null;
        Mock<IPerformanceImprovement> performanceImprovement = null;

        [SetUp]
        public void Setup()
        {
            // Arrange
            mockConnection = new Mock<ISqlConnectionFactory>();
            mockCommand = new Mock<ISQLiteCommand>();
            dataGenerationReposity = new Mock<IDataGenerationRepository>();
            performanceImprovement = new Mock<IPerformanceImprovement>();
        }

        [Test]
        public async Task DataGenerationInsert_Test()
        {
            // Arrange
            mockConnection.Setup(s => s.GetConnection()).Returns(new SQLiteConnection());
            var connection = mockConnection.Object.GetConnection();

            mockCommand.Setup(s => s.GetSQLiteCommand(connection)).Returns(new SQLiteCommand());
            var command = mockCommand.Object.GetSQLiteCommand(connection);


            var users = new ConcurrentBag<User>();
            var accounts = new ConcurrentBag<Account>();
            var documents = new ConcurrentBag<Document>();

            Parallel.For(0, 100, i =>
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

                // Documents
                Parallel.For(0, 10000, d =>
                {
                    int documentNumber = i * 10000 + d;

                    var documentPath = "TestDoc.txt";
                    var document = new Document
                    {
                        Id = documentNumber,
                        Name = $"Document{i}-{d}.txt",
                        FilePath = documentPath,
                        Length = 1000,
                        AccountId = i
                    };
                    documents.Add(document);
                });
            });

            // Act
            dataGenerationReposity.Setup(s => s.InsertUsers(command, users));
            dataGenerationReposity.Setup(s => s.InsertAccounts(command, accounts));
            dataGenerationReposity.Setup(s => s.InsertDocuments(command, documents));

            dataGenerationReposity.Setup(s=>s.GetUsersCount(connection)).Returns(Task.FromResult(100));
            int usersCount = await dataGenerationReposity.Object.GetUsersCount(connection);

            dataGenerationReposity.Setup(s => s.GetAccountsCount(connection)).Returns(Task.FromResult(100));
            int accountsCount = await dataGenerationReposity.Object.GetAccountsCount(connection);

            dataGenerationReposity.Setup(s => s.GetDocumentsCount(connection)).Returns(Task.FromResult(1000000));
            int documentsCount = await dataGenerationReposity.Object.GetDocumentsCount(connection);

            //Assert  

            // Users count
            Assert.IsTrue(usersCount == 100);

            // Accounts count
            Assert.IsTrue(accountsCount == 100);

            // Documents count
            Assert.IsTrue(documentsCount == 1000000);
        }

        [Test]
        public async Task ImprovePerformance_Test()
        {
            // Arrange
            mockConnection.Setup(s => s.GetConnection()).Returns(new SQLiteConnection());
            var connection = mockConnection.Object.GetConnection();

            performanceImprovement.Setup(s => s.ImprovePerformanceAsync(connection));

            // Act
            performanceImprovement.Object.ImprovePerformanceAsync(connection);

            // Assert
            performanceImprovement.Verify(a=>a.ImprovePerformanceAsync(connection),Times.AtLeastOnce());
        }

        private static IEnumerable<DateTime> RandomDay()
        {
            DateTime start = new DateTime(1985, 1, 1);
            Random gen = new Random();
            int range = (DateTime.Today - start).Days;
            while (true)
                yield return start.AddDays(gen.Next(range));
        }
    }
}