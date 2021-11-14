using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public class Flow
    {
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        public string Name { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        public string Postfix { get; set; }

        public DateTime CreationDate { get; set; }


        public int FacultyId { get; set; }
        public Faculty Faculty { get; set; }


        public ICollection<Group> Groups { get; set; }

        public ICollection<Voting> Votings { get; set; }
    }
}
