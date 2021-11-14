using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public class Appeal
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string Message { get; set; }

        public string Response { get; set; }

        public short Importance { get; set; }


        public string UserId { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("CreatedAppeals")]
        public User User { get; set; }

        public string AdminId { get; set; }
        [ForeignKey("AdminId")]
        [InverseProperty("AdminnedAppeals")]
        public User Admin { get; set; }
    }
}
