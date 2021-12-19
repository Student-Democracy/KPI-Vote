using DAL.Entities;
using BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PL.Models
{
    public class GroupViewModel
    {
        public int Id { get; set; }

        public short Number { get; set; }

        public int FlowId { get; set; }

        public string FlowName { get; set; }    

        public IEnumerable<FlowModel> Flows { get; set; }
    }
}
