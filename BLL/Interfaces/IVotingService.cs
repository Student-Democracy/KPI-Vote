﻿using BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    interface IVotingService : ICRUD<VotingModel>
    {
        Task<IEnumerable<VotingModel>> GetFilteredAndSortedForUserAsync(string userId);

        Task<IEnumerable<VotingModel>> GetFilteredAndSortedForAdminAsync();

        Task<IEnumerable<VotingModel>> GetUserVotingsAsync(string userId);
    }
}