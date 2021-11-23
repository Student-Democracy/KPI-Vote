using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public enum VotingStatus : short
    {
        Denied = -1,
        NotConfirmed = 0,
        Confirmed = 1
    }

    public class Voting
    {
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(250)")]
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal MinimalForPercentage { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime CompletionDate { get; set; }

        public short VisibilityTerm { get; set; }

        public VotingStatus Status { get; set; }

        public string AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        [InverseProperty("CreatedVotings")]
        public User Author { get; set; }

        public string StatusSetterId { get; set; }
        [ForeignKey("StatusSetterId")]
        [InverseProperty("StatusSettedVotings")]
        public User StatusSetter { get; set; }

        public int? FacultyId { get; set; }
        public Faculty Faculty { get; set; }

        public int? FlowId { get; set; }
        public Flow Flow { get; set; }

        public int? GroupId { get; set; }
        public Group Group { get; set; }


        public ICollection<Vote> Votes { get; set; }
    }
}
