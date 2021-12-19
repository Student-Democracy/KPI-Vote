using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public class User : IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(50)")]
        public string FirstName { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(50)")]
        public string LastName { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(50)")]
        public string Patronymic { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(255)")]
        public string TelegramTag { get; set; }

        [PersonalData]
        public bool PasswordChanged { get; set; }

        [PersonalData]
        public DateTime RegistrationDate { get; set; }

        [PersonalData]
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
