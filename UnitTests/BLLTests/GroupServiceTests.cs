using AutoMapper;
using BLL.Services;
using BLL.Models;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace UnitTests.BLLTests
{
    public class GroupServiceTests
    {
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            _mapper = UnitTestHelper.CreateMapperProfile();
        }

        [Test]
        public void GetAll_ReturnsRightEnumerable()
        {
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            // Arrange
            var expected = context.Groups.Include(v => v.Users).Include(v => v.Votings).ToArray();
            var service = new GroupService(context, _mapper);

            // Act
            var actual = service.GetAll().ToArray();

            // Assert
            Assert.AreEqual(expected.Length, actual.Length, "The count of elements received is not right");
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i].Id, actual[i].Id, "Elements' ids are not equal");
                Assert.AreEqual(expected[i].Number, actual[i].Number, "Elements' numbers are not equal");
                Assert.AreEqual(expected[i].Users.Count, actual[i].UserIds.Count, "Elements' users counts are not equal");
                Assert.AreEqual(expected[i].Votings.Count, actual[i].VotingIds.Count, "Elements' votings counts are not equal");
            }
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task GetByIdAsync_ReturnsRightElement(int id)
        {
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            // Arrange
            var expected = await context.Groups.Include(v => v.Users).Include(v => v.Votings).SingleOrDefaultAsync(v => v.Id == id);
            var service = new GroupService(context, _mapper);

            // Act
            var actual = await service.GetByIdAsync(id);

            // Assert
            Assert.AreEqual(expected.Id, actual.Id, "Elements' ids are not equal");
            Assert.AreEqual(expected.Number, actual.Number, "Elements' Number are not equal");
            Assert.AreEqual(expected.Users.Count, actual.UserIds.Count, "Elements' users counts are not equal");
            Assert.AreEqual(expected.Votings.Count, actual.VotingIds.Count, "Elements' votings counts are not equal");
        }
        [Test]
        public async Task AddAsync_ValidBan_AddsElement()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new GroupService(context, _mapper);
            var groupModel = new GroupModel()
            {
                CreationDate = DateTime.Now.AddDays(2),
                FlowId = Int32.Parse(context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id)
            };
            var expectedCount = context.Groups.Count() + 1;

            // Act
            await service.AddAsync(groupModel);

            // Assert
            Assert.AreEqual(expectedCount, context.Groups.Count(), "The element was not added");
            var addedBan = context.Groups.Last();
            Assert.AreEqual(groupModel.CreationDate, addedBan.CreationDate, "Elements' dates from beginning are not equal");
            Assert.AreEqual(groupModel.FlowId, addedBan.FlowId, "Elements' dates to end are not equal");
        }

        [Test]
        public void AddAsync_NullModel_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new GroupService(context, _mapper);
            GroupModel groupModel = null;
            var expectedCount = context.Groups.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.AddAsync(groupModel),
                "Method does not throw an ArgumentNullException if model is null");
        }

        [Test]
        public void AddAsync_NullAdminId_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new GroupService(context, _mapper);
            var groupModel = new GroupModel()
            {
                CreationDate = DateTime.Now.AddDays(2),
                FlowId = null
        };
            var expectedCount = context.Groups.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.AddAsync(groupModel),
                "Method does not throw an ArgumentNullException if ban's admin id is null");
        }

        public void AddAsync_EmptyAdminId_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new GroupService(context, _mapper);
            var groupModel = new GroupModel()
            {
                CreationDate = DateTime.Now.AddDays(2),
                FlowId = ''
            };
            var expectedCount = context.Groups.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.AddAsync(groupModel),
                "Method does not throw an ArgumentNullException if ban's admin id is empty");
        }

        [Test]
        public void AddAsync_NotFoundUserId_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new GroupService(context, _mapper);
            var groupModel = new GroupModel()
            {
                CreationDate = DateTime.Now.AddDays(2),
                FlowId = "kopei"
            };
            var expectedCount = context.Groups.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.AddAsync(groupModel),
                "Method does not throw an ArgumentNullException if ban's user id is not found");
        }

        [Test]
        public void AddAsync_UserIsAlreadyBlocked_ThrowsArgumentException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new GroupService(context, _mapper);
            var groupModel = new GroupModel()
            {
                CreationDate = DateTime.Now.AddDays(2),
                FlowId = Int32.Parse(context.Users.FirstOrDefault(user => user.Email == "pivo@gmail.com").Id)
            };
            var expectedCount = context.Groups.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.AddAsync(groupModel),
                "Method does not throw an ArgumentException if ban's user is such an id already blocked");
        }
 
        [Test]
        public void AddAsync_DateToLessThanDateFrom_ThrowsArgumentException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new GroupService(context, _mapper);
            var groupModel = new GroupModel()
            {
                CreationDate = DateTime.Now.AddDays(-24),
                FlowId = Int32.Parse(context.Users.FirstOrDefault(user => user.Email == "pivo@gmail.com").Id)
            };
            var expectedCount = context.Groups.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.AddAsync(groupModel),
                "Method does not throw an ArgumentException if ban's date to is less than date from");
        }


        public async Task UpdateAsync_ValidBan_AddsElement()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new GroupService(context, _mapper);
            var groupModel = new GroupModel()
            {
                CreationDate = DateTime.Now.AddDays(36),
                FlowId = Int32.Parse(context.Users.FirstOrDefault(user => user.Email == "pivo@gmail.com").Id)
            };
            var expectedCount = context.Groups.Count() + 1;

            // Act
            await service.UpdateAsync(groupModel);

            // Assert
            Assert.AreEqual(expectedCount, context.Groups.Count(), "The element was not added");
            var addedBan = context.Groups.Last();
            Assert.AreEqual(groupModel.CreationDate, addedBan.CreationDate, "Elements' dates from beginning are not equal");
            Assert.AreEqual(groupModel.FlowId, addedBan.FlowId, "Elements' dates to end are not equal");
        }

        [Test]
        public void UpdateAsync_NullModel_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new GroupService(context, _mapper);
            GroupModel groupModel = null;
            var expectedCount = context.Groups.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateAsync(groupModel),
                "Method does not throw an ArgumentNullException if model is null");
        }

        [Test]
        public void UpdateAsync_NullAdminId_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new GroupService(context, _mapper);
            var groupModel = new GroupModel()
            {
                CreationDate = DateTime.Now.AddDays(2),
                FlowId = null
            };
            var expectedCount = context.Groups.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateAsync(groupModel),
                "Method does not throw an ArgumentNullException if ban's admin id is null");
        }

        public void UpdateAsync_EmptyAdminId_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new GroupService(context, _mapper);
            var groupModel = new GroupModel()
            {
                CreationDate = DateTime.Now.AddDays(2),
                FlowId = ''
            };
            var expectedCount = context.Groups.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateAsync(groupModel),
                "Method does not throw an ArgumentNullException if ban's admin id is empty");
        }

        [Test]
        public void UpdateAsync_NotFoundAdminId_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new GroupService(context, _mapper);
            var groupModel = new GroupModel()
            {
                CreationDate = DateTime.Now.AddDays(2),
                FlowId = "kopei"
            };
            var expectedCount = context.Groups.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateAsync(groupModel),
                "Method does not throw an ArgumentNullException if ban's admin id is not found");
        }

        [Test]
        public void UpdateAsync_UserIsAlreadyBlocked_ThrowsArgumentException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new GroupService(context, _mapper);
            var groupModel = new GroupModel()
            {
                CreationDate = DateTime.Now.AddDays(2),
                FlowId = Int32.Parse(context.Users.FirstOrDefault(user => user.Email == "pivo@gmail.com").Id)
            };
            var expectedCount = context.Groups.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateAsync(groupModel),
                "Method does not throw an ArgumentException if ban's user is such an id already blocked");
        }

        [Test]
        public void UpdateAsync_DateToLessThanDateFrom_ThrowsArgumentException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new GroupService(context, _mapper);
            var groupModel = new GroupModel()
            {
                CreationDate = DateTime.Now.AddDays(-23),
                FlowId = Int32.Parse(context.Users.FirstOrDefault(user => user.Email == "pivo@gmail.com").Id)

            };
            var expectedCount = context.Groups.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateAsync(groupModel),
                "Method does not throw an ArgumentException if ban's date to is less than date from");
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task DeleteByIdAsync_ValidId_DeletesElement(int id)
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new GroupService(context, _mapper);
            var expectedCount = context.Groups.Count() - 1;

            // Act
            await service.DeleteByIdAsync(id);
            var actual = context.Groups;

            // Assert
            Assert.AreEqual(expectedCount, actual.Count(), "The element was not deleted");
            Assert.AreEqual(await actual.FindAsync(id), null, "Wrong element was deleted");
        }

        [Test]
        public void DeleteByIdAsync_NotValidId_ThrowsInvalidOperationException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new GroupService(context, _mapper);
            var id = -1;

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await service.DeleteByIdAsync(id),
                "Method does not throw an InvalidOperationException if status setter was not found");
        }


        /*private static void AddDataForSpecificTests(ApplicationContext context)
        {
            context.Groups.Add(new Group() { Number = 5, CreationDate = new DateTime(2025, 11, 15), Flow = new Flow("name", "ms", new DateTime(2025, 11, 1)), User = context.Users.FirstOrDefault(user => user.Email == "SSS@gmail.com") });
            context.SaveChanges();
            context.Groups.Add(new Ban() { DateFrom = new DateTime(2021, 10, 20), DateTo = new DateTime(2025, 12, 20), Hammer = "Ban hammer 4", Admin = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), User = context.Users.FirstOrDefault(user => user.Email == "petrenko3@gmail.com") });
            context.SaveChanges();
        }*/
    }
}
