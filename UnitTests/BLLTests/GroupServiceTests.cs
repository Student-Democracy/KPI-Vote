﻿using AutoMapper;
using BLL.Services;
using DAL;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

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
                Assert.AreEqual(expected[i].Users.Count, actual[i].Users.Count, "Elements' users counts are not equal");
                Assert.AreEqual(expected[i].Votings.Count, actual[i].Votings.Count, "Elements' votings counts are not equal");
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
            Assert.AreEqual(expected.Users.Count, actual.Users.Count, "Elements' users counts are not equal");
            Assert.AreEqual(expected.Votings.Count, actual.Votings.Count, "Elements' votings counts are not equal");
        }
    }
}
