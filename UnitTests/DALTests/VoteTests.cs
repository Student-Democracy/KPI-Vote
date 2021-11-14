using DAL;
using DAL.Entities;
using NUnit.Framework;
using System;
using System.Linq;

namespace UnitTests.DALTests
{
    public class VoteTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Add_TheSameVotingAndUser_ThrowsException()
        {
            using (var context = new ApplicationContext(UnitTestHelper.GetUnitTestDbOptions()))
            {
                // Arrange
                var result = VoteResult.Against;
                var voting = context.Votings.FirstOrDefault(v => v.Name == "Voting 1");
                var user = context.Users.FirstOrDefault(user => user.Email == "petrenko1@gmail.com");

                // Act & Assert
                Assert.Catch<Exception>(() => 
                {
                    context.Votes.Add(new Vote() { Result = result, Voting = voting, User = user });
                    context.SaveChanges();
                }
                );
            }
        }
    }
}