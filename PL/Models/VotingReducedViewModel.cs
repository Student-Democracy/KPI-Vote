using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PL.Models
{
    public class VotingReducedViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime CreationDate { get; set; }

        public string Status { get; set; }

        public bool IsSuccessfulNow { get; set; }

        public string Level { get; set; }

        public decimal ForPercentage { get; set; }

        public bool IsUserAbleToChangeStatus { get; set; }

        public bool IsUserAbleToEdit { get; set; }
    }
}
