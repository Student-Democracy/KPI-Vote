using BLL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IVoteService
    {
        Task AddAsync(VoteModel model);

        IEnumerable<VoteModel> GetAll();

        Task<VoteModel> GetByIdAsync(string userId, int votingId);
    }
}
