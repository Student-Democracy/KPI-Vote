using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PL.Models
{
    public class UserProfileViewModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Patronymic { get; set; }

        public string Email { get; set; }

        public string TelegramTag { get; set; }

        public string Group { get; set; }

        public string Faculty { get; set; }

        public IEnumerable<string> Roles;

        public ICollection<VotingReducedViewModel> Votings { get; set; }
    }
}
