using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PL.Models
{
    public class FlowViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Postfix { get; set; }

        public int FacultyId { get; set; }

        public string FacultyNameChoose { get; set; }

        public List<string> Faculties { get; set; }
    }
}
