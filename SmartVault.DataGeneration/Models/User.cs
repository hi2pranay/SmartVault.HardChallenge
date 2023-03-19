using System;

namespace SmartVault.DataGeneration.Models
{
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int AccountId { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
    }
}
