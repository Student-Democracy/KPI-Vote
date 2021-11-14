using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public class User : IdentityUser
    {
        [Column(TypeName = "nvarchar(50)")]
        public string FirstName { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string LastName { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Patronymic { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        public string TelegramTag { get; set; }

        public bool PasswordChanged { get; set; }

        public DateTime RegistrationDate { get; set; }


        public int GroupId { get; set; }
        public Group Group { get; set; }


        public ICollection<Appeal> CreatedAppeals { get; set; }
        public ICollection<Appeal> AdminnedAppeals { get; set; }

        public ICollection<Ban> Bans { get; set; }
        public ICollection<Ban> GivenBans { get; set; }

        public ICollection<Voting> CreatedVotings { get; set; }
        public ICollection<Voting> StatusSettedVotings { get; set; }

        public ICollection<Vote> Votes { get; set; }
    }
}
