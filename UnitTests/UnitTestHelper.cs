using System;
using System.Linq;
using AutoMapper;
using BLL;
using DAL;
using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace UnitTests
{
    internal static class UnitTestHelper
    {
        public static DbContextOptions<ApplicationContext> GetUnitTestDbOptions()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var context = new ApplicationContext(options))
            {
                SeedData(context);
            }
            return options;
        }

        public static void SeedData(ApplicationContext context)
        {
            var faculties = new Faculty[]
            {
                new Faculty() { Name = "ФІОТ", CreationDate = new DateTime(2021, 11, 14) },
                new Faculty() { Name = "ФПМ", CreationDate = new DateTime(2021, 11, 14) }
            };
            foreach(var faculty in faculties)
            {
                context.Faculties.Add(faculty);
                context.SaveChanges();
            }
            var flows = new Flow[]
            {
                new Flow() { Name = "ІС-9", CreationDate = new DateTime(2021, 11, 14), Faculty = faculties[0] },
                new Flow() { Name = "ІС-0", CreationDate = new DateTime(2021, 11, 14), Faculty = faculties[0] },
                new Flow() { Name = "ІС-0", Postfix = "мп", CreationDate = new DateTime(2021, 11, 14), Faculty = faculties[0] },
                new Flow() { Name = "ІС-0", Postfix = "мн", CreationDate = new DateTime(2021, 11, 14), Faculty = faculties[0] },
                new Flow() { Name = "ІА-0", CreationDate = new DateTime(2021, 11, 14), Faculty = faculties[0] },
                new Flow() { Name = "КВ-0", CreationDate = new DateTime(2021, 11, 14), Faculty = faculties[1] }
            };
            foreach (var flow in flows)
            {
                context.Flows.Add(flow);
                context.SaveChanges();
            }
            var groups = new Group[]
            {
                //WARNING!!! NUMERATION OF IDs IN TABLES STARTS WITH 1
                new Group() { Number = 1, CreationDate = new DateTime(2021, 11, 14), Flow = flows[0] },//0
                new Group() { Number = 2, CreationDate = new DateTime(2021, 11, 14), Flow = flows[0] },//1
                new Group() { Number = 3, CreationDate = new DateTime(2021, 11, 14), Flow = flows[0] },//2
                new Group() { Number = 1, CreationDate = new DateTime(2021, 11, 14), Flow = flows[1] },//3
                new Group() { Number = 2, CreationDate = new DateTime(2021, 11, 14), Flow = flows[1] },//4 ІС-02 - 5 in table
                new Group() { Number = 3, CreationDate = new DateTime(2021, 11, 14), Flow = flows[1] },//5
                new Group() { Number = 1, CreationDate = new DateTime(2021, 11, 14), Flow = flows[2] },//6
                new Group() { Number = 1, CreationDate = new DateTime(2021, 11, 14), Flow = flows[3] },//7
                new Group() { Number = 1, CreationDate = new DateTime(2021, 11, 14), Flow = flows[4] },//8
                new Group() { Number = 2, CreationDate = new DateTime(2021, 11, 14), Flow = flows[4] },//9
                new Group() { Number = 3, CreationDate = new DateTime(2021, 11, 14), Flow = flows[4] },//10
                new Group() { Number = 4, CreationDate = new DateTime(2021, 11, 14), Flow = flows[4] },//11
                new Group() { Number = 1, CreationDate = new DateTime(2021, 11, 14), Flow = flows[5] },//12
                new Group() { Number = 2, CreationDate = new DateTime(2021, 11, 14), Flow = flows[5] },//13
                new Group() { Number = 3, CreationDate = new DateTime(2021, 11, 14), Flow = flows[5] }//14 КВ-03 - 15 in table
            };
            foreach (var group in groups)
            {
                context.Groups.Add(group);
                context.SaveChanges();
            }
            //context.Roles.Add(new IdentityRole() { Name = "Адміністратор"});
            //context.Roles.Add(new IdentityRole() { Name = "Голова СР КПІ" });
            //context.Roles.Add(new IdentityRole() { Name = "Голова СР Факультету" });
            //context.Roles.Add(new IdentityRole() { Name = "Староста потоку" });
            //context.Roles.Add(new IdentityRole() { Name = "Староста групи" });
            //context.Roles.Add(new IdentityRole() { Name = "Студент" });
            //context.SaveChanges();
            var users = new User[]
            {
                new User() { FirstName = "Petro", LastName = "Petrenko", Patronymic = "Petrovich", Email = "petrenko1@gmail.com", TelegramTag = "@petrenko", PasswordHash = "sample1", PasswordChanged = false, RegistrationDate = new DateTime(2021, 11, 14), Group = groups[4] },
                //0 WARNING!!! ID FOR USER IS NOT INT. IT IS STRING
                new User() { FirstName = "Ivan", LastName = "Petrenko", Patronymic = "Petrovich", Email = "petrenko2@gmail.com", TelegramTag = "@IPetrenko", PasswordHash = "sample1jjh", PasswordChanged = true, RegistrationDate = new DateTime(2021, 11, 14), Group = groups[4] },
                //1
                new User() { FirstName = "Petro", LastName = "Ivanov", Patronymic = "Petrovich", Email = "ivanov@gmail.com", TelegramTag = "@ivanov", PasswordHash = "sample1fag", PasswordChanged = true, RegistrationDate = new DateTime(2021, 11, 14), Group = groups[4] },
                //2
                new User() { FirstName = "Petro", LastName = "Petrenko", Patronymic = "Ivanovich", Email = "petrenko3@gmail.com", TelegramTag = "@PPI", PasswordHash = "sample1tnhgn", PasswordChanged = true, RegistrationDate = new DateTime(2021, 11, 14), Group = groups[4] },
                //3
                new User() { FirstName = "Petro", LastName = "Sydorenko", Patronymic = "Sydorovich", Email = "sydorenko@gmail.com", TelegramTag = "@sydorenko", PasswordHash = "sample1", PasswordChanged = false, RegistrationDate = new DateTime(2021, 11, 14), Group = groups[4] },
                //4
                new User() { FirstName = "Sydor", LastName = "Sydorenko", Patronymic = "Sydorovich", Email = "SSS@gmail.com", TelegramTag = "@SSS", PasswordHash = "sample1asd", PasswordChanged = true, RegistrationDate = new DateTime(2021, 11, 14), Group = groups[4] },
                //5
                new User() { FirstName = "Petro", LastName = "Ivanov", Patronymic = "Ivanovych", Email = "pivo@gmail.com", TelegramTag = "@sydorenko228", PasswordHash = "sdasdasdfdsf", PasswordChanged = true, RegistrationDate = new DateTime(2021, 11, 14), Group = groups[14] }
                //6
            };
            foreach (var user in users)
            {
                context.Users.Add(user);
                context.SaveChanges();
            }
            //context.UserRoles.Add(new IdentityUserRole<string>() { UserId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id, RoleId = context.Roles.FirstOrDefault(r => r.Name == "Адміністратор").Id });
            //context.UserRoles.Add(new IdentityUserRole<string>() { UserId = context.Users.FirstOrDefault(user => user.Email == "petrenko2@gmail.com").Id, RoleId = context.Roles.FirstOrDefault(r => r.Name == "Голова СР КПІ").Id });
            //context.UserRoles.Add(new IdentityUserRole<string>() { UserId = context.Users.FirstOrDefault(user => user.Email == "ivanov@gmail.com").Id, RoleId = context.Roles.FirstOrDefault(r => r.Name == "Голова СР Факультету").Id });
            //context.UserRoles.Add(new IdentityUserRole<string>() { UserId = context.Users.FirstOrDefault(user => user.Email == "petrenko3@gmail.com").Id, RoleId = context.Roles.FirstOrDefault(r => r.Name == "Староста потоку").Id });
            //context.UserRoles.Add(new IdentityUserRole<string>() { UserId = context.Users.FirstOrDefault(user => user.Email == "sydorenko@gmail.com").Id, RoleId = context.Roles.FirstOrDefault(r => r.Name == "Староста групи").Id });
            //context.UserRoles.Add(new IdentityUserRole<string>() { UserId = context.Users.FirstOrDefault(user => user.Email == "SSS@gmail.com").Id, RoleId = context.Roles.FirstOrDefault(r => r.Name == "Студент").Id });
            //context.UserRoles.Add(new IdentityUserRole<string>() { UserId = context.Users.FirstOrDefault(user => user.Email == "pivo@gmail.com").Id, RoleId = context.Roles.FirstOrDefault(r => r.Name == "Студент").Id });
            //context.SaveChanges();
            context.Bans.Add(new Ban() { DateFrom = new DateTime(2021, 11, 14), DateTo = new DateTime(2021, 11, 15), Hammer = "Ban hammer 1", Admin = users[0], User = users[6] });
            context.SaveChanges();
            context.Bans.Add(new Ban() { DateFrom = new DateTime(2021, 10, 20), DateTo = new DateTime(2021, 12, 20), Hammer = "Ban hammer 2", Admin = users[0], User = users[2] });
            context.SaveChanges();
            context.Appeals.Add(new Appeal() { Date = new DateTime(2021, 11, 14), User = users[6], Message = "Message sample", Importance = 3 });
            context.SaveChanges();
            context.Appeals.Add(new Appeal() { Date = new DateTime(2021, 11, 14), Admin = users[0], User = users[6], Message = "Hello", Response = "World" });
            context.SaveChanges();
            var votings = new Voting[]
            {
                new Voting() { Name = "Voting 1", Author = users[0], StatusSetter = users[0], CreationDate = new DateTime(2021, 11, 14), CompletionDate = new DateTime(2021, 12, 15), VisibilityTerm = 5, MinimalForPercentage = 55M, MinimalAttendancePercentage = 10.5m, Status = VotingStatus.Confirmed },
                new Voting() { Name = "Voting 2", Author = users[4], StatusSetter = users[0], CreationDate = new DateTime(2021, 11, 14), CompletionDate = new DateTime(2021, 12, 15), VisibilityTerm = 5, MinimalForPercentage = 55M, MinimalAttendancePercentage = 10.5m, Status = VotingStatus.Confirmed, Faculty = faculties[0]},
                new Voting() { Name = "Voting 3", Author = users[4], StatusSetter = users[0], CreationDate = new DateTime(2021, 11, 14), CompletionDate = new DateTime(2021, 12, 15), VisibilityTerm = 5, MinimalForPercentage = 55M, MinimalAttendancePercentage = 10.5m, Status = VotingStatus.Confirmed, Flow = flows[1] },
                new Voting() { Name = "Voting 4", Author = users[4], StatusSetter = users[0], CreationDate = new DateTime(2021, 11, 14), CompletionDate = new DateTime(2021, 12, 15), VisibilityTerm = 5, MinimalForPercentage = 55M, MinimalAttendancePercentage = 10.5m, Status = VotingStatus.Confirmed, Group = groups[4] }
            };
            foreach(var voting in votings)
            {
                context.Votings.Add(voting);
                context.SaveChanges();
            }
            var votes = new Vote[]
            {
                new Vote() { Result = VoteResult.For, Voting = votings[0], User = users[0] },
                new Vote() { Result = VoteResult.Against, Voting = votings[0], User = users[4] },
                new Vote() { Result = VoteResult.Neutral, Voting = votings[1], User = users[0] }
            };
            foreach(var vote in votes)
                context.Votes.Add(vote);
            context.SaveChanges();
        }

        public static Mapper CreateMapperProfile()
        {
            var myProfile = new AutoMapperProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));

            return new Mapper(configuration);
        }
    }
}