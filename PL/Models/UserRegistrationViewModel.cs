using DAL.Entities;
using BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PL.Models
{
    public class UserRegistrationViewModel
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Patronymic { get; set; }

        public string Email { get; set; }

        public string TelegramTag { get; set; }

        public string PasswordHash { get; set; }

        public bool PasswordChanged { get; set; }

        public DateTime RegistrationDate { get; set; }

        public int GroupId { get; set; }

        public IEnumerable<GroupModel> Groups { get; set; }

        public IEnumerable<FlowModel> Flows { get; set; }

        public string GroupName { get; set; }

        public string RoleChoose { get; set; }

        public User Author { get; set; }
    }
}
