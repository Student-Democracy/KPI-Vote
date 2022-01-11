using BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IBlockService : ICRUD<BlockModel>
    {
        Task<IEnumerable<BlockModel>> GetSortedActiveBlocksAsync();  //active blocks mean that a ban is still continues

        Task<BlockModel> GetByUserIdAsync(string userId);

        Task<IEnumerable<BlockModel>> GetSortedByAdminIdAsync(string adminId);

        Task DeleteByUserIdAsync(string userId);

    }
}
