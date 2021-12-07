using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PL.Models
{
    public class UserProfileViewModel
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string TelegramTag { get; set; }

        public string Group { get; set; }

        public string Faculty { get; set; }

        public IEnumerable<string> Roles;

        public IEnumerable<VotingReducedViewModel> Votings { get; set; }
    }
}
