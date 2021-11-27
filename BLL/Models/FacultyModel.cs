using DAL.Entities;
using System;
using System.Collections.Generic;

namespace BLL.Models
{
    public class FacultyModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Postfix { get; set; }

        public DateTime CreationDate { get; set; }


        public int FacultyId { get; set; }


        public ICollection<Group> Groups { get; set; }

        public ICollection<Voting> Votings { get; set; }
    }
}
