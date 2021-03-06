using System;
using System.Collections.Generic;

namespace DAL.Entities
{
    public class Group
    {
        public int Id { get; set; }

        public short Number { get; set; }

        public DateTime CreationDate { get; set; }


        public int FlowId { get; set; }
        public Flow Flow { get; set; }


        public ICollection<User> Users { get; set; }

        public ICollection<Voting> Votings { get; set; }
    }
}
