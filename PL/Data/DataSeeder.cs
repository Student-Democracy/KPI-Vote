using BLL.Interfaces;
using BLL.Models;
using BLL.Services;
using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PL.Data
{
    public class DataSeeder
    {
        public RoleManager<IdentityRole> RoleManager { get; set; }

        public UserManager<User> UserManager { get; set; }

        public IGroupService GroupService { get; set; }
        
        public IFlowService FlowService { get; set; }
        
        public IFacultyService FacultyService { get; set; }
        
        public IVotingService VotingService { get; set; }

        public IVoteService VoteService { get; set; }
        
        public IAppealService AppealService { get; set; }
        
        public IBlockService BlockService { get; set; }

        public async Task SeedRoles()
        {
                var roles = new string[]
                {
                    "Адміністратор",
                    "Голова СР КПІ",
                    "Голова СР Факультету",
                    "Староста потоку",
                    "Староста групи",
                    "Студент"
                };
                foreach (var role in roles)
                {
                    if (await RoleManager.FindByNameAsync(role) is null)
                        await RoleManager.CreateAsync(new IdentityRole { Name = role, NormalizedName = role.ToUpperInvariant() });
                }
        }

        public async Task SeedData()
        {
            var roles = new string[]
                {
                    "Адміністратор",
                    "Голова СР КПІ",
                    "Голова СР Факультету",
                    "Староста потоку",
                    "Староста групи",
                    "Студент"
                };
            var faculties = new FacultyModel[]
            {
                new FacultyModel() { Name = "ФІОТ", CreationDate = new DateTime(2021, 11, 14) },
                new FacultyModel() { Name = "ФПМ", CreationDate = new DateTime(2021, 11, 14) }
            };
            if (!(FacultyService is null) && !FacultyService.GetAll().Any())
            {
                foreach (var faculty in faculties)
                {
                    await FacultyService.AddAsync(faculty);
                }
            }
            var flows = new FlowModel[]
            {
                new FlowModel() { Name = "ІС-9", CreationDate = new DateTime(2021, 11, 14), FacultyId = 1 },
                new FlowModel() { Name = "ІС-0", CreationDate = new DateTime(2021, 11, 14), FacultyId = 1 },
                new FlowModel() { Name = "ІС-0", Postfix = "мп", CreationDate = new DateTime(2021, 11, 14), FacultyId = 1 },
                new FlowModel() { Name = "ІС-0", Postfix = "мн", CreationDate = new DateTime(2021, 11, 14), FacultyId = 1 },
                new FlowModel() { Name = "ІА-0", CreationDate = new DateTime(2021, 11, 14), FacultyId = 1 },
                new FlowModel() { Name = "КВ-0", CreationDate = new DateTime(2021, 11, 14), FacultyId = 2 }
            };
            if (!(FlowService is null) && !FlowService.GetAll().Any())
            {
                foreach (var flow in flows)
                {
                    await FlowService.AddAsync(flow);
                }
            }
            var groups = new GroupModel[]
            {
                //WARNING!!! NUMERATION OF IDs IN TABLES STARTS WITH 1
                new GroupModel() { Number = 1, CreationDate = new DateTime(2021, 11, 14), FlowId = 1 },//0
                new GroupModel() { Number = 2, CreationDate = new DateTime(2021, 11, 14), FlowId = 1 },//1
                new GroupModel() { Number = 3, CreationDate = new DateTime(2021, 11, 14), FlowId = 1 },//2
                new GroupModel() { Number = 1, CreationDate = new DateTime(2021, 11, 14), FlowId = 2 },//3
                new GroupModel() { Number = 2, CreationDate = new DateTime(2021, 11, 14), FlowId = 2 },//4 ІС-02 - 5 in table
                new GroupModel() { Number = 3, CreationDate = new DateTime(2021, 11, 14), FlowId = 2 },//5
                new GroupModel() { Number = 1, CreationDate = new DateTime(2021, 11, 14), FlowId = 3 },//6
                new GroupModel() { Number = 1, CreationDate = new DateTime(2021, 11, 14), FlowId = 4 },//7
                new GroupModel() { Number = 1, CreationDate = new DateTime(2021, 11, 14), FlowId = 5 },//8
                new GroupModel() { Number = 2, CreationDate = new DateTime(2021, 11, 14), FlowId = 5 },//9
                new GroupModel() { Number = 3, CreationDate = new DateTime(2021, 11, 14), FlowId = 5 },//10
                new GroupModel() { Number = 4, CreationDate = new DateTime(2021, 11, 14), FlowId = 5 },//11
                new GroupModel() { Number = 1, CreationDate = new DateTime(2021, 11, 14), FlowId = 6 },//12
                new GroupModel() { Number = 2, CreationDate = new DateTime(2021, 11, 14), FlowId = 6 },//13
                new GroupModel() { Number = 3, CreationDate = new DateTime(2021, 11, 14), FlowId = 6 }//14 КВ-03 - 15 in table
            };
            if (!(GroupService is null) && !GroupService.GetAll().Any())
            {
                foreach (var group in groups)
                {
                    await GroupService.AddAsync(group);
                }
            }
            var users = new User[]
            {
                new User() { FirstName = "Petro", LastName = "Petrenko", Patronymic = "Petrovich", Email = "petrenko1@gmail.com", TelegramTag = "@petrenko", PasswordHash = "sample1", PasswordChanged = false, RegistrationDate = new DateTime(2021, 11, 14), GroupId = 5 },
                //0 WARNING!!! ID FOR USER IS NOT INT. IT IS STRING
                new User() { FirstName = "Ivan", LastName = "Petrenko", Patronymic = "Petrovich", Email = "petrenko2@gmail.com", TelegramTag = "@IPetrenko", PasswordHash = "sample1jjh", PasswordChanged = true, RegistrationDate = new DateTime(2021, 11, 14), GroupId = 5 },
                //1
                new User() { FirstName = "Petro", LastName = "Ivanov", Patronymic = "Petrovich", Email = "ivanov@gmail.com", TelegramTag = "@ivanov", PasswordHash = "sample1fag", PasswordChanged = true, RegistrationDate = new DateTime(2021, 11, 14), GroupId = 5 },
                //2
                new User() { FirstName = "Petro", LastName = "Petrenko", Patronymic = "Ivanovich", Email = "petrenko3@gmail.com", TelegramTag = "@PPI", PasswordHash = "sample1tnhgn", PasswordChanged = true, RegistrationDate = new DateTime(2021, 11, 14), GroupId = 5 },
                //3
                new User() { FirstName = "Petro", LastName = "Sydorenko", Patronymic = "Sydorovich", Email = "sydorenko@gmail.com", TelegramTag = "@sydorenko", PasswordHash = "sample1", PasswordChanged = false, RegistrationDate = new DateTime(2021, 11, 14), GroupId = 5 },
                //4
                new User() { FirstName = "Sydor", LastName = "Sydorenko", Patronymic = "Sydorovich", Email = "SSS@gmail.com", TelegramTag = "@SSS", PasswordHash = "sample1asd", PasswordChanged = true, RegistrationDate = new DateTime(2021, 11, 14), GroupId = 5 },
                //5
                new User() { FirstName = "Petro", LastName = "Ivanov", Patronymic = "Ivanovych", Email = "pivo@gmail.com", TelegramTag = "@sydorenko228", PasswordHash = "sdasdasdfdsf", PasswordChanged = true, RegistrationDate = new DateTime(2021, 11, 14), GroupId = 15 }
                //6
            };
            if (!(UserManager is null))
            {
                int count = 0;
                foreach (var user in users)
                {
                    User newUser = await UserManager.FindByEmailAsync(user.Email);
                    if (newUser is null)
                    {
                        user.UserName = user.Email;
                        user.PasswordHash = null;
                        await UserManager.CreateAsync(user, "P@$$w0rd");
                        newUser = await UserManager.FindByEmailAsync(user.Email);
                        await UserManager.AddToRoleAsync(newUser, "Студент");
                    }
                    if (count <= 4 && !await UserManager.IsInRoleAsync(newUser, roles[count]))
                    {
                        await UserManager.AddToRoleAsync(newUser, roles[count]);
                    }
                    count++;
                }
            }
        }
    }
}
