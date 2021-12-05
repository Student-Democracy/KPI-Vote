using AutoMapper;
using BLL.Models;
using BLL.Services;
using DAL;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTests.BLLTests
{
    public class AppealServiceTests
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
            var expected = context.Appeals.ToArray();
            var service = new AppealService(context, _mapper);

            // Act
            var actual = service.GetAll().ToArray();

            // Assert
            Assert.AreEqual(expected.Length, actual.Length, "The count of elements received is not right");
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i].Id, actual[i].Id, "Elements' ids are not equal");
                Assert.AreEqual(expected[i].Message, actual[i].Message, "Elements' messages are not equal");
            }
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task GetByIdAsync_ReturnsRightElement(int id)
        {
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            // Arrange
            var expected = await context.Appeals.SingleOrDefaultAsync(v => v.Id == id);
            var service = new AppealService(context, _mapper);

            // Act
            var actual = await service.GetByIdAsync(id);

            // Assert
            Assert.AreEqual(expected.Id, actual.Id, "Elements' ids are not equal");
            Assert.AreEqual(expected.Message, actual.Message, "Elements' messages are not equal");
        }

        [Test]
        public async Task AddAsync_ValidAppeal_AddsElement()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new AppealService(context, _mapper);
            var appealModel = new AppealModel()
            {
                Message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. ",
                Importance = 1,
                Date = DateTime.Now,
                UserId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Appeals.Count() + 1;

            // Act
            await service.AddAsync(appealModel);

            // Assert
            Assert.AreEqual(expectedCount, context.Appeals.Count(), "The element was not added");
            var addedAppeal = context.Appeals.Last();
            Assert.AreEqual(appealModel.Message, addedAppeal.Message, "Elements' results are not equal");
            Assert.AreEqual(appealModel.Importance, addedAppeal.Importance, "Elements' importance are not equal");
            Assert.AreEqual(appealModel.Date, addedAppeal.Date,
                "Elements' dates are not equal");
            Assert.AreEqual(appealModel.UserId, addedAppeal.UserId,
                "Elements' author ids are not equal");
        }

        [Test]
        public async Task UpdateAsync_ValidAppeal_UpdatesElement()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new AppealService(context, _mapper);
            var appealModel = new AppealModel()
            {
                Id = context.Appeals.Where(a=>a.Message == "Hello").FirstOrDefault().Id,
                Message = "Hi",
                Importance = 2,
                Date = new DateTime(2021, 11, 14),
                UserId = context.Users.FirstOrDefault(user => user.Email == "pivo@gmail.com").Id
            };

            // Act
            await service.UpdateAsync(_mapper.Map<AppealModel>(appealModel));

            // Assert
            var updatedAppeal = context.Appeals.FirstOrDefault(v => v.Id == appealModel.Id);
            Assert.AreEqual(appealModel.Message, updatedAppeal.Message, "Elements' results are not equal");
            Assert.AreEqual(appealModel.Importance, updatedAppeal.Importance, "Elements' importances are not equal");
            Assert.AreEqual(appealModel.Date, updatedAppeal.Date,
                "Elements' dates are not equal");
            Assert.AreEqual(appealModel.UserId, updatedAppeal.UserId,
                "Elements' author ids are not equal");
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task DeleteByIdAsync_ValidId_DeletesElement(int id)
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new AppealService(context, _mapper);
            var expectedCount = context.Appeals.Count() - 1;

            // Act
            await service.DeleteByIdAsync(id);
            var actual = context.Appeals;

            // Assert
            Assert.AreEqual(expectedCount, actual.Count(), "The element was not deleted");
            Assert.AreEqual(await actual.FindAsync(id), null, "Wrong element was deleted");
        }

        [Test]
        public async Task GetFilteredAndSortedForUserAsync_ValidUserId_ReturnsRightData()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new AppealService(context, _mapper);
            var user = context.Users.FirstOrDefault(u => u.Email == "pivo@gmail.com");
            var expectedMessages = new string[]
            {
                "Message sample",
                "Hello"
            };

            // Act
            var actualMessages = (await service.GetUserAppealsAsync(user.Id)).Select(a=>a.Message).ToArray();

            // Assert
            Assert.AreEqual(expectedMessages.Length, actualMessages.Length, "Arrays do not have the same Length");
            for (int i = 0; i < expectedMessages.Length; i++)
            {
                Assert.AreEqual(expectedMessages[i], actualMessages[i], "Appeals are sorted in the wrong order");
            }
        }

        [Test]
        public async Task ResponseAppealAsync_ValidAppealId_CorrectResponse()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new AppealService(context, _mapper);
            var appealModel = new AppealModel()
            {
                Id = context.Appeals.Where(a => a.Message == "Message sample").FirstOrDefault().Id,
                Message = "Message sample",
                Importance = 3,
                Date = new DateTime(2021, 11, 14),
                UserId = context.Users.FirstOrDefault(user => user.Email == "pivo@gmail.com").Id,
                Response = "World",
                AdminId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };

            // Act
            await service.ResponseAppealAsync(appealModel);

            // Assert
            var respondedAppeal = context.Appeals.Last();
            Assert.AreEqual(appealModel.Response, respondedAppeal.Response, "Elements' responses are not equal");
            Assert.AreEqual(appealModel.AdminId, respondedAppeal.AdminId,
                "Elements' admin ids are not equal");
        }
    }
}
