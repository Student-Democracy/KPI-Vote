using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public class Ban
    {
        public int Id { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        [Column(TypeName = "nvarchar(250)")]
        public string Hammer { get; set; }


        public string UserId { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("Bans")]
        public User User { get; set; }

        public string AdminId { get; set; }
        [ForeignKey("AdminId")]
        [InverseProperty("GivenBans")]
        public User Admin { get; set; }
    }
}
