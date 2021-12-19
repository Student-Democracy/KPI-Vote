using DAL;
using DAL.Entities;
using BLL;
using BLL.Models;
using BLL.Services;
using NUnit.Framework;
using System;
using System.Linq;
using Moq;
using AutoMapper;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace UnitTests.BLLTests
{
    [TestFixture]
    public class VotingServiceTests
    {
        private IMapper _mapper;

        private readonly double floatingTolerance = 1.0 / Math.Pow(10, 2);

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
            var expected = context.Votings.Include(v => v.Votes).ToArray();
            var service = new VotingService(context, _mapper);

            // Act
            var actual = service.GetAll().ToArray();

            // Assert
            Assert.AreEqual(expected.Length, actual.Length, "The count of elements received is not right");
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i].Id, actual[i].Id, "Elements' ids are not equal");
                Assert.AreEqual(expected[i].Name, actual[i].Name, "Elements' names are not equal");
                Assert.AreEqual(expected[i].Votes.Count, actual[i].Votes.Count, "Elements' votes counts are not equal");
            }
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task GetByIdAsync_ReturnsRightElement(int id)
        {
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            // Arrange
            var expected = await context.Votings.Include(v => v.Votes).SingleOrDefaultAsync(v => v.Id == id);
            var service = new VotingService(context, _mapper);

            // Act
            var actual = await service.GetByIdAsync(id);

            // Assert
            Assert.AreEqual(expected.Id, actual.Id, "Elements' ids are not equal");
            Assert.AreEqual(expected.Name, actual.Name, "Elements' names are not equal");
            Assert.AreEqual(expected.Votes.Count, actual.Votes.Count, "Elements' votes counts are not equal");
        }

        [Test]
        public async Task AddAsync_ValidVote_AddsElement()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var votingModel = new VotingModel()
            {
                Name = "Sample name",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " +
                "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " +
                "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam." +
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " +
                "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " +
                "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam." +
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " +
                "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " +
                "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam.",
                MinimalForPercentage = 55m,
                CompletionDate = DateTime.Now.AddDays(30),
                MinimalAttendancePercentage = 10m,
                VisibilityTerm = 5,
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Votings.Count() + 1;

            // Act
            await service.AddAsync(votingModel);

            // Assert
            Assert.AreEqual(expectedCount, context.Votings.Count(), "The element was not added");
            var addedVoting = context.Votings.Last();
            Assert.AreEqual(votingModel.Name, addedVoting.Name, "Elements' results are not equal");
            Assert.AreEqual(votingModel.Description, addedVoting.Description, "Elements' descriptions are not equal");
            Assert.AreEqual(votingModel.MinimalForPercentage, addedVoting.MinimalForPercentage, 
                "Elements' minimal for percentages are not equal");
            Assert.AreEqual(votingModel.CompletionDate, addedVoting.CompletionDate,
                "Elements' completion dates are not equal");
            Assert.AreEqual(votingModel.VisibilityTerm, addedVoting.VisibilityTerm,
                "Elements' visibility terms are not equal");
            Assert.AreEqual(votingModel.AuthorId, addedVoting.AuthorId,
                "Elements' author ids are not equal");
            Assert.AreNotEqual((DateTime)default, addedVoting.CreationDate, "Method does not set the creation date");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void AddAsync_NullOrEmptyName_ThrowsArgumentNullException(string name)
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var votingModel = new VotingModel()
            {
                Name = name,
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " +
                "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " +
                "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam.",
                MinimalForPercentage = 55m,
                MinimalAttendancePercentage = 10m,
                CompletionDate = DateTime.Now.AddDays(30),
                VisibilityTerm = 5,
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Votings.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.AddAsync(votingModel),
                "Method does not throw an ArgumentNullException if voting's name is null");
        }

        [Test]
        public void AddAsync_NullDescription_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var votingModel = new VotingModel()
            {
                Name = "Sample one",
                Description = null,
                MinimalForPercentage = 55m,
                CompletionDate = DateTime.Now.AddDays(30),
                VisibilityTerm = 5,
                MinimalAttendancePercentage = 10m,
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Votings.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.AddAsync(votingModel),
                "Method does not throw an ArgumentNullException if voting's description is null");
        }

        [Test]
        public void AddAsync_NullAuthorId_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var votingModel = new VotingModel()
            {
                Name = "Sample one",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " +
                "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " +
                "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam."
                + "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " +
                "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " +
                "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam."
                +
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " +
                "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " +
                "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam.",
                MinimalForPercentage = 55m,
                CompletionDate = DateTime.Now.AddDays(30),
                MinimalAttendancePercentage = 10m,
                VisibilityTerm = 5,
                AuthorId = null
            };
            var expectedCount = context.Votings.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.AddAsync(votingModel),
                "Method does not throw an ArgumentNullException if voting's author id is null");
        }

        [Test]
        public void AddAsync_NullModel_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            VotingModel votingModel = null;
            var expectedCount = context.Votings.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.AddAsync(votingModel),
                "Method does not throw an ArgumentNullException if model is null");
        }

        [Test]
        public void AddAsync_TooLongName_ThrowsArgumentException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var votingModel = new VotingModel()
            {
                Name = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Ut sed elementum magna. In commodo imperdiet egestas. Praesent quis purus elit. " +
                "Praesent auctor pretium quam, in placerat tellus imperdiet id. " +
                "Aenean semper sed turpis vitae eleifend porta ante.",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " +
                "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " +
                "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam.",
                MinimalForPercentage = 55m,
                CompletionDate = DateTime.Now.AddDays(30),
                VisibilityTerm = 5,
                MinimalAttendancePercentage = 10m,
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Votings.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.AddAsync(votingModel),
                "Method does not throw an ArgumentException if the name is too long");
        }

        [Test]
        public void AddAsync_TooShortDescription_ThrowsArgumentException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var votingModel = new VotingModel()
            {
                Name = "Sample 1",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
                MinimalForPercentage = 55m,
                CompletionDate = DateTime.Now.AddDays(30),
                VisibilityTerm = 5,
                MinimalAttendancePercentage = 10m,
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Votings.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.AddAsync(votingModel),
                "Method does not throw an ArgumentException if the description is too short");
        }

        [Test]
        [TestCase(-1.1)]
        [TestCase(0)]
        [TestCase(101.1)]
        [TestCase(49.9)]
        public void AddAsync_InvalidMinForPercantage_ThrowsArgumentException(decimal percentage)
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var votingModel = new VotingModel()
            {
                Name = "Sample 1",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " +
                "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " +
                "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam.",
                MinimalForPercentage = percentage,
                MinimalAttendancePercentage = 10m,
                CompletionDate = DateTime.Now.AddDays(30),
                VisibilityTerm = 5,
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Votings.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.AddAsync(votingModel),
                "Method does not throw an ArgumentException if the minimal for percantage is 0 or lower than 0");
        }

        [Test]
        [TestCase(-1.1)]
        [TestCase(0)]
        [TestCase(101.1)]
        public void AddAsync_InvalidMinAttendancePercantage_ThrowsArgumentException(decimal percentage)
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var votingModel = new VotingModel()
            {
                Name = "Sample 1",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " +
                "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " +
                "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam.",
                MinimalForPercentage = 55m,
                MinimalAttendancePercentage = percentage,
                CompletionDate = DateTime.Now.AddDays(30),
                VisibilityTerm = 5,
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Votings.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.AddAsync(votingModel),
                "Method does not throw an ArgumentException if the minimal for percantage is 0 or lower than 0");
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(32)]
        public void AddAsync_InvalidVisibilityTerm_ThrowsArgumentException(short term)
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var votingModel = new VotingModel()
            {
                Name = "Sample 1",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " +
                "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " +
                "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam.",
                MinimalForPercentage = 55m,
                MinimalAttendancePercentage = 10m,
                CompletionDate = DateTime.Now.AddDays(30),
                VisibilityTerm = term,
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Votings.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.AddAsync(votingModel),
                "Method does not throw an ArgumentException if the visibility term is 0 or lower than 0, or bigger than 31");
        }

        [Test]
        public void AddAsync_InvalidCompletionDate_ThrowsArgumentException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var votingModel = new VotingModel()
            {
                Name = "Sample 1",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " +
                "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " +
                "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam.",
                MinimalForPercentage = 55m,
                MinimalAttendancePercentage = 10m,
                CompletionDate = DateTime.Now.AddDays(-30),
                VisibilityTerm = 5,
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Votings.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.AddAsync(votingModel),
                "Method does not throw an ArgumentException if the completion date is lower than current");
        }

        [Test]
        public async Task UpdateAsync_ValidVote_UpdatesElement()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var voting = context.Votings.FirstOrDefault(v => v.Name == "Voting 1");
            voting.Status = VotingStatus.NotConfirmed;
            context.Votings.Update(voting);
            await context.SaveChangesAsync();
            var votingModel = new VotingModel()
            {
                Id = context.Votings.FirstOrDefault(v => v.Name == "Voting 1").Id,
                Name = "Not the Voting 1",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " +
                "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " +
                "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam."
                + "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " +
                "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " +
                "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam." +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam."
                + "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " +
                "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " +
                "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam.",
                MinimalForPercentage = 55m,
                CreationDate = new DateTime(2021, 11, 14),
                CompletionDate = new DateTime(2021, 12, 14),
                MinimalAttendancePercentage = 10m,
                VisibilityTerm = 5,
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };

            // Act
            await service.UpdateAsync(_mapper.Map<VotingModel>(votingModel));

            // Assert
            var updatedVoting = context.Votings.FirstOrDefault(v => v.Id == votingModel.Id);
            Assert.AreEqual(votingModel.Name, updatedVoting.Name, "Elements' results are not equal");
            Assert.AreEqual(votingModel.Description, updatedVoting.Description, "Elements' descriptions are not equal");
            Assert.AreEqual(votingModel.MinimalForPercentage, updatedVoting.MinimalForPercentage,
                "Elements' minimal for percentages are not equal");
            Assert.AreEqual(votingModel.CompletionDate, updatedVoting.CompletionDate,
                "Elements' completion dates are not equal");
            Assert.AreEqual(votingModel.VisibilityTerm, updatedVoting.VisibilityTerm,
                "Elements' visibility terms are not equal");
            Assert.AreEqual(votingModel.AuthorId, updatedVoting.AuthorId,
                "Elements' author ids are not equal");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void UpdateAsync_NullOrEmptyName_ThrowsArgumentNullException(string name)
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var votingModel = new VotingModel()
            {
                Id = context.Votings.FirstOrDefault(v => v.Name == "Voting 1").Id,
                Name = name,
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " +
                "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " +
                "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam.",
                MinimalForPercentage = 55m,
                CompletionDate = new DateTime(2021, 12, 14),
                MinimalAttendancePercentage = 10m,
                CreationDate = new DateTime(2021, 11, 14),
                VisibilityTerm = 5,
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateAsync(votingModel),
                "Method does not throw an ArgumentNullException if voting's name is null");
        }

        [Test]
        public void UpdateAsync_NullDescription_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var votingModel = new VotingModel()
            {
                Id = context.Votings.FirstOrDefault(v => v.Name == "Voting 1").Id,
                Name = "Sample one",
                Description = null,
                MinimalForPercentage = 55m,
                CompletionDate = new DateTime(2021, 12, 14),
                CreationDate = new DateTime(2021, 11, 14),
                VisibilityTerm = 5,
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateAsync(votingModel),
                "Method does not throw an ArgumentNullException if voting's description is null");
        }

        [Test]
        public void UpdateAsync_NullModel_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            VotingModel votingModel = null;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateAsync(votingModel),
                "Method does not throw an ArgumentNullException if model is null");
        }

        [Test]
        public void UpdateAsync_ChangedAuthorId_ThrowsArgumentException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var votingModel = new VotingModel()
            {
                Id = context.Votings.FirstOrDefault(v => v.Name == "Voting 1").Id,
                Name = "Sample one",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " +
                "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " +
                "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam.",
                MinimalForPercentage = 55m,
                MinimalAttendancePercentage = 10m,
                CompletionDate = new DateTime(2021, 12, 14),
                CreationDate = new DateTime(2021, 11, 14),
                VisibilityTerm = 5,
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "pivo@gmail.com").Id
            };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateAsync(votingModel),
                "Method does not throw an ArgumentException if voting's author id is null");
        }

        [Test]
        public void UpdateAsync_TooLongName_ThrowsArgumentException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var votingModel = new VotingModel()
            {
                Id = context.Votings.FirstOrDefault(v => v.Name == "Voting 1").Id,
                Name = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Ut sed elementum magna. In commodo imperdiet egestas. Praesent quis purus elit. " +
                "Praesent auctor pretium quam, in placerat tellus imperdiet id. " +
                "Aenean semper sed turpis vitae eleifend porta ante.",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " +
                "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " +
                "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam.",
                MinimalForPercentage = 55m,
                MinimalAttendancePercentage = 10m,
                CreationDate = new DateTime(2021, 11, 14),
                CompletionDate = new DateTime(2021, 12, 14),
                VisibilityTerm = 5,
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateAsync(votingModel),
                "Method does not throw an ArgumentException if the name is too long");
        }

        [Test]
        public void UpdateAsync_TooShortDescription_ThrowsArgumentException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var votingModel = new VotingModel()
            {
                Id = context.Votings.FirstOrDefault(v => v.Name == "Voting 1").Id,
                Name = "Sample 1",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
                MinimalForPercentage = 55m,
                MinimalAttendancePercentage = 10m,
                CompletionDate = new DateTime(2021, 12, 14),
                VisibilityTerm = 5,
                CreationDate = new DateTime(2021, 11, 14),
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateAsync(votingModel),
                "Method does not throw an ArgumentException if the description is too short");
        }

        [Test]
        [TestCase(-1.1)]
        [TestCase(0)]
        [TestCase(49.9)]
        [TestCase(100.1)]
        public void UpdateAsync_InvalidMinForPercantage_ThrowsArgumentException(decimal percentage)
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var votingModel = new VotingModel()
            {
                Id = context.Votings.FirstOrDefault(v => v.Name == "Voting 1").Id,
                Name = "Sample 1",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " +
                "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " +
                "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam.",
                MinimalForPercentage = percentage,
                MinimalAttendancePercentage = 10m,
                CreationDate = new DateTime(2021, 11, 14),
                CompletionDate = new DateTime(2021, 12, 14),
                VisibilityTerm = 5,
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateAsync(votingModel),
                "Method does not throw an ArgumentException if the minimal for percantage is 0 or lower than 0");
        }

        [Test]
        [TestCase(-1.1)]
        [TestCase(0)]
        [TestCase(100.1)]
        public void UpdateAsync_InvalidMinForAttendance_ThrowsArgumentException(decimal percentage)
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var votingModel = new VotingModel()
            {
                Id = context.Votings.FirstOrDefault(v => v.Name == "Voting 1").Id,
                Name = "Sample 1",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " +
                "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " +
                "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam.",
                MinimalForPercentage = 55m,
                MinimalAttendancePercentage = percentage,
                CreationDate = new DateTime(2021, 11, 14),
                CompletionDate = new DateTime(2021, 12, 14),
                VisibilityTerm = 5,
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateAsync(votingModel),
                "Method does not throw an ArgumentException if the minimal for percantage is 0 or lower than 0");
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(32)]
        public void UpdateAsync_InvalidVisibilityTerm_ThrowsArgumentException(short term)
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var votingModel = new VotingModel()
            {
                Id = context.Votings.FirstOrDefault(v => v.Name == "Voting 1").Id,
                Name = "Sample 1",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " +
                "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " +
                "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam.",
                MinimalForPercentage = 55m,
                MinimalAttendancePercentage = 10,
                CompletionDate = new DateTime(2021, 12, 14),
                VisibilityTerm = term,
                CreationDate = new DateTime(2021, 11, 14),
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateAsync(votingModel),
                "Method does not throw an ArgumentException if the visibility term is 0 or lower than 0, or bigger than 30");
        }

        [Test]
        public void UpdateAsync_InvalidCompletionDate_ThrowsArgumentException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var votingModel = new VotingModel()
            {
                Id = context.Votings.FirstOrDefault(v => v.Name == "Voting 1").Id,
                Name = "Sample 1",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " +
                "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " +
                "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam.",
                MinimalForPercentage = 55m,
                MinimalAttendancePercentage = 10m,
                CompletionDate = new DateTime(2021, 11, 1),
                VisibilityTerm = 5,
                CreationDate = new DateTime(2021, 11, 14),
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateAsync(votingModel),
                "Method does not throw an ArgumentException if the completion date is lower than current");
        }

        [Test]
        public void UpdateAsync_ChangedCreationDate_ThrowsArgumentException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var votingModel = new VotingModel()
            {
                Id = context.Votings.FirstOrDefault(v => v.Name == "Voting 1").Id,
                Name = "Sample 1",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " +
                "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " +
                "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam.",
                MinimalForPercentage = 55m,
                MinimalAttendancePercentage = 10m,
                CreationDate = DateTime.Now,
                CompletionDate = DateTime.Now.AddDays(5),
                VisibilityTerm = 5,
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateAsync(votingModel),
                "Method does not throw an ArgumentException if the creation date is changed");
        }

        [Test]
        public async Task ChangeStatusAsync_ValidVote_ChangesStatus()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var votingModel = new VotingModel()
            {
                Id = context.Votings.FirstOrDefault(v => v.Name == "Voting 1").Id,
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " + "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " + "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " + "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " + "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam." + "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " +
                "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " +
                "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam."
                + "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " +
                "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " +
                "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam.",
                Name = "Voting 1",
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id,
                CreationDate = new DateTime(2021, 11, 14),
                CompletionDate = new DateTime(2021, 12, 15),
                MinimalAttendancePercentage = 10m,
                VisibilityTerm = 5,
                MinimalForPercentage = 55M,
                StatusSetterId = context.Users.FirstOrDefault(user => user.Email == "sydorenko@gmail.com").Id,
                Status = VotingStatus.Denied
            };

            // Act
            await service.ChangeStatusAsync(_mapper.Map<VotingModel>(votingModel));

            // Assert
            var updatedVoting = context.Votings.FirstOrDefault(v => v.Id == votingModel.Id);
            Assert.AreEqual(votingModel.StatusSetterId, updatedVoting.StatusSetter.Id,
                "Elements' status setter's ids are not equal");
            Assert.AreEqual(votingModel.Status, updatedVoting.Status,
                "Elements' statuses are not equal");
        }

        [Test]
        public void ChangeStatusAsync_NullModel_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            VotingModel votingModel = null;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.ChangeStatusAsync(votingModel),
                "Method does not throw an ArgumentNullException if model is null");
        }

        [Test]
        public void ChangeStatusAsync_NullStatusSetter_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var votingModel = new VotingModel()
            {
                Id = context.Votings.FirstOrDefault(v => v.Name == "Voting 1").Id,
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " +
                "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " +
                "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam.",
                Name = "Voting 1",
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id,
                CreationDate = new DateTime(2021, 11, 14),
                CompletionDate = new DateTime(2021, 12, 15),
                VisibilityTerm = 5,
                MinimalForPercentage = 55M,
                MinimalAttendancePercentage = 10m,
                StatusSetterId = null,
                Status = VotingStatus.Denied
            };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.ChangeStatusAsync(votingModel),
                "Method does not throw an ArgumentNullException if status setter is null");
        }

        [Test]
        public void ChangeStatusAsync_NotFoundStatusSetter_ThrowsInvalidOperationException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var votingModel = new VotingModel()
            {
                Id = context.Votings.FirstOrDefault(v => v.Name == "Voting 1").Id,
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " +
                "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " +
                "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam.",
                Name = "Voting 1",
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id,
                CreationDate = new DateTime(2021, 11, 14),
                CompletionDate = new DateTime(2021, 12, 15),
                VisibilityTerm = 5,
                MinimalForPercentage = 55M,
                MinimalAttendancePercentage = 10m,
                StatusSetterId = "NOT FOUND",
                Status = VotingStatus.Denied
            };

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await service.ChangeStatusAsync(votingModel),
                "Method does not throw an InvalidOperationException if status setter is not found");
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task DeleteByIdAsync_ValidId_DeletesElement(int id)
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var voting = await context.Votings.FindAsync(id);
            voting.Status = VotingStatus.NotConfirmed;
            context.Votings.Update(voting);
            await context.SaveChangesAsync();
            var service = new VotingService(context, _mapper);
            var expectedCount = context.Votings.Count() - 1;

            // Act
            await service.DeleteByIdAsync(id);
            var actual = context.Votings;

            // Assert
            Assert.AreEqual(expectedCount, actual.Count(), "The element was not deleted");
            Assert.AreEqual(await actual.FindAsync(id), null, "Wrong element was deleted");
        }

        [Test]
        public void DeleteByIdAsync_NotValidId_ThrowsInvalidOperationException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var id = -1;

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await service.DeleteByIdAsync(id),
                "Method does not throw an InvalidOperationException if status setter was not found");
        }

        private static void AddDataForSpecificTests(ApplicationContext context)
        {
            context.Votings.Add(new Voting() { Name = "Voting 5", Author = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), StatusSetter = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), CreationDate = new DateTime(2022, 1, 14), CompletionDate = new DateTime(2022, 12, 15), VisibilityTerm = 5, MinimalForPercentage = 55M, MinimalAttendancePercentage = 10.5m, Status = VotingStatus.Confirmed });
            context.SaveChanges();
            context.Votings.Add(new Voting() { Name = "Voting 6", Author = context.Users.FirstOrDefault(user => user.Email == "sydorenko@gmail.com"), StatusSetter = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), CreationDate = new DateTime(2022, 1, 14), CompletionDate = new DateTime(2022, 12, 15), VisibilityTerm = 5, MinimalForPercentage = 55M, MinimalAttendancePercentage = 10.5m, Status = VotingStatus.Confirmed, Faculty = context.Faculties.FirstOrDefault(f => f.Name == "ФІОТ") });
            context.SaveChanges();
            context.Votings.Add(new Voting() { Name = "Voting 7", Author = context.Users.FirstOrDefault(user => user.Email == "sydorenko@gmail.com"), StatusSetter = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), CreationDate = new DateTime(2022, 1, 14), CompletionDate = new DateTime(2022, 12, 15), VisibilityTerm = 5, MinimalForPercentage = 55M, MinimalAttendancePercentage = 10.5m, Status = VotingStatus.Confirmed, Flow = context.Flows.FirstOrDefault(f => f.Name == "ІС-0") });
            context.SaveChanges();
            context.Votings.Add(new Voting() { Name = "Voting 8", Author = context.Users.FirstOrDefault(user => user.Email == "sydorenko@gmail.com"), StatusSetter = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), CreationDate = new DateTime(2022, 1, 14), CompletionDate = new DateTime(2022, 12, 15), VisibilityTerm = 5, MinimalForPercentage = 55M, MinimalAttendancePercentage = 10.5m, Status = VotingStatus.Confirmed, Group = context.Groups.FirstOrDefault(g => g.Number == 2 && g.Flow.Name == "ІС-0") });
            context.SaveChanges();
            context.Votings.Add(new Voting() { Name = "Voting 9", Author = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), StatusSetter = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), CreationDate = new DateTime(2020, 11, 14), CompletionDate = new DateTime(2020, 12, 15), VisibilityTerm = 5, MinimalForPercentage = 55M, MinimalAttendancePercentage = 10.5m, Status = VotingStatus.Confirmed });
            context.SaveChanges();
            context.Votings.Add(new Voting() { Name = "Voting 10", Author = context.Users.FirstOrDefault(user => user.Email == "sydorenko@gmail.com"), StatusSetter = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), CreationDate = new DateTime(2020, 11, 14), CompletionDate = new DateTime(2020, 12, 15), VisibilityTerm = 5, MinimalForPercentage = 55M, MinimalAttendancePercentage = 10.5m, Status = VotingStatus.Confirmed, Faculty = context.Faculties.FirstOrDefault(f => f.Name == "ФІОТ") });
            context.SaveChanges();
            context.Votings.Add(new Voting() { Name = "Voting 11", Author = context.Users.FirstOrDefault(user => user.Email == "sydorenko@gmail.com"), StatusSetter = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), CreationDate = new DateTime(2020, 11, 14), CompletionDate = new DateTime(2020, 12, 15), VisibilityTerm = 5, MinimalForPercentage = 55M, MinimalAttendancePercentage = 10.5m, Status = VotingStatus.Confirmed, Flow = context.Flows.FirstOrDefault(f => f.Name == "ІС-0") });
            context.SaveChanges();
            context.Votings.Add(new Voting() { Name = "Voting 12", Author = context.Users.FirstOrDefault(user => user.Email == "sydorenko@gmail.com"), StatusSetter = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), CreationDate = new DateTime(2020, 11, 14), CompletionDate = new DateTime(2020, 12, 15), VisibilityTerm = 5, MinimalForPercentage = 55M, MinimalAttendancePercentage = 10.5m, Status = VotingStatus.Confirmed, Group = context.Groups.FirstOrDefault(g => g.Number == 2 && g.Flow.Name == "ІС-0") });
            context.SaveChanges();
            context.Votings.Add(new Voting() { Name = "Voting 13", Author = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), StatusSetter = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), CreationDate = new DateTime(2021, 11, 14), CompletionDate = new DateTime(2021, 12, 15), VisibilityTerm = 5, MinimalForPercentage = 55M, MinimalAttendancePercentage = 10.5m, Status = VotingStatus.Denied });
            context.SaveChanges();
            context.Votings.Add(new Voting() { Name = "Voting 14", Author = context.Users.FirstOrDefault(user => user.Email == "sydorenko@gmail.com"), StatusSetter = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), CreationDate = new DateTime(2021, 11, 14), CompletionDate = new DateTime(2021, 12, 15), VisibilityTerm = 5, MinimalForPercentage = 55M, MinimalAttendancePercentage = 10.5m, Status = VotingStatus.Denied, Flow = context.Flows.FirstOrDefault(f => f.Name == "ІС-0") });
            context.SaveChanges();
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void GetFilteredAndSortedForUserAsync_NullOrEmptyUserId_ThrowsArgumentNullException(string userId)
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.GetFilteredAndSortedForUserAsync(userId),
                "Method does not throw an ArgumentNullException if user id is null or empty");
        }

        [Test]
        public void GetFilteredAndSortedForUserAsync_InvalidUserId_ThrowsInvalidOperationException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var userId = "invalid";

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await service.GetFilteredAndSortedForUserAsync(userId),
                "Method does not throw an InvalidOperationException if user id is invalid");
        }

        [Test]
        public async Task GetFilteredAndSortedForAdminAsync_ReturnsRightData()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            AddDataForSpecificTests(context);
            var service = new VotingService(context, _mapper);
            var expectedNames = new string[]
            {
                "Voting 5",
                "Voting 6",
                "Voting 7",
                "Voting 8",
                "Voting 1",
                "Voting 2",
                "Voting 3",
                "Voting 4",
                "Voting 13",
                "Voting 14",
                "Voting 9",
                "Voting 10",
                "Voting 11",
                "Voting 12"
            };

            // Act
            var actualNames = (await service.GetFilteredAndSortedForAdminAsync()).Select(v => v.Name).ToArray();

            // Assert
            Assert.AreEqual(expectedNames.Length, actualNames.Length, "Arrays do not have the same Length");
            for (int i = 0; i < expectedNames.Length; i++)
            {
                Assert.AreEqual(expectedNames[i], actualNames[i], "Votings are sorted in the wrong order");
            }
        }

        [Test]
        public async Task GetNotConfirmedAsync_ReturnsRightData()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            await context.Votings.AddAsync(new Voting() { Name = "Voting 5", Author = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), StatusSetter = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), CreationDate = new DateTime(2021, 2, 14), CompletionDate = new DateTime(2021, 12, 15), VisibilityTerm = 5, MinimalForPercentage = 55M, MinimalAttendancePercentage = 10.5m, Status = VotingStatus.NotConfirmed });
            await context.Votings.AddAsync(new Voting() { Name = "Voting 6", Author = context.Users.FirstOrDefault(user => user.Email == "sydorenko@gmail.com"), StatusSetter = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), CreationDate = new DateTime(2022, 1, 14), CompletionDate = new DateTime(2022, 7, 15), VisibilityTerm = 5, MinimalForPercentage = 55M, MinimalAttendancePercentage = 10.5m, Status = VotingStatus.NotConfirmed, Faculty = context.Faculties.FirstOrDefault(f => f.Name == "ФІОТ") });
            await context.Votings.AddAsync(new Voting() { Name = "Voting 7", Author = context.Users.FirstOrDefault(user => user.Email == "sydorenko@gmail.com"), StatusSetter = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), CreationDate = new DateTime(2020, 11, 14), CompletionDate = new DateTime(2020, 12, 15), VisibilityTerm = 5, MinimalForPercentage = 55M, MinimalAttendancePercentage = 10.5m, Status = VotingStatus.NotConfirmed, Flow = context.Flows.FirstOrDefault(f => f.Name == "ІС-0") });
            await context.Votings.AddAsync(new Voting() { Name = "Voting 8", Author = context.Users.FirstOrDefault(user => user.Email == "sydorenko@gmail.com"), StatusSetter = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), CreationDate = new DateTime(2022, 1, 14), CompletionDate = new DateTime(2022, 12, 15), VisibilityTerm = 5, MinimalForPercentage = 55M, MinimalAttendancePercentage = 10.5m, Status = VotingStatus.Confirmed, Group = context.Groups.FirstOrDefault(g => g.Number == 2 && g.Flow.Name == "ІС-0") });
            await context.Votings.AddAsync(new Voting() { Name = "Voting 9", Author = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), StatusSetter = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com"), CreationDate = new DateTime(2020, 11, 14), CompletionDate = new DateTime(2020, 12, 15), VisibilityTerm = 5, MinimalForPercentage = 55M, MinimalAttendancePercentage = 10.5m, Status = VotingStatus.Denied });
            await context.SaveChangesAsync();
            var service = new VotingService(context, _mapper);
            var expectedNames = new string[]
            {
                "Voting 7",
                "Voting 5",
                "Voting 6"
            };

            // Act
            var actualNames = (await service.GetNotConfirmedAsync()).Select(v => v.Name).ToArray();

            // Assert
            Assert.AreEqual(expectedNames.Length, actualNames.Length, "Arrays do not have the same Length");
            for (int i = 0; i < expectedNames.Length; i++)
            {
                Assert.AreEqual(expectedNames[i], actualNames[i], "Votings are sorted in the wrong order");
            }
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void GetUserVotingsAsync_NullOrEmptyUserId_ThrowsArgumentNullException(string userId)
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.GetFilteredAndSortedForUserAsync(userId),
                "Method does not throw an ArgumentNullException if user id is null or empty");
        }

        [Test]
        public void GetUserVotingsAsync_InvalidUserId_ThrowsInvalidOperationException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var userId = "invalid";

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await service.GetFilteredAndSortedForUserAsync(userId),
                "Method does not throw an InvalidOperationException if user id is invalid");
        }

        [Test]
        public async Task GetUserVotingsAsync_ValidUserId_ReturnsRightData()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            AddDataForSpecificTests(context);
            var service = new VotingService(context, _mapper);
            var user = context.Users.FirstOrDefault(u => u.Email == "petrenko1@gmail.com");
            var expectedNames = new string[]
            {
                "Voting 1",
                "Voting 2"
            };

            // Act
            var actualNames = (await service.GetUserVotingsAsync(user.Id)).Select(v => v.Name).ToArray();

            // Assert
            Assert.AreEqual(expectedNames.Length, actualNames.Length, "Arrays do not have the same Length");
            for (int i = 0; i < expectedNames.Length; i++)
            {
                Assert.AreEqual(expectedNames[i], actualNames[i], "Votings are sorted in the wrong order");
            }
        }

        [Test]
        public async Task GetActualForPercentageAsync_ValidVoting_ReturnsRightInfo()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var voting = context.Votings.Find(1);
            await context.Votes.AddAsync(new Vote() { Result = VoteResult.For, Voting = voting, User = context.Users.FirstOrDefault(user => user.Email == "pivo@gmail.com") });
            await context.SaveChangesAsync();
            var expected = 2.0m / 3.0m;

            // Act
            var actual = await service.GetActualForPercentageAsync(_mapper.Map<VotingModel>(voting));

            // Assert
            Assert.That(actual, Is.EqualTo(expected).Within(floatingTolerance), "The information is not right");
        }

        [Test]
        public async Task GetActualForPercentageAsync_ZeroVotersInVoting_ReturnsRightInfo()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var voting = await context.Votings.FindAsync(4);
            var expected = 0m;

            // Act
            var actual = await service.GetActualForPercentageAsync(_mapper.Map<VotingModel>(voting));

            // Assert
            Assert.That(actual, Is.EqualTo(expected).Within(floatingTolerance), "The information is not right");
        }

        [Test]
        public async Task GetActualAttendancePercentageAsync_KPILevel_ReturnsRightInfo()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var voting = await context.Votings.FindAsync(1);
            var ban1 = await context.Bans.FindAsync(1);
            ban1.DateFrom = DateTime.MinValue;
            ban1.DateTo = DateTime.MaxValue;
            context.Bans.Update(ban1);
            var ban2 = await context.Bans.FindAsync(2);
            ban2.DateFrom = DateTime.MinValue;
            ban2.DateTo = DateTime.MinValue;
            context.Bans.Update(ban2);
            await context.SaveChangesAsync();
            var expected = 2.0m / 6.0m;

            // Act
            var actual = await service.GetActualAttendancePercentageAsync(_mapper.Map<VotingModel>(voting));

            // Assert
            Assert.That(actual, Is.EqualTo(expected).Within(floatingTolerance), "The information is not right");
        }

        [Test]
        public async Task GetActualAttendancePercentageAsync_FacultyLevel_ReturnsRightInfo()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            await context.AddAsync(new User() { FirstName = "Petro", LastName = "Petrenko", Patronymic = "Petrovich", Email = "petrenko1@gmail.com", TelegramTag = "@petrenko", PasswordHash = "sampleDadcassacsasdefes", PasswordChanged = false, RegistrationDate = new DateTime(2021, 11, 14), Group = context.Groups.Find(1) });
            await context.SaveChangesAsync();
            var service = new VotingService(context, _mapper);
            var voting = await context.Votings.FindAsync(2);
            var ban1 = await context.Bans.FindAsync(1);
            ban1.DateFrom = DateTime.MinValue;
            ban1.DateTo = DateTime.MaxValue;
            context.Bans.Update(ban1);
            var ban2 = await context.Bans.FindAsync(2);
            ban2.DateFrom = DateTime.MinValue;
            ban2.DateTo = DateTime.MinValue;
            context.Bans.Update(ban2);
            await context.SaveChangesAsync();
            var expected = 1.0m / 7.0m;

            // Act
            var actual = await service.GetActualAttendancePercentageAsync(_mapper.Map<VotingModel>(voting));

            // Assert
            Assert.That(actual, Is.EqualTo(expected).Within(floatingTolerance), "The information is not right");
        }

        [Test]
        public async Task GetActualAttendancePercentageAsync_FlowLevel_ReturnsRightInfo()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            await context.AddAsync(new User() { FirstName = "Petro", LastName = "Petrenko", Patronymic = "Petrovich", Email = "petrenko78677@gmail.com", TelegramTag = "@petreddnko", PasswordHash = "sampleDadcassacsasdefes", PasswordChanged = false, RegistrationDate = new DateTime(2021, 11, 14), Group = context.Groups.Find(6) });
            await context.SaveChangesAsync();
            await context.AddAsync(new User() { FirstName = "Max", LastName = "Petrenko", Patronymic = "Petrovich", Email = "petrenko423234432@gmail.com", TelegramTag = "@petrffasenko", PasswordHash = "sampleDadcassacsasdefes", PasswordChanged = false, RegistrationDate = new DateTime(2021, 11, 14), Group = context.Groups.Find(6) });
            await context.SaveChangesAsync();
            var votingId = 3;
            await context.Votes.AddAsync(new Vote() { Result = VoteResult.For, VotingId = votingId,  UserId = context.Users.Where(u => u.Email == "petrenko78677@gmail.com").FirstOrDefault().Id });
            await context.SaveChangesAsync();
            var service = new VotingService(context, _mapper);
            var voting = await context.Votings.FindAsync(votingId);
            var ban1 = await context.Bans.FindAsync(1);
            ban1.DateFrom = DateTime.MinValue;
            ban1.DateTo = DateTime.MaxValue;
            context.Bans.Update(ban1);
            var ban2 = await context.Bans.FindAsync(2);
            ban2.DateFrom = DateTime.MinValue;
            ban2.DateTo = DateTime.MinValue;
            context.Bans.Update(ban2);
            await context.SaveChangesAsync();
            var expected = 1.0m / 8.0m;

            // Act
            var actual = await service.GetActualAttendancePercentageAsync(_mapper.Map<VotingModel>(voting));

            // Assert
            Assert.That(actual, Is.EqualTo(expected).Within(floatingTolerance), "The information is not right");
        }

        [Test]
        public async Task GetActualAttendancePercentageAsync_GroupLevel_ReturnsRightInfo()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var votingId = 4;
            await context.Votes.AddAsync(new Vote() { Result = VoteResult.For, VotingId = votingId, UserId = context.Users.Where(u => u.Email == "petrenko1@gmail.com").FirstOrDefault().Id });
            await context.SaveChangesAsync();
            var service = new VotingService(context, _mapper);
            var voting = await context.Votings.FindAsync(votingId);
            var ban1 = await context.Bans.FindAsync(1);
            ban1.DateFrom = DateTime.MinValue;
            ban1.DateTo = DateTime.MaxValue;
            context.Bans.Update(ban1);
            var ban2 = await context.Bans.FindAsync(2);
            ban2.DateFrom = DateTime.MinValue;
            ban2.DateTo = DateTime.MinValue;
            context.Bans.Update(ban2);
            await context.SaveChangesAsync();
            var expected = 1.0m / 6.0m;

            // Act
            var actual = await service.GetActualAttendancePercentageAsync(_mapper.Map<VotingModel>(voting));

            // Assert
            Assert.That(actual, Is.EqualTo(expected).Within(floatingTolerance), "The information is not right");
        }

        [Test]
        public async Task IsVotingSuccessfulAsync_ValidVotingSmallAttendance_ReturnsFalse()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var votingId = 4;
            await context.Votes.AddAsync(new Vote() { Result = VoteResult.For, VotingId = votingId, UserId = context.Users.Where(u => u.Email == "petrenko1@gmail.com").FirstOrDefault().Id });
            await context.SaveChangesAsync();
            var service = new VotingService(context, _mapper);
            var voting = await context.Votings.FindAsync(votingId);
            voting.MinimalAttendancePercentage = 50m;
            voting.CompletionDate = DateTime.Now.AddDays(-1);
            context.Votings.Update(voting);
            await context.SaveChangesAsync();
            var expected = false;

            // Act
            var actual = await service.IsVotingSuccessfulAsync(_mapper.Map<VotingModel>(voting));

            // Assert
            Assert.AreEqual(expected, actual, "Method did not return right data");
        }

        [Test]
        public async Task IsVotingSuccessfulAsync_ValidVotingSmallForPercentage_ReturnsFalse()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var votingId = 4;
            await context.Votes.AddAsync(new Vote() { Result = VoteResult.For, VotingId = votingId, UserId = context.Users.Where(u => u.Email == "petrenko1@gmail.com").FirstOrDefault().Id });
            await context.Votes.AddAsync(new Vote() { Result = VoteResult.Against, VotingId = votingId, UserId = context.Users.Where(u => u.Email == "sydorenko@gmail.com").FirstOrDefault().Id });
            await context.Votes.AddAsync(new Vote() { Result = VoteResult.Neutral, VotingId = votingId, UserId = context.Users.Where(u => u.Email == "pivo@gmail.com").FirstOrDefault().Id });
            await context.SaveChangesAsync();
            var service = new VotingService(context, _mapper);
            var voting = await context.Votings.FindAsync(votingId);
            voting.CompletionDate = DateTime.Now.AddDays(-1);
            context.Votings.Update(voting);
            await context.SaveChangesAsync();
            var expected = false;

            // Act
            var actual = await service.IsVotingSuccessfulAsync(_mapper.Map<VotingModel>(voting));

            // Assert
            Assert.AreEqual(expected, actual, "Method did not return right data");
        }

        [Test]
        public async Task IsVotingSuccessfulAsync_ValidVoting_ReturnsTrue()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var votingId = 4;
            await context.Votes.AddAsync(new Vote() { Result = VoteResult.For, VotingId = votingId, UserId = context.Users.Where(u => u.Email == "petrenko1@gmail.com").FirstOrDefault().Id });
            await context.Votes.AddAsync(new Vote() { Result = VoteResult.For, VotingId = votingId, UserId = context.Users.Where(u => u.Email == "sydorenko@gmail.com").FirstOrDefault().Id });
            await context.Votes.AddAsync(new Vote() { Result = VoteResult.Neutral, VotingId = votingId, UserId = context.Users.Where(u => u.Email == "pivo@gmail.com").FirstOrDefault().Id });
            await context.SaveChangesAsync();
            var service = new VotingService(context, _mapper);
            var voting = await context.Votings.FindAsync(votingId);
            voting.CompletionDate = DateTime.Now.AddDays(-1);
            context.Votings.Update(voting);
            await context.SaveChangesAsync();
            var expected = true;

            // Act
            var actual = await service.IsVotingSuccessfulAsync(_mapper.Map<VotingModel>(voting));

            // Assert
            Assert.AreEqual(expected, actual, "Method did not return right data");
        }

        [Test]
        public void IsVotingSuccessfulAsync_NotCompletedVoting_ThrowsArgumentException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var service = new VotingService(context, _mapper);
            var votingId = 4;
            var voting = context.Votings.Find(votingId);
            voting.CompletionDate = DateTime.Now.AddDays(1);
            context.Votings.Update(voting);
            context.SaveChanges();

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.IsVotingSuccessfulAsync(_mapper.Map<VotingModel>(voting)),
                "Method does not throw an ArgumentException if the voting is not completed yet");
        }
    }
}
