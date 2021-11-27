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


    }
}
