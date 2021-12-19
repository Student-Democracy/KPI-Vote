using DAL.Entities;
using System;
using System.Collections.Generic;

namespace BLL.Models
{
    public class GroupModel
    {
        public int Id { get; set; }

        public short Number { get; set; }

        public DateTime CreationDate { get; set; }

        public int FlowId { get; set; }

        public ICollection<string> UserIds { get; set; }

        public ICollection<int> VotingIds { get; set; }
    }
}
