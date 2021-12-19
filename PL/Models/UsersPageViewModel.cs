using DAL.Entities;
using BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PL.Models
{
    public class UsersPageViewModel
    {
        public IEnumerable<FacultyModel> Faculties { get; set; }

        public IEnumerable<FlowModel> Flows { get; set; }

        public IEnumerable<GroupModel> Groups { get; set; }

        public IEnumerable<User> Users { get; set; }

        public List<string> ActiveBlocks { get; set; }

    }
}
