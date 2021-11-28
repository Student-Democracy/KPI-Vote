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
    public class BlockServiceTests
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
            var expected = context.Bans.ToArray();
            var service = new BlockService(context, _mapper);

            // Act
            var actual = service.GetAll().ToArray();

            // Assert
            Assert.AreEqual(expected.Length, actual.Length, "The count of elements received is not right");
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i].Id, actual[i].Id, "Elements' ids are not equal");
                Assert.AreEqual(expected[i].Hammer, actual[i].Hammer, "Elements' hammers are not equal");
                Assert.IsNotNull(expected[i].UserId, actual[i].UserId, "Elements' user's ids are not null");
                Assert.IsNotNull(expected[i].AdminId, actual[i].AdminId, "Elements' user's ids are not null");
            }
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task GetByIdAsync_ReturnsRightElement(int id)
        {
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());

            // Arrange
            var expected = await context.Bans.SingleOrDefaultAsync(v => v.Id == id);
            var service = new BlockService(context, _mapper);

            // Act
            var actual = await service.GetByIdAsync(id);

            // Assert
            Assert.AreEqual(expected.Id, actual.Id, "Elements' ids are not equal");
            Assert.AreEqual(expected.Hammer, actual.Hammer, "Elements' hammers are not equal");
            Assert.IsNotNull(expected.UserId, actual.UserId, "Elements' user's ids are not null");
            Assert.IsNotNull(expected.AdminId, actual.AdminId, "Elements' user's ids are not null");
        }

        [Test]
        public async Task AddAsync_ValidBan_AddsElement()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var blockModel = new BlockModel()
            {
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(35),
                Hammer = "Test ban hammer consist of nothing",
                UserId = context.Users.FirstOrDefault(user => user.Email == "SSS@gmail.com").Id,
                AdminId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Bans.Count() + 1;

            // Act
            await service.AddAsync(blockModel);

            // Assert
            Assert.AreEqual(expectedCount, context.Bans.Count(), "The element was not added");
            var addedBan = context.Bans.Last();
            Assert.AreEqual(blockModel.DateFrom, addedBan.DateFrom, "Elements' dates from beginning are not equal");
            Assert.AreEqual(blockModel.DateTo, addedBan.DateTo, "Elements' dates to end are not equal");
            Assert.AreEqual(blockModel.Hammer, addedBan.Hammer, "Elements' hammers are not equal");
            Assert.AreEqual(blockModel.UserId, addedBan.UserId, "Elements' user ids are not equal");
            Assert.AreEqual(blockModel.AdminId, addedBan.AdminId, "Elements' admin ids are not equal");
        }

        [Test]
        public void AddAsync_NullModel_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            BlockModel blockModel = null;
            var expectedCount = context.Bans.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.AddAsync(blockModel),
                "Method does not throw an ArgumentNullException if model is null");
        }

        [Test]
        public void AddAsync_NullAdminId_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var blockModel = new BlockModel()
            {
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(35),
                Hammer = "Test ban hammer consist of nothing",
                UserId = context.Users.FirstOrDefault(user => user.Email == "SSS@gmail.com").Id,
                AdminId = null
            };
            var expectedCount = context.Bans.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.AddAsync(blockModel),
                "Method does not throw an ArgumentNullException if ban's admin id is null");
        }

        public void AddAsync_EmptyAdminId_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var blockModel = new BlockModel()
            {
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(35),
                Hammer = "Test ban hammer consist of nothing",
                UserId = context.Users.FirstOrDefault(user => user.Email == "SSS@gmail.com").Id,
                AdminId = ""
            };
            var expectedCount = context.Bans.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.AddAsync(blockModel),
                "Method does not throw an ArgumentNullException if ban's admin id is empty");
        }

        [Test]
        public void AddAsync_NullUserId_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var blockModel = new BlockModel()
            {
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(35),
                Hammer = "Test ban hammer consist of nothing",
                UserId = null,
                AdminId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Bans.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.AddAsync(blockModel),
                "Method does not throw an ArgumentNullException if ban's user id is null");
        }

        [Test]
        public void AddAsync_EmptyUserId_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var blockModel = new BlockModel()
            {
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(35),
                Hammer = "Test ban hammer consist of nothing",
                UserId = "",
                AdminId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Bans.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.AddAsync(blockModel),
                "Method does not throw an ArgumentNullException if ban's user id is empty");
        }

        [Test]
        public void AddAsync_NotFoundUserId_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var blockModel = new BlockModel()
            {
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(35),
                Hammer = "Test ban hammer consist of nothing",
                UserId = "kpikpikpikpi",
                AdminId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Bans.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.AddAsync(blockModel),
                "Method does not throw an ArgumentNullException if ban's user id is not found");
        }

        [Test]
        public void AddAsync_NotFoundAdminId_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var blockModel = new BlockModel()
            {
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(35),
                Hammer = "Test ban hammer consist of nothing",
                UserId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id,
                AdminId = "kpikpikpikpi"
            };
            var expectedCount = context.Bans.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.AddAsync(blockModel),
                "Method does not throw an ArgumentNullException if ban's admin id is not found");
        }

        public void AddAsync_EqualAdminIdAndUserId_ThrowsArgumentException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var blockModel = new BlockModel()
            {
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(35),
                Hammer = "Test ban hammer consist of nothing",
                UserId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id,
                AdminId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Bans.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.AddAsync(blockModel),
                "Method does not throw an ArgumentException if ban's user id and admin id is equal");
        }

        [Test]
        public void AddAsync_UserIsAlreadyBlocked_ThrowsArgumentException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var blockModel = new BlockModel()
            {
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(35),
                Hammer = "Test ban hammer consist of nothing",
                UserId = context.Users.FirstOrDefault(user => user.Email == "pivo@gmail.com").Id,
                AdminId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Bans.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.AddAsync(blockModel),
                "Method does not throw an ArgumentException if ban's user is such an id already blocked");
        }

        [Test]
        public void AddAsync_NullHammer_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var blockModel = new BlockModel()
            {
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(35),
                Hammer = null,
                UserId = context.Users.FirstOrDefault(user => user.Email == "SSS@gmail.com").Id,
                AdminId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Bans.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.AddAsync(blockModel),
                "Method does not throw an ArgumentNullException if ban's hammer is null");
        }

        [Test]
        public void AddAsync_EmptyHammer_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var blockModel = new BlockModel()
            {
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(35),
                Hammer = "",
                UserId = context.Users.FirstOrDefault(user => user.Email == "SSS@gmail.com").Id,
                AdminId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Bans.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.AddAsync(blockModel),
                "Method does not throw an ArgumentNullException if ban's hammer is empty");
        }

        [Test]
        public void AddAsync_TooShortHammer_ThrowsArgumentException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var blockModel = new BlockModel()
            {
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(35),
                Hammer = "123",
                UserId = context.Users.FirstOrDefault(user => user.Email == "SSS@gmail.com").Id,
                AdminId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Bans.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.AddAsync(blockModel),
                "Method does not throw an ArgumentException if ban's hammer is too short");
        }

        [Test]
        public void AddAsync_TooLongHammer_ThrowsArgumentException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var blockModel = new BlockModel()
            {
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(35),
                Hammer = "Fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff"+
                            "fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" +
                            "Fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" +
                            "fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" +
                            "Fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" +
                            "fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" +
                            "Fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" +
                            "fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" +
                            "Fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" +
                            "fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" +
                            "Fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" +
                            "fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" +
                            "Fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" +
                            "fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" +
                            "Fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff",
                UserId = context.Users.FirstOrDefault(user => user.Email == "SSS@gmail.com").Id,
                AdminId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Bans.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.AddAsync(blockModel),
                "Method does not throw an ArgumentException if ban's hammer is too long");
        }

        [Test]
        public void AddAsync_DateToLessThanDateFrom_ThrowsArgumentException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var blockModel = new BlockModel()
            {
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(-25),
                Hammer = "Test ban hammer consist of nothing",
                UserId = context.Users.FirstOrDefault(user => user.Email == "SSS@gmail.com").Id,
                AdminId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Bans.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.AddAsync(blockModel),
                "Method does not throw an ArgumentException if ban's date to is less than date from");
        }


        public async Task UpdateAsync_ValidBan_AddsElement()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var blockModel = new BlockModel()
            {
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(35),
                Hammer = "Test ban hammer consist of nothing",
                UserId = context.Users.FirstOrDefault(user => user.Email == "SSS@gmail.com").Id,
                AdminId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Bans.Count() + 1;

            // Act
            await service.UpdateAsync(blockModel);

            // Assert
            Assert.AreEqual(expectedCount, context.Bans.Count(), "The element was not added");
            var addedBan = context.Bans.Last();
            Assert.AreEqual(blockModel.DateFrom, addedBan.DateFrom, "Elements' dates from beginning are not equal");
            Assert.AreEqual(blockModel.DateTo, addedBan.DateTo, "Elements' dates to end are not equal");
            Assert.AreEqual(blockModel.Hammer, addedBan.Hammer, "Elements' hammers are not equal");
            Assert.AreEqual(blockModel.UserId, addedBan.UserId, "Elements' user ids are not equal");
            Assert.AreEqual(blockModel.AdminId, addedBan.AdminId, "Elements' admin ids are not equal");
        }

        [Test]
        public void UpdateAsync_NullModel_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            BlockModel blockModel = null;
            var expectedCount = context.Bans.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateAsync(blockModel),
                "Method does not throw an ArgumentNullException if model is null");
        }

        [Test]
        public void UpdateAsync_NullAdminId_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var blockModel = new BlockModel()
            {
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(35),
                Hammer = "Test ban hammer consist of nothing",
                UserId = context.Users.FirstOrDefault(user => user.Email == "SSS@gmail.com").Id,
                AdminId = null
            };
            var expectedCount = context.Bans.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateAsync(blockModel),
                "Method does not throw an ArgumentNullException if ban's admin id is null");
        }

        public void UpdateAsync_EmptyAdminId_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var blockModel = new BlockModel()
            {
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(35),
                Hammer = "Test ban hammer consist of nothing",
                UserId = context.Users.FirstOrDefault(user => user.Email == "SSS@gmail.com").Id,
                AdminId = ""
            };
            var expectedCount = context.Bans.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateAsync(blockModel),
                "Method does not throw an ArgumentNullException if ban's admin id is empty");
        }

        [Test]
        public void UpdateAsync_NullUserId_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var blockModel = new BlockModel()
            {
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(35),
                Hammer = "Test ban hammer consist of nothing",
                UserId = null,
                AdminId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Bans.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateAsync(blockModel),
                "Method does not throw an ArgumentNullException if ban's user id is null");
        }

        [Test]
        public void UpdateAsync_EmptyUserId_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var blockModel = new BlockModel()
            {
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(35),
                Hammer = "Test ban hammer consist of nothing",
                UserId = "",
                AdminId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Bans.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateAsync(blockModel),
                "Method does not throw an ArgumentNullException if ban's user id is empty");
        }

        [Test]
        public void UpdateAsync_NotFoundUserId_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var blockModel = new BlockModel()
            {
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(35),
                Hammer = "Test ban hammer consist of nothing",
                UserId = "kpikpikpikpi",
                AdminId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Bans.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateAsync(blockModel),
                "Method does not throw an ArgumentNullException if ban's user id is not found");
        }

        [Test]
        public void UpdateAsync_NotFoundAdminId_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var blockModel = new BlockModel()
            {
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(35),
                Hammer = "Test ban hammer consist of nothing",
                UserId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id,
                AdminId = "kpikpikpikpi"
            };
            var expectedCount = context.Bans.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateAsync(blockModel),
                "Method does not throw an ArgumentNullException if ban's admin id is not found");
        }

        public void UpdateAsync_EqualAdminIdAndUserId_ThrowsArgumentException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var blockModel = new BlockModel()
            {
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(35),
                Hammer = "Test ban hammer consist of nothing",
                UserId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id,
                AdminId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Bans.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateAsync(blockModel),
                "Method does not throw an ArgumentException if ban's user id and admin id is equal");
        }

        [Test]
        public void UpdateAsync_UserIsAlreadyBlocked_ThrowsArgumentException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var blockModel = new BlockModel()
            {
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(35),
                Hammer = "Test ban hammer consist of nothing",
                UserId = context.Users.FirstOrDefault(user => user.Email == "pivo@gmail.com").Id,
                AdminId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Bans.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateAsync(blockModel),
                "Method does not throw an ArgumentException if ban's user is such an id already blocked");
        }

        [Test]
        public void UpdateAsync_NullHammer_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var blockModel = new BlockModel()
            {
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(35),
                Hammer = null,
                UserId = context.Users.FirstOrDefault(user => user.Email == "SSS@gmail.com").Id,
                AdminId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Bans.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateAsync(blockModel),
                "Method does not throw an ArgumentNullException if ban's hammer is null");
        }

        [Test]
        public void UpdateAsync_EmptyHammer_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var blockModel = new BlockModel()
            {
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(35),
                Hammer = "",
                UserId = context.Users.FirstOrDefault(user => user.Email == "SSS@gmail.com").Id,
                AdminId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Bans.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateAsync(blockModel),
                "Method does not throw an ArgumentNullException if ban's hammer is empty");
        }

        [Test]
        public void UpdateAsync_TooShortHammer_ThrowsArgumentException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var blockModel = new BlockModel()
            {
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(35),
                Hammer = "123",
                UserId = context.Users.FirstOrDefault(user => user.Email == "SSS@gmail.com").Id,
                AdminId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Bans.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateAsync(blockModel),
                "Method does not throw an ArgumentException if ban's hammer is too short");
        }

        [Test]
        public void UpdateAsync_TooLongHammer_ThrowsArgumentException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var blockModel = new BlockModel()
            {
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(35),
                Hammer = "Fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" +
                            "fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" +
                            "Fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" +
                            "fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" +
                            "Fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" +
                            "fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" +
                            "Fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" +
                            "fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" +
                            "Fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" +
                            "fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" +
                            "Fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" +
                            "fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" +
                            "Fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" +
                            "fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff" +
                            "Fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff",
                UserId = context.Users.FirstOrDefault(user => user.Email == "SSS@gmail.com").Id,
                AdminId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Bans.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateAsync(blockModel),
                "Method does not throw an ArgumentException if ban's hammer is too long");
        }

        [Test]
        public void UpdateAsync_DateToLessThanDateFrom_ThrowsArgumentException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var blockModel = new BlockModel()
            {
                DateFrom = DateTime.Now.AddDays(2),
                DateTo = DateTime.Now.AddDays(-25),
                Hammer = "Test ban hammer consist of nothing",
                UserId = context.Users.FirstOrDefault(user => user.Email == "SSS@gmail.com").Id,
                AdminId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Bans.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateAsync(blockModel),
                "Method does not throw an ArgumentException if ban's date to is less than date from");
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task DeleteByIdAsync_ValidId_DeletesElement(int id)
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var expectedCount = context.Bans.Count() - 1;

            // Act
            await service.DeleteByIdAsync(id);
            var actual = context.Bans;

            // Assert
            Assert.AreEqual(expectedCount, actual.Count(), "The element was not deleted");
            Assert.AreEqual(await actual.FindAsync(id), null, "Wrong element was deleted");
        }

        [Test]
        public void DeleteByIdAsync_NotValidId_ThrowsInvalidOperationException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            var id = -1;

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await service.DeleteByIdAsync(id),
                "Method does not throw an InvalidOperationException if status setter was not found");
        }


        private static void AddDataForSpecificTests(ApplicationContext context)
        {
            context.Bans.Add(new Ban() { DateFrom = new DateTime(2021, 11, 14), DateTo = new DateTime(2025, 11, 15), Hammer = "Ban hammer 3", Admin = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), User = context.Users.FirstOrDefault(user => user.Email == "SSS@gmail.com") });
            context.SaveChanges();
            context.Bans.Add(new Ban() { DateFrom = new DateTime(2021, 10, 20), DateTo = new DateTime(2025, 12, 20), Hammer = "Ban hammer 4", Admin = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), User = context.Users.FirstOrDefault(user => user.Email == "petrenko3@gmail.com") });
            context.SaveChanges();
        }


        [Test]
        public async Task GetByUserIdAsync_ReturnsRightElement()
        {
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());

            // Arrange
            string userId = context.Users.FirstOrDefault(user => user.Email == "ivanov@gmail.com").Id;
            var expected = await context.Bans.SingleOrDefaultAsync(p => p.UserId == userId);
            var service = new BlockService(context, _mapper);

            // Act
            var actual = await service.GetByUserIdAsync(userId);

            // Assert
            Assert.AreEqual(expected.Id, actual.Id, "Elements' ids are not equal");
            Assert.AreEqual(expected.Hammer, actual.Hammer, "Elements' hammers are not equal");
            Assert.IsNotNull(expected.UserId, actual.UserId, "Elements' user's ids are not null");
            Assert.IsNotNull(expected.AdminId, actual.AdminId, "Elements' user's ids are not null");
        }

        [Test]
        public void GetByUserIdAsync_NullUserId_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            string userId = null;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.GetByUserIdAsync(userId),
                "Method does not throw an ArgumentNullException if ban's user id is null");
        }

        [Test]
        public void GetByUserIdAsync_EmptyUserId_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            string userId = "";

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.GetByUserIdAsync(userId),
                "Method does not throw an ArgumentNullException if ban's user id is empty");
        }

        [Test]
        public void GetByUserIdAsync_NotFoundUserId_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            string userId = "kpikpikpikpi";

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.GetByUserIdAsync(userId),
                "Method does not throw an ArgumentNullException if ban's user id is not found");
        }

        [Test]
        public async Task GetSortedByAdminIdAsync_ReturnsRightElement()
        {
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());

            // Arrange
            string adminId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id;
            var expectedHammers = new string[] 
            {
                "Ban hammer 1",
                "Ban hammer 2"
            };
            
            var service = new BlockService(context, _mapper);

            // Act
            var actualHammers = (await service.GetSortedByAdminIdAsync(adminId)).Select(p => p.Hammer).ToArray();

            // Assert
            Assert.AreEqual(expectedHammers.Length, actualHammers.Length, "Arrays do not have the same Length");
            for (int i = 0; i < expectedHammers.Length; i++)
            {
                Assert.AreEqual(expectedHammers[i], actualHammers[i], "Bans are sorted in the wrong order");
            }
        }

        [Test]
        public void GetSortedByAdminIdAsync_NullAdminId_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            string adminId = null;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.GetSortedByAdminIdAsync(adminId),
                "Method does not throw an ArgumentNullException if ban's admin id is null");
        }

        [Test]
        public void GetSortedByAdminIdAsync_EmptyAdminId_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            string adminId = "";

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.GetSortedByAdminIdAsync(adminId),
                "Method does not throw an ArgumentNullException if ban's admin id is empty");
        }

        [Test]
        public void GetSortedByAdminIdAsync_NotFoundAdminId_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new BlockService(context, _mapper);
            string adminId = "kpikpikpikpi";

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.GetSortedByAdminIdAsync(adminId),
                "Method does not throw an ArgumentNullException if ban's admin id is not found");
        }

        [Test]
        public async Task GetSortedActiveBlocksAsync_ReturnsRightElement()
        {
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            AddDataForSpecificTests(context);

            // Arrange            
            var expectedHammers = new string[]
            {
                "Ban hammer 3",
                "Ban hammer 2",
                "Ban hammer 4"
            };

            var service = new BlockService(context, _mapper);

            // Act
            var actualHammers = (await service.GetSortedActiveBlocksAsync()).Select(p => p.Hammer).ToArray();

            // Assert
            Assert.AreEqual(expectedHammers.Length, actualHammers.Length, "Arrays do not have the same Length");
            for (int i = 0; i < expectedHammers.Length; i++)
            {
                Assert.AreEqual(expectedHammers[i], actualHammers[i], "Bans are sorted in the wrong order");
            }
        }
        
    }
}
