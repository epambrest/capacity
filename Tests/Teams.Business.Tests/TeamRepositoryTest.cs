using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Teams.Data;
using Teams.Data.Models;
using Teams.Data.Repository;

namespace Teams.Business.Tests
{
    [TestFixture]
    class TeamRepositoryTest
    {
        private ApplicationDbContext context;
        private IRepository<Team, int> teamRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TeamListDatabase")
            .Options;

            context = new ApplicationDbContext(options);

        }

        private IQueryable<Team> GenerateData(int num)
        {
            var lst = new List<Team>();
            for (int i = 1; i < num; i++)
            {
                lst.Add(new Team
                {
                    Id = i,
                    TeamName = "Name" + i,
                    TeamOwner = "Owner" + i
                });
            }
            return lst.AsQueryable();
        }

        #region GetAll
        [Test]
        public void GetAll_TeamRepositoryReturnsListCount100_ListCount100()
        {
            //Arrange
            context.Team.AddRange(GenerateData(100));
            context.SaveChanges();
            teamRepository = new TeamRepository(context);

            //Act
            var teams = teamRepository.GetAll();

            //Assert
            Assert.AreEqual(context.Team.Count(), teams.Count());

        }

        [Test]
        public void GetAll_TeamRepositoryReturnsEmptyList_ReturnsEmpty()
        {
            //Arrange
            teamRepository = new TeamRepository(context);

            //Act
            var teams = teamRepository.GetAll();

            //Assert
            Assert.IsEmpty(teams);

        }
        #endregion

        #region GetByIdAsync
        [Test]
        public void GetByIdAsync_TeamRepositoryReturnsTeamId1_ReturnsTeamId1()
        {
            //Arrange
            if (context.Team.Count() < 1)
                context.Team.AddRange(GenerateData(100));
            teamRepository = new TeamRepository(context);

            //Act
            var team = teamRepository.GetByIdAsync(1).Result;

            //Assert
            Assert.AreEqual(1, team.Id);

        }

        [Test]
        public void GetByIdAsync_TeamRepositoryReturnsNull_ReturnsNull()
        {
            //Arrange
            teamRepository = new TeamRepository(context);

            //Act
            var team = teamRepository.GetByIdAsync(101).Result;

            //Assert
            Assert.IsNull(team);

        }
        #endregion

        #region InsertAsync
        [Test]
        public void InsertAsync_TeamRepositoryReturnsTrue_ReturnsTrue()
        {
            //Arrange
            teamRepository = new TeamRepository(context);
            Team team = new Team() { TeamName = "Name101", TeamOwner = "Owner101" };

            //Act
            var result = teamRepository.InsertAsync(team).Result;

            //Assert
            Assert.IsTrue(result);

        }
        #endregion

        #region DeleteAsync
        [Test]
        public void DeleteAsync_TeamRepositoryReturnsTrue_ReturnsTrue()
        {
            //Arrange
            context.Team.AddRange(GenerateData(2));
            context.SaveChanges();
            teamRepository = new TeamRepository(context);

            //Act
            var result = teamRepository.DeleteAsync(teamRepository.GetByIdAsync(1).Result).Result;

            //Assert
            Assert.IsTrue(result);

        }

        [Test]
        public void DeleteAsync_TeamRepositoryReturnsNull_ReturnsNull()
        {
            //Arrange
            teamRepository = new TeamRepository(context);

            //Act&Assert
            try
            {
                var result = teamRepository.DeleteAsync(null);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Value cannot be null.", e.Message, string.Format("result != expected"));
            }

        }
        #endregion

        #region UpdateAsync
        [Test]
        public void UpdateAsync_TeamRepositoryReturnsTrue_ReturnsTrue()
        {
            //Arrange
            if (context.Team.Count() < 1)
            {
                context.Team.AddRange(GenerateData(100));
                context.SaveChanges();
            }
            teamRepository = new TeamRepository(context);

            //Act
            var result = teamRepository.UpdateAsync(new Team() { Id = 1, TeamName = "1Name", TeamOwner = "1Onwer" }).Result;

            //Assert
            Assert.IsTrue(result);

        }

        [Test]
        public void UpdateAsync_TeamRepositoryReturnsFalse_ReturnsFalse()
        {
            //Arrange
            teamRepository = new TeamRepository(context);

            //Act
            var result = teamRepository.UpdateAsync(new Team() { Id = 101, TeamName = "1Name", TeamOwner = "1Onwer" }).Result;

            //Assert
            Assert.IsFalse(result);

        }
        #endregion
    }
}