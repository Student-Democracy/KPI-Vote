using BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    interface IVotingService : ICRUD<VotingModel>
    {
        Task AddVoteAsync(VoteModel model);

        IEnumerable<VoteModel> GetAllVotes();

        Task<VoteModel> GetVoteByIdAsync(int id);
    }
}
