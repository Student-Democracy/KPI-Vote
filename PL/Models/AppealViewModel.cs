using System;

namespace PL.Models
{
    public class AppealViewModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Response { get; set; }
        public string Admin { get; set; }
        public string User { get; set; }
        public short Importance { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
