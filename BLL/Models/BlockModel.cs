using DAL.Entities;
using System;
using System.Collections.Generic;

namespace BLL.Models
{
    public class BlockModel
    {
        public int Id { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public string Hammer { get; set; }

        public string UserId { get; set; }

        public string AdminId { get; set; }

    }
}
