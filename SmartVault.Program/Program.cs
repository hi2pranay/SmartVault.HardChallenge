using Dapper;
using Microsoft.Extensions.Configuration;
using SmartVault.Program.BusinessObjects;
using System;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace SmartVault.Program
{
    partial class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }

            WriteEveryThirdFileToFile(args[0]);
            GetAllFileSizes();

            // Implement OAuthIntegration
            var oAuth = new OAuthIntegration
            {
                ProviderName = "Google",
                ClientId = "my_google_client_id",
                ClientSecret = "my_google_client_secret",
                AuthorizationEndpoint = "https://accounts.google.com/o/oauth2/auth",
                TokenEndpoint = "https://oauth2.googleapis.com/token",
                UserInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo",
                Code = "authorization_code_here",
            };
            var oAuthToken = new OAuthIntegrationImplementation(new System.Net.Http.HttpClient());
            var token = oAuthToken.GetAccessTokenAsync(oAuth.ProviderName,oAuth.ClientId, oAuth.ClientSecret, oAuth.TokenEndpoint,oAuth.Code);
        }

        private static void GetAllFileSizes()
        {
            // TODO: Implement functionality

            // Define the path to the directory
            string directoryPath = @"C:\Users\PranaySapa\Downloads\SmartVault-Interview-V4\SmartVault-Interview-V4\HardChallenge\SmartVault.Program\bin\Debug\net5.0";

            // Get the array of file paths in the directory
            string[] filePaths = Directory.GetFiles(directoryPath);

            // Calculate the total file size
            long totalFileSize = 0;
            foreach (string filePath in filePaths)
            {
                FileInfo fileInfo = new FileInfo(filePath);
                totalFileSize += fileInfo.Length;
            }

            // Print the total file size
            Console.WriteLine($"Total files:{filePaths.Length} and all file sizes: {totalFileSize} bytes");
        }

        private static void WriteEveryThirdFileToFile(string accountId)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            // TODO: Implement functionality
            using (var connection = new SQLiteConnection(@"data source=C:\Users\PranaySapa\Downloads\SmartVault-Interview-V4\SmartVault-Interview-V4\HardChallenge\SmartVault.DataGeneration\bin\Debug\net5.0\testdb.sqlite"))
            {
                var documents = connection.Query<Document>($"SELECT * FROM Document WHERE AccountId = {accountId} AND (Id % 3) = 0").ToList();

                // TODO: Write the contents of every third file to a single file.

                using (var streamWriter = new StreamWriter($"Account_{accountId}_Documents.txt"))
                {
                    foreach (var document in documents)
                    {
                        string documentContent = $"Id:{document.Id}, Name:{document.Name}, FilePath:{document.FilePath}, Length:{document.Length}, AccountId:{document.AccountId}";
                        streamWriter.WriteLine(documentContent + Environment.NewLine);
                    }
                }
            }
        }
    }
}