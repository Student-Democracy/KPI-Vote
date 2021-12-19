using BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IVotingService : ICRUD<VotingModel>
    {
        Task<IEnumerable<VotingModel>> GetFilteredAndSortedForUserAsync(string userId);

        Task<IEnumerable<VotingModel>> GetFilteredAndSortedForAdminAsync();

        Task<IEnumerable<VotingModel>> GetNotConfirmedAsync();

        Task<IEnumerable<VotingModel>> GetUserVotingsAsync(string userId);

        Task ChangeStatusAsync(VotingModel model);

        Task<bool> IsVotingSuccessfulAsync(VotingModel model);

        Task<bool> IsVotingSuccessfulNowAsync(VotingModel model);

        Task<decimal> GetActualAttendancePercentageAsync(VotingModel model);

        Task<decimal> GetActualForPercentageAsync(VotingModel model);

        Task<int> GetVotersNumberAsync(VotingModel model);

        Task<int> GetVotersForNumberAsync(VotingModel model);

        Task<string> GetVotingStatusAsync(VotingModel model);

        Task<string> GetVotingLevelAsync(VotingModel model);
    }
}
