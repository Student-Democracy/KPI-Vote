namespace DAL.Entities
{
    public enum VoteResult : short
    {
        Against = 0,
        For = 1,
        Neutral = 2
    }

    public class Vote
    {
        public VoteResult Result { get; set; }


        public string UserId { get; set; }
        public User User { get; set; }

        public int VotingId { get; set; }
        public Voting Voting { get; set; }
    }
}
