using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PL.Models
{
    public class VotingViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal MinimalForPercentage { get; set; }

        public decimal MinimalAttendancePercentage { get; set; }

        public decimal ForPercentage { 
            get {
                if (VotesTotally != 0)
                    return (decimal)VotesFor / VotesTotally * 100.0m;
                else
                    return 0m;
            }
        }

        public decimal AttendancePercentage { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime CompletionDate { get; set; }

        public short VisibilityTerm { get; set; }

        public string Status { get; set; }


        public string Author { get; set; }

        public string AuthorId { get; set; }

        public string StatusSetter { get; set; }

        public string StatusSetterId { get; set; }

        public string Level { get; set; }


        public int VotesFor { get; set; }

        public int VotesTotally { get; set; }

        public bool IsSuccessfulNow { get; set; }


        public VoteViewModel UserVote { get; set; }
    }
}
