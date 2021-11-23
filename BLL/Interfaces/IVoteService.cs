using BLL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IVoteService
    {
        Task AddVoteAsync(VoteModel model);

        IEnumerable<VoteModel> GetAllVotes();

        Task<VoteModel> GetVoteByIdAsync(int id);
    }
}
