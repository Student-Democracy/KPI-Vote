﻿using BLL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    interface IAppealService : ICRUD<AppealModel>
    {
        Task<IEnumerable<AppealModel>> GetUserAppealsAsync(string userId);
        Task ResponseAppealAsync(AppealModel model);
    }
}