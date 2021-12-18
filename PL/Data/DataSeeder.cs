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
            var bans = new BlockModel[]
            {
                new BlockModel() { DateTo = DateTime.Now.AddDays(2), Hammer = "Ban hammer 1", AdminId = (await UserManager.FindByEmailAsync("petrenko1@gmail.com")).Id, UserId = (await UserManager.FindByEmailAsync("pivo@gmail.com")).Id },
                new BlockModel() { DateTo = DateTime.Now.AddDays(15), Hammer = "Ban hammer 2", AdminId = (await UserManager.FindByEmailAsync("petrenko1@gmail.com")).Id, UserId = (await UserManager.FindByEmailAsync("ivanov@gmail.com")).Id }
            };
            if (!(BlockService is null) && !BlockService.GetAll().Any())
            {
                foreach (var ban in bans)
                {
                    await BlockService.AddAsync(ban);
                }
            }
            var appeals = new AppealModel[]
            {
                new AppealModel() { Date = new DateTime(2021, 11, 14), UserId = (await UserManager.FindByEmailAsync("pivo@gmail.com")).Id, Message = "Message sample", Importance = 3 },
                new AppealModel() { Date = new DateTime(2021, 11, 14), AdminId = (await UserManager.FindByEmailAsync("petrenko1@gmail.com")).Id, UserId = (await UserManager.FindByEmailAsync("pivo@gmail.com")).Id, Message = "Hello", Response = "World", Importance = 1 }
            };
            if (!(AppealService is null) && !AppealService.GetAll().Any())
            {
                foreach (var appeal in appeals)
                {
                    await AppealService.AddAsync(appeal);
                }
            }
            var votingDescription = " Mauris mattis neque eget orci pulvinar sagittis. Fusce cursus urna eget magna faucibus, et vulputate justo condimentum. Curabitur ac volutpat odio, in scelerisque dolor. Nulla facilisi. Fusce lobortis luctus turpis a luctus. Nullam at odio congue, pretium elit eget, sollicitudin ligula. Pellentesque vitae turpis non nunc commodo fringilla. In et imperdiet metus. Pellentesque tristique, ante sed vehicula ultricies, ipsum metus dapibus massa, in convallis leo urna nec enim. Interdum et malesuada fames ac ante ipsum primis in faucibus. Donec nec molestie odio, ac tristique sem. Phasellus non dui ante. Praesent tristique, diam non congue facilisis, ipsum tellus viverra nibh, eget dapibus dui urna sed velit.";
            for (int i = 0; i < 1000; i++)
                votingDescription += 'a';
            var votings = new VotingModel[]
            {
                new VotingModel() { Name = "Voting 1",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor dictum lacus, id fringilla ex ornare nec. Maecenas mollis ex in odio aliquam placerat. Morbi eu lobortis enim, at luctus mauris. Vivamus tincidunt euismod commodo. Etiam hendrerit finibus justo, vel imperdiet lectus accumsan eget. Ut sem dolor, efficitur ut odio sed, aliquam condimentum dui. Nam suscipit laoreet est at ligula.", AuthorId = (await UserManager.FindByEmailAsync("petrenko1@gmail.com")).Id, StatusSetterId = (await UserManager.FindByEmailAsync("petrenko1@gmail.com")).Id, CompletionDate = DateTime.Now.AddDays(100), VisibilityTerm = 5, MinimalForPercentage = 55M, MinimalAttendancePercentage = 10.5m, Status = VotingStatus.Confirmed },
                new VotingModel() { Name = "Voting 2", Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor dictum lacus, id fringilla ex ornare nec. Maecenas mollis ex in odio aliquam placerat. Morbi eu lobortis enim, at luctus mauris. Vivamus tincidunt euismod commodo. Etiam hendrerit finibus justo, vel imperdiet lectus accumsan eget. Ut sem dolor, efficitur ut odio sed, aliquam condimentum dui. Nam suscipit laoreet est at ligula.", AuthorId = (await UserManager.FindByEmailAsync("sydorenko@gmail.com")).Id, StatusSetterId = (await UserManager.FindByEmailAsync("petrenko1@gmail.com")).Id, CompletionDate = DateTime.Now.AddDays(30), VisibilityTerm = 30, MinimalForPercentage = 55M, MinimalAttendancePercentage = 10.5m, Status = VotingStatus.Confirmed, FacultyId = 1},
                new VotingModel() { Name = "Voting 3", Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor dictum lacus, id fringilla ex ornare nec. Maecenas mollis ex in odio aliquam placerat. Morbi eu lobortis enim, at luctus mauris. Vivamus tincidunt euismod commodo. Etiam hendrerit finibus justo, vel imperdiet lectus accumsan eget. Ut sem dolor, efficitur ut odio sed, aliquam condimentum dui. Nam suscipit laoreet est at ligula.", AuthorId = (await UserManager.FindByEmailAsync("sydorenko@gmail.com")).Id, StatusSetterId = (await UserManager.FindByEmailAsync("petrenko1@gmail.com")).Id, CompletionDate = DateTime.Now.AddDays(2), VisibilityTerm = 1, MinimalForPercentage = 55M, MinimalAttendancePercentage = 10.5m, Status = VotingStatus.Confirmed, FlowId = 2 },
                new VotingModel() { Name = "Voting 4", Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor dictum lacus, id fringilla ex ornare nec. Maecenas mollis ex in odio aliquam placerat. Morbi eu lobortis enim, at luctus mauris. Vivamus tincidunt euismod commodo. Etiam hendrerit finibus justo, vel imperdiet lectus accumsan eget. Ut sem dolor, efficitur ut odio sed, aliquam condimentum dui. Nam suscipit laoreet est at ligula.", AuthorId = (await UserManager.FindByEmailAsync("sydorenko@gmail.com")).Id, StatusSetterId = (await UserManager.FindByEmailAsync("petrenko1@gmail.com")).Id, CompletionDate = DateTime.Now.AddDays(5), VisibilityTerm = 5, MinimalForPercentage = 55M, MinimalAttendancePercentage = 10.5m, Status = VotingStatus.Confirmed, GroupId = 5 }
            };
            if (!(VotingService is null) && !VotingService.GetAll().Any())
            {
                foreach (var voting in votings)
                {
                    voting.Description += votingDescription;
                    await VotingService.AddAsync(voting);
                }
            }
            var votes = new VoteModel[]
            {
                new VoteModel() { Result = VoteResult.For, VotingId = 1, UserId = (await UserManager.FindByEmailAsync("petrenko1@gmail.com")).Id },
                new VoteModel() { Result = VoteResult.Against, VotingId = 1, UserId = (await UserManager.FindByEmailAsync("sydorenko@gmail.com")).Id },
                new VoteModel() { Result = VoteResult.Neutral, VotingId = 2, UserId = (await UserManager.FindByEmailAsync("petrenko1@gmail.com")).Id }
            };
            if (!(VoteService is null) && !VoteService.GetAll().Any())
            {
                foreach (var vote in votes)
                {
                    await VoteService.AddAsync(vote);
                }
            }
        }
    }
}
