using DAL.Entities;

namespace BLL.Models
{
    public class VoteModel
    {
        public VoteResult Result { get; set; }


        public string UserId { get; set; }

        public int VotingId { get; set; }

    }
}
