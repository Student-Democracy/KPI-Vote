using BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    interface IBlockService : ICRUD<BlockModel>
    {
        IEnumerable<BlockModel> GetSortedActiveBlocks();  //active blocks mean that a ban is still continues

        Task<BlockModel> GetByUserIdAsync(string userId);

        IEnumerable<BlockModel> GetSortedByAdminId(string adminId);    

    }
}
