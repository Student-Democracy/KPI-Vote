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
        IEnumerable<VotingModel> GetFilteredAndSortedForUser(string userId);

        IEnumerable<VotingModel> GetFilteredAndSortedForAdmin();
    }
}
