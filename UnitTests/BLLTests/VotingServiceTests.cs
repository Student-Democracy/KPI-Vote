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
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam.",
                MinimalForPercentage = 5.5m,
                CompletionDate = DateTime.Now.AddDays(30),
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
                MinimalForPercentage = 5.5m,
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
                MinimalForPercentage = 5.5m,
                CompletionDate = DateTime.Now.AddDays(30),
                VisibilityTerm = 5,
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
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam.",
                MinimalForPercentage = 5.5m,
                CompletionDate = DateTime.Now.AddDays(30),
                VisibilityTerm = 5,
                AuthorId = null
            };
            var expectedCount = context.Votings.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.AddAsync(votingModel),
                "Method does not throw an ArgumentNullException if voting's author id is null");
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
                MinimalForPercentage = 5.5m,
                CompletionDate = DateTime.Now.AddDays(30),
                VisibilityTerm = 5,
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
                MinimalForPercentage = 5.5m,
                CompletionDate = DateTime.Now.AddDays(30),
                VisibilityTerm = 5,
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
                MinimalForPercentage = 5.5m,
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
                MinimalForPercentage = 5.5m,
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
            var votingModel = new VotingModel()
            {
                Id = context.Votings.FirstOrDefault(v => v.Name == "Voting 1").Id,
                Name = "Voting 1",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Integer vel sem quis tortor pretium placerat. Pellentesque habitant morbi " +
                "tristique senectus et netus et malesuada fames ac turpis egestas. In semper porta iaculis. " +
                "Cras accumsan, eros ut imperdiet finibus, elit mauris aliquam risus, in vehicula diam urna quis metus. " +
                "Nullam dignissim, leo eu pretium viverra, risus elit bibendum nisi, ac nam.",
                MinimalForPercentage = 5.5m,
                CreationDate = new DateTime(2021, 11, 14),
                CompletionDate = new DateTime(2021, 12, 14),
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
                MinimalForPercentage = 5.5m,
                CompletionDate = new DateTime(2021, 12, 14),
                CreationDate = new DateTime(2021, 11, 14),
                VisibilityTerm = 5,
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Votings.Count() + 1;

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
                MinimalForPercentage = 5.5m,
                CompletionDate = new DateTime(2021, 12, 14),
                CreationDate = new DateTime(2021, 11, 14),
                VisibilityTerm = 5,
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Votings.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateAsync(votingModel),
                "Method does not throw an ArgumentNullException if voting's description is null");
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
                MinimalForPercentage = 5.5m,
                CompletionDate = new DateTime(2021, 12, 14),
                CreationDate = new DateTime(2021, 11, 14),
                VisibilityTerm = 5,
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "pivo@gmail.com").Id
            };
            var expectedCount = context.Votings.Count() + 1;

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
                MinimalForPercentage = 5.5m,
                CreationDate = new DateTime(2021, 11, 14),
                CompletionDate = new DateTime(2021, 12, 14),
                VisibilityTerm = 5,
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Votings.Count() + 1;

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
                MinimalForPercentage = 5.5m,
                CompletionDate = new DateTime(2021, 12, 14),
                VisibilityTerm = 5,
                CreationDate = new DateTime(2021, 11, 14),
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Votings.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateAsync(votingModel),
                "Method does not throw an ArgumentException if the description is too short");
        }

        [Test]
        [TestCase(-1.1)]
        [TestCase(0)]
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
                CreationDate = new DateTime(2021, 11, 14),
                CompletionDate = new DateTime(2021, 12, 14),
                VisibilityTerm = 5,
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Votings.Count() + 1;

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
                MinimalForPercentage = 5.5m,
                CompletionDate = new DateTime(2021, 12, 14),
                VisibilityTerm = term,
                CreationDate = new DateTime(2021, 11, 14),
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Votings.Count() + 1;

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
                MinimalForPercentage = 5.5m,
                CompletionDate = new DateTime(2021, 11, 1),
                VisibilityTerm = 5,
                CreationDate = new DateTime(2021, 11, 14),
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Votings.Count() + 1;

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
                MinimalForPercentage = 5.5m,
                CreationDate = DateTime.Now,
                CompletionDate = DateTime.Now.AddDays(5),
                VisibilityTerm = 5,
                AuthorId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id
            };
            var expectedCount = context.Votings.Count() + 1;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateAsync(votingModel),
                "Method does not throw an ArgumentException if the creation date is changed");
        }
    }
}
