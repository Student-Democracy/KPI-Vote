using System;

namespace BLL.Models
{
    public class AppealModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public string Response { get; set; }
        public short Importance { get; set; }
        public string UserId { get; set; }
        public string AdminId { get; set; }
    }
}
