using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public class Faculty
    {
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(250)")]
        public string Name { get; set; }

        public DateTime CreationDate { get; set; }


        public ICollection<Flow> Flows { get; set; }

        public ICollection<Voting> Votings { get; set; }
    }
}
