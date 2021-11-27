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
    public class VoteServiceTests
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
            var expected = context.Votes.ToArray();
            var service = new VoteService(context, _mapper);

            // Act
            var actual = service.GetAll().ToArray();

            // Assert
            Assert.AreEqual(expected.Length, actual.Length, "The count of elements received is not right");
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i].Result, actual[i].Result, "Elements' results are not equal");
                Assert.AreEqual(expected[i].UserId, actual[i].UserId, "Elements' user ids are not equal");
                Assert.AreEqual(expected[i].VotingId, actual[i].VotingId, "Elements' voting ids are not equal");
            }
        }

        [Test]
        public async Task GetByIdAsync_ReturnsRightElement()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var votingId = context.Votings.FirstOrDefault(v => v.Name == "Voting 1").Id;
            var userId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id;
            var expected = await context.Votes.SingleOrDefaultAsync(v => v.UserId == userId && v.VotingId == votingId);
            var service = new VoteService(context, _mapper);

            // Act
            var actual = await service.GetByIdAsync(userId, votingId);

            // Assert
            Assert.AreEqual(expected.Result, actual.Result, "Elements' results are not equal");
            Assert.AreEqual(expected.UserId, actual.UserId, "Elements' user ids are not equal");
            Assert.AreEqual(expected.VotingId, actual.VotingId, "Elements' voting ids are not equal");
        }

        [Test]
        public async Task AddAsync_ValidVote_AddsElement()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var votingId = context.Votings.FirstOrDefault(v => v.Name == "Voting 2").Id;
            var userId = context.Users.FirstOrDefault(user => user.Email == "sydorenko@gmail.com").Id;
            var result = VoteResult.Neutral;
            var service = new VoteService(context, _mapper);
            var voteModel = new VoteModel() { Result = result, UserId = userId, VotingId = votingId };
            var expectedCount = context.Votes.Count() + 1;

            // Act
            await service.AddAsync(voteModel);

            // Assert
            Assert.AreEqual(expectedCount, context.Votes.Count(), "The element was not added");
            var addedVote = context.Votes.Last();
            Assert.AreEqual(voteModel.Result, addedVote.Result, "Elements' results are not equal");
            Assert.AreEqual(voteModel.UserId, addedVote.UserId, "Elements' user ids are not equal");
            Assert.AreEqual(voteModel.VotingId, addedVote.VotingId, "Elements' voting ids are not equal");
        }

        [Test]
        public void AddAsync_InvalidVotingId_ThrowsArgumentException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var votingId = -1;
            var userId = context.Users.FirstOrDefault(user => user.Email == "sydorenko@gmail.com").Id;
            var result = VoteResult.Neutral;
            var service = new VoteService(context, _mapper);
            var voteModel = new VoteModel() { Result = result, UserId = userId, VotingId = votingId };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.AddAsync(voteModel), 
                "Method does not throw an ArgumentException if voting id is invalid");
        }

        [Test]
        public void AddAsync_InvalidUserId_ThrowsArgumentException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var votingId = context.Votings.FirstOrDefault(v => v.Name == "Voting 2").Id;
            var userId = "INVALID";
            var result = VoteResult.Neutral;
            var service = new VoteService(context, _mapper);
            var voteModel = new VoteModel() { Result = result, UserId = userId, VotingId = votingId };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.AddAsync(voteModel),
                "Method does not throw an ArgumentException if user id is invalid");
        }

        [Test]
        public void AddAsync_NullUserId_ThrowsArgumentNullException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var votingId = context.Votings.FirstOrDefault(v => v.Name == "Voting 2").Id;
            string userId = null;
            var result = VoteResult.Neutral;
            var service = new VoteService(context, _mapper);
            var voteModel = new VoteModel() { Result = result, UserId = userId, VotingId = votingId };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.AddAsync(voteModel),
                "Method does not throw an ArgumentException if user id is null");
        }

        [Test]
        public void AddAsync_ExistingVote_ThrowsArgumentException()
        {
            // Arrange
            using var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions());
            var votingId = context.Votings.FirstOrDefault(v => v.Name == "Voting 2").Id;
            var userId = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com").Id;
            var result = VoteResult.For;
            var service = new VoteService(context, _mapper);
            var voteModel = new VoteModel() { Result = result, UserId = userId, VotingId = votingId };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await service.AddAsync(voteModel),
                "Method does not throw an ArgumentException if vote is already existing");
        }
    }
}
