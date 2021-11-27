using DAL.Entities;
using System;
using System.Collections.Generic;

namespace BLL.Models
{
    public class VotingModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal MinimalForPercentage { get; set; }

        public decimal MinimalAttendancePercentage { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime CompletionDate { get; set; }

        public short VisibilityTerm { get; set; }

        public VotingStatus Status { get; set; }


        public string AuthorId { get; set; }

        public string StatusSetterId { get; set; }

        public int? FacultyId { get; set; }

        public int? FlowId { get; set; }

        public int? GroupId { get; set; }


        public ICollection<VoteModel> Votes { get; set; }
    }
}
