using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SmartVault.DataGeneration.Persistence;
using SmartVault.Library;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SmartVault.DataGeneration
{
    partial class Program
    {
        private static readonly ISqlConnectionFactory _sqlConnectionFactory;
        private static readonly IConfiguration configuration;
        private static readonly IPerformanceImprovement performanceImprovement;

        static Program()
        {
            configuration = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json").Build();

            _sqlConnectionFactory = new SmartVault.DataGeneration.Persistence.SqlConnectionFactory(configuration);
            performanceImprovement = new PerformanceImprovement();
        }

        static async Task Main(string[] args)
        {
            SQLiteConnection.CreateFile(configuration["DatabaseFileName"]);
            File.WriteAllText("TestDoc.txt", $"This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}");

            using (var connection = _sqlConnectionFactory.GetConnection())
            {
                var files = Directory.GetFiles(@"..\..\..\..\BusinessObjectSchema");
                for (int i = 0; i <= 2; i++)
                {
                    var serializer = new XmlSerializer(typeof(BusinessObject));
                    var businessObject = serializer.Deserialize(new StreamReader(files[i])) as BusinessObject;
                    connection.Execute(businessObject?.Script);
                }

                Stopwatch watch = new Stopwatch();
                watch.Start();

                // Performance improvement for DataGeneration, which is less < 2 min
                await performanceImprovement.ImprovePerformanceAsync(connection);

                watch.Stop();
                TimeSpan t = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds);
                string answer = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                                        t.Hours,
                                        t.Minutes,
                                        t.Seconds,
                                        t.Milliseconds);
                Console.WriteLine($"Execution Time: {answer}");

                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                //var documentNumber = 0;
                //for (int i = 0; i < 100; i++)
                //{
                //    var randomDayIterator = RandomDay().GetEnumerator();
                //    randomDayIterator.MoveNext();
                //    connection.Execute($"INSERT INTO User (Id, FirstName, LastName, DateOfBirth, AccountId, Username, Password) VALUES('{i}','FName{i}','LName{i}','{randomDayIterator.Current.ToString("yyyy-MM-dd")}','{i}','UserName-{i}','e10adc3949ba59abbe56e057f20f883e')");
                //    connection.Execute($"INSERT INTO Account (Id, Name) VALUES('{i}','Account{i}')");

                //    for (int d = 0; d < 10000; d++, documentNumber++)
                //    {
                //        var documentPath = new FileInfo("TestDoc.txt").FullName;
                //        connection.Execute($"INSERT INTO Document (Id, Name, FilePath, Length, AccountId) VALUES('{documentNumber}','Document{i}-{d}.txt','{documentPath}','{new FileInfo(documentPath).Length}','{i}')");
                //    }
                //}

                var accountData = connection.Query("SELECT COUNT(*) FROM Account;");
                Console.WriteLine($"AccountCount: {JsonConvert.SerializeObject(accountData)}");
                var documentData = connection.Query("SELECT COUNT(*) FROM Document;");
                Console.WriteLine($"DocumentCount: {JsonConvert.SerializeObject(documentData)}");
                var userData = connection.Query("SELECT COUNT(*) FROM User;");
                Console.WriteLine($"UserCount: {JsonConvert.SerializeObject(userData)}");
            }
        }

        
    }
}