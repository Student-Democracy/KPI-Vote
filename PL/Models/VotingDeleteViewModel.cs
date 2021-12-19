using System;

namespace PL.Models
{
    public class VotingDeleteViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime CreationDate { get; set; }

        public bool IsValid { get; set; }
    }
}
