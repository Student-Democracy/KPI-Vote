using DAL.Entities;
using System;
using System.Collections.Generic;

namespace BLL.Models
{
    public class FacultyModel
    {
        public int Id { get; set; }

        public string Name { get; set; }       

        public DateTime CreationDate { get; set; }    

        public ICollection<int> FlowIds { get; set; }

        public ICollection<int> VotingIds { get; set; }
    }
}
