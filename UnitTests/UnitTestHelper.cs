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
            context.Faculties.Add(new Faculty() { Name = "ФІОТ", CreationDate = new DateTime(2021, 11, 14)});
            context.Faculties.Add(new Faculty() { Name = "ФПМ", CreationDate = new DateTime(2021, 11, 14) });
            context.SaveChanges();
            context.Flows.Add(new Flow() { Name = "ІС-9", CreationDate = new DateTime(2021, 11, 14), Faculty = context.Faculties.FirstOrDefault(f => f.Name == "ФІОТ") });
            context.Flows.Add(new Flow() { Name = "ІС-0", CreationDate = new DateTime(2021, 11, 14), Faculty = context.Faculties.FirstOrDefault(f => f.Name == "ФІОТ") });
            context.Flows.Add(new Flow() { Name = "ІС-0", Postfix = "мп", CreationDate = new DateTime(2021, 11, 14), Faculty = context.Faculties.FirstOrDefault(f => f.Name == "ФІОТ") });
            context.Flows.Add(new Flow() { Name = "ІС-0", Postfix = "мн", CreationDate = new DateTime(2021, 11, 14), Faculty = context.Faculties.FirstOrDefault(f => f.Name == "ФІОТ") });
            context.Flows.Add(new Flow() { Name = "ІА-0", CreationDate = new DateTime(2021, 11, 14), Faculty = context.Faculties.FirstOrDefault(f => f.Name == "ФІОТ") });
            context.Flows.Add(new Flow() { Name = "КВ-0", CreationDate = new DateTime(2021, 11, 14), Faculty = context.Faculties.FirstOrDefault(f => f.Name == "ФПМ") });
            context.SaveChanges();
            context.Groups.Add(new Group() { Number = 1, CreationDate = new DateTime(2021, 11, 14), Flow = context.Flows.FirstOrDefault(f => f.Name == "ІС-9" && f.Postfix == null) });
            context.Groups.Add(new Group() { Number = 2, CreationDate = new DateTime(2021, 11, 14), Flow = context.Flows.FirstOrDefault(f => f.Name == "ІС-9" && f.Postfix == null) });
            context.Groups.Add(new Group() { Number = 3, CreationDate = new DateTime(2021, 11, 14), Flow = context.Flows.FirstOrDefault(f => f.Name == "ІС-9" && f.Postfix == null) });
            context.Groups.Add(new Group() { Number = 1, CreationDate = new DateTime(2021, 11, 14), Flow = context.Flows.FirstOrDefault(f => f.Name == "ІС-0" && f.Postfix == null) });
            context.Groups.Add(new Group() { Number = 2, CreationDate = new DateTime(2021, 11, 14), Flow = context.Flows.FirstOrDefault(f => f.Name == "ІС-0" && f.Postfix == null) });
            context.Groups.Add(new Group() { Number = 3, CreationDate = new DateTime(2021, 11, 14), Flow = context.Flows.FirstOrDefault(f => f.Name == "ІС-0" && f.Postfix == null) });
            context.Groups.Add(new Group() { Number = 1, CreationDate = new DateTime(2021, 11, 14), Flow = context.Flows.FirstOrDefault(f => f.Name == "ІС-0" && f.Postfix == "мп") });
            context.Groups.Add(new Group() { Number = 1, CreationDate = new DateTime(2021, 11, 14), Flow = context.Flows.FirstOrDefault(f => f.Name == "ІС-0" && f.Postfix == "мн") });
            context.Groups.Add(new Group() { Number = 1, CreationDate = new DateTime(2021, 11, 14), Flow = context.Flows.FirstOrDefault(f => f.Name == "ІА-0" && f.Postfix == null) });
            context.Groups.Add(new Group() { Number = 2, CreationDate = new DateTime(2021, 11, 14), Flow = context.Flows.FirstOrDefault(f => f.Name == "ІА-0" && f.Postfix == null) });
            context.Groups.Add(new Group() { Number = 3, CreationDate = new DateTime(2021, 11, 14), Flow = context.Flows.FirstOrDefault(f => f.Name == "ІА-0" && f.Postfix == null) });
            context.Groups.Add(new Group() { Number = 4, CreationDate = new DateTime(2021, 11, 14), Flow = context.Flows.FirstOrDefault(f => f.Name == "ІА-0" && f.Postfix == null) });
            context.Groups.Add(new Group() { Number = 1, CreationDate = new DateTime(2021, 11, 14), Flow = context.Flows.FirstOrDefault(f => f.Name == "КВ-0" && f.Postfix == null) });
            context.Groups.Add(new Group() { Number = 2, CreationDate = new DateTime(2021, 11, 14), Flow = context.Flows.FirstOrDefault(f => f.Name == "КВ-0" && f.Postfix == null) });
            context.Groups.Add(new Group() { Number = 3, CreationDate = new DateTime(2021, 11, 14), Flow = context.Flows.FirstOrDefault(f => f.Name == "КВ-0" && f.Postfix == null) });
            context.SaveChanges();
            //context.Roles.Add(new IdentityRole() { Name = "Адміністратор"});
            //context.Roles.Add(new IdentityRole() { Name = "Голова СР КПІ" });
            //context.Roles.Add(new IdentityRole() { Name = "Голова СР Факультету" });
            //context.Roles.Add(new IdentityRole() { Name = "Староста потоку" });
            //context.Roles.Add(new IdentityRole() { Name = "Староста групи" });
            //context.Roles.Add(new IdentityRole() { Name = "Студент" });
            //context.SaveChanges();
            context.Users.Add(new User() { FirstName = "Petro", LastName = "Petrenko", Patronymic = "Petrovich", Email = "petrenko1@gmail.com", TelegramTag = "@petrenko", PasswordHash = "sample1", PasswordChanged = false, RegistrationDate = new DateTime(2021, 11, 14), Group = context.Groups.FirstOrDefault(g => g.Number == 2 && g.Flow.Name == "ІС-0" && g.Flow.Postfix == null) });
            context.Users.Add(new User() { FirstName = "Ivan", LastName = "Petrenko", Patronymic = "Petrovich", Email = "petrenko2@gmail.com", TelegramTag = "@IPetrenko", PasswordHash = "sample1jjh", PasswordChanged = true, RegistrationDate = new DateTime(2021, 11, 14), Group = context.Groups.FirstOrDefault(g => g.Number == 2 && g.Flow.Name == "ІС-0" && g.Flow.Postfix == null) });
            context.Users.Add(new User() { FirstName = "Petro", LastName = "Ivanov", Patronymic = "Petrovich", Email = "ivanov@gmail.com", TelegramTag = "@ivanov", PasswordHash = "sample1fag", PasswordChanged = true, RegistrationDate = new DateTime(2021, 11, 14), Group = context.Groups.FirstOrDefault(g => g.Number == 2 && g.Flow.Name == "ІС-0" && g.Flow.Postfix == null) });
            context.Users.Add(new User() { FirstName = "Petro", LastName = "Petrenko", Patronymic = "Ivanovich", Email = "petrenko3@gmail.com", TelegramTag = "@PPI", PasswordHash = "sample1tnhgn", PasswordChanged = true, RegistrationDate = new DateTime(2021, 11, 14), Group = context.Groups.FirstOrDefault(g => g.Number == 2 && g.Flow.Name == "ІС-0" && g.Flow.Postfix == null) });
            context.Users.Add(new User() { FirstName = "Petro", LastName = "Sydorenko", Patronymic = "Sydorovich", Email = "sydorenko@gmail.com", TelegramTag = "@sydorenko", PasswordHash = "sample1", PasswordChanged = false, RegistrationDate = new DateTime(2021, 11, 14), Group = context.Groups.FirstOrDefault(g => g.Number == 2 && g.Flow.Name == "ІС-0" && g.Flow.Postfix == null) });
            context.Users.Add(new User() { FirstName = "Sydor", LastName = "Sydorenko", Patronymic = "Sydorovich", Email = "SSS@gmail.com", TelegramTag = "@SSS", PasswordHash = "sample1asd", PasswordChanged = true, RegistrationDate = new DateTime(2021, 11, 14), Group = context.Groups.FirstOrDefault(g => g.Number == 2 && g.Flow.Name == "ІС-0" && g.Flow.Postfix == null) });
            context.Users.Add(new User() { FirstName = "Petro", LastName = "Ivanov", Patronymic = "Ivanovych", Email = "pivo@gmail.com", TelegramTag = "@sydorenko", PasswordHash = "sdasdasdfdsf", PasswordChanged = true, RegistrationDate = new DateTime(2021, 11, 14), Group = context.Groups.FirstOrDefault(g => g.Number == 3 && g.Flow.Name == "КВ-0" && g.Flow.Postfix == null) });
            context.SaveChanges();
            //context.UserRoles.Add(new IdentityUserRole<string>() { UserId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id, RoleId = context.Roles.FirstOrDefault(r => r.Name == "Адміністратор").Id });
            //context.UserRoles.Add(new IdentityUserRole<string>() { UserId = context.Users.FirstOrDefault(user => user.Email == "petrenko2@gmail.com").Id, RoleId = context.Roles.FirstOrDefault(r => r.Name == "Голова СР КПІ").Id });
            //context.UserRoles.Add(new IdentityUserRole<string>() { UserId = context.Users.FirstOrDefault(user => user.Email == "ivanov@gmail.com").Id, RoleId = context.Roles.FirstOrDefault(r => r.Name == "Голова СР Факультету").Id });
            //context.UserRoles.Add(new IdentityUserRole<string>() { UserId = context.Users.FirstOrDefault(user => user.Email == "petrenko3@gmail.com").Id, RoleId = context.Roles.FirstOrDefault(r => r.Name == "Староста потоку").Id });
            //context.UserRoles.Add(new IdentityUserRole<string>() { UserId = context.Users.FirstOrDefault(user => user.Email == "sydorenko@gmail.com").Id, RoleId = context.Roles.FirstOrDefault(r => r.Name == "Староста групи").Id });
            //context.UserRoles.Add(new IdentityUserRole<string>() { UserId = context.Users.FirstOrDefault(user => user.Email == "SSS@gmail.com").Id, RoleId = context.Roles.FirstOrDefault(r => r.Name == "Студент").Id });
            //context.UserRoles.Add(new IdentityUserRole<string>() { UserId = context.Users.FirstOrDefault(user => user.Email == "pivo@gmail.com").Id, RoleId = context.Roles.FirstOrDefault(r => r.Name == "Студент").Id });
            //context.SaveChanges();
            context.Bans.Add(new Ban() { DateFrom = new DateTime(2021, 11, 14), DateTo = new DateTime(2021, 11, 15), Hammer = "Ban hammer 1", Admin = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), User = context.Users.FirstOrDefault(user => user.Email == "pivo@gmail.com") });
            context.Bans.Add(new Ban() { DateFrom = new DateTime(2021, 10, 20), DateTo = new DateTime(2021, 12, 20), Hammer = "Ban hammer 2", Admin = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), User = context.Users.FirstOrDefault(user => user.Email == "ivanov@gmail.com") });
            context.SaveChanges();
            context.Appeals.Add(new Appeal() { Date = new DateTime(2021, 11, 14), User = context.Users.FirstOrDefault(user => user.Email == "pivo@gmail.com"), Message = "Message sample", Importance = 3});
            context.Appeals.Add(new Appeal() { Date = new DateTime(2021, 11, 14), Admin = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), User = context.Users.FirstOrDefault(user => user.Email == "pivo@gmail.com"), Message = "Hello", Response = "World" });
            context.SaveChanges();
            context.Votings.Add(new Voting() { Name = "Voting 1", Author = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com") , StatusSetter = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), CreationDate = new DateTime(2021, 11, 14), CompletionDate = new DateTime(2021, 12, 15), VisibilityTerm = 5, MinimalForPercentage = 55M, MinimalAttendancePercentage = 10.5m, Status = VotingStatus.Confirmed });
            context.SaveChanges();
            context.Votings.Add(new Voting() { Name = "Voting 2", Author = context.Users.FirstOrDefault(user => user.Email == "sydorenko@gmail.com"), StatusSetter = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), CreationDate = new DateTime(2021, 11, 14), CompletionDate = new DateTime(2021, 12, 15), VisibilityTerm = 5, MinimalForPercentage = 55M, MinimalAttendancePercentage = 10.5m, Status = VotingStatus.Confirmed, Faculty = context.Faculties.FirstOrDefault(f => f.Name == "ФІОТ")});
            context.SaveChanges();
            context.Votings.Add(new Voting() { Name = "Voting 3", Author = context.Users.FirstOrDefault(user => user.Email == "sydorenko@gmail.com"), StatusSetter = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), CreationDate = new DateTime(2021, 11, 14), CompletionDate = new DateTime(2021, 12, 15), VisibilityTerm = 5, MinimalForPercentage = 55M, MinimalAttendancePercentage = 10.5m, Status = VotingStatus.Confirmed, Flow = context.Flows.FirstOrDefault(f => f.Name == "ІС-0") });
            context.SaveChanges();
            context.Votings.Add(new Voting() { Name = "Voting 4", Author = context.Users.FirstOrDefault(user => user.Email == "sydorenko@gmail.com"), StatusSetter = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), CreationDate = new DateTime(2021, 11, 14), CompletionDate = new DateTime(2021, 12, 15), VisibilityTerm = 5, MinimalForPercentage = 55M, MinimalAttendancePercentage = 10.5m, Status = VotingStatus.Confirmed, Group = context.Groups.FirstOrDefault(g => g.Number == 2 && g.Flow.Name == "ІС-0") });
            context.SaveChanges();
            context.Votes.Add(new Vote() { Result = VoteResult.For, Voting = context.Votings.FirstOrDefault(v => v.Name == "Voting 1"), User = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com") });
            context.Votes.Add(new Vote() { Result = VoteResult.Against, Voting = context.Votings.FirstOrDefault(v => v.Name == "Voting 1"), User = context.Users.FirstOrDefault(user => user.Email == "sydorenko@gmail.com") });
            context.Votes.Add(new Vote() { Result = VoteResult.Neutral, Voting = context.Votings.FirstOrDefault(v => v.Name == "Voting 2"), User = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com") });
            context.SaveChanges();
            //context.Votings.Update(new Voting() { Id = 1, Name = "Sample 1", Author = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), StatusSetter = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), CreationDate = new DateTime(2021, 11, 14), CompletionDate = new DateTime(2021, 12, 15), VisibilityTerm = 5, MinimalForPercentage = 5.5M, Status = VotingStatus.Confirmed });
        }

        public static Mapper CreateMapperProfile()
        {
            var myProfile = new AutoMapperProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(myProfile));

            return new Mapper(configuration);
        }
    }
}