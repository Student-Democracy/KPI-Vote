using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PL.Models
{
    public class BlockViewModel
    {
        public int Id { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public string Hammer { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }

        public string AdminId { get; set; }

        public User Admin { get; set; }
    }
}
