using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PL.Models
{
    public class VotingReducedViewModel
    {
        public string Name { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime CompletionDate { get; set; }

        public short VisibilityTerm { get; set; }

        public VotingStatus Status { get; set; }

        public string Level { get; set; }

        public decimal ForPercentage { get; set; }
    }
}
