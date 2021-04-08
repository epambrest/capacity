using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Teams.Business.Models;
using Teams.Business.Repository;
using Teams.Data.Mappings;
using Teams.Data.Models;
using Teams.Data.Repository;

namespace Teams.Data.Tests
{
    [TestFixture]
    class TeamRepositoryTest
    {
        private ApplicationDbContext context;
        private IRepository<TeamBusiness, int> teamRepository;
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TeamListDatabase")
            .Options;

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MemberWorkingDaysProfile());
                mc.AddProfile(new SprintProfile());
                mc.AddProfile(new TaskProfile());
                mc.AddProfile(new TeamProfile());
                mc.AddProfile(new UserProfile());
                mc.AddProfile(new TeamMemberProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            _mapper = mapper;

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
                    TeamOwner = "Owner" + i,
                    TeamMembers = new List<TeamMember>(),
                    Owner = new User()
                });
            }
            return lst.AsQueryable();
        }

        #region GetAll
        [Test]
        public async System.Threading.Tasks.Task GetAll_TeamRepositoryReturnsListCount100_ListCount100()
        {
            //Arrange
            context.Team.AddRange(GenerateData(100));
            context.SaveChanges();
            teamRepository = new TeamRepository(context, _mapper);

            //Act
            var teams = await teamRepository.GetAllAsync();

            //Assert
            Assert.AreEqual(context.Team.Count(), teams.Count());

        }

        [Test]
        public async System.Threading.Tasks.Task GetAll_TeamRepositoryReturnsEmptyList_ReturnsEmpty()
        {
            //Arrange
            teamRepository = new TeamRepository(context, _mapper);

            //Act
            var teams = await teamRepository.GetAllAsync();

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
            teamRepository = new TeamRepository(context, _mapper);

            //Act
            var team = teamRepository.GetByIdAsync(1).Result;

            //Assert
            Assert.AreEqual(1, team.Id);

        }

        [Test]
        public void GetByIdAsync_TeamRepositoryReturnsNull_ReturnsNull()
        {
            //Arrange
            teamRepository = new TeamRepository(context, _mapper);

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
            teamRepository = new TeamRepository (context, _mapper);
            TeamBusiness team = new TeamBusiness() { TeamName = "Name101", TeamOwner = "Owner101" };

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
            teamRepository = new TeamRepository(context, _mapper);

            //Act
            var result = teamRepository.DeleteAsync(teamRepository.GetByIdAsync(1).Result).Result;

            //Assert
            Assert.IsTrue(result);

        }

        [Test]
        public void DeleteAsync_TeamRepositoryReturnsNull_ReturnsNull()
        {
            //Arrange
            teamRepository = new TeamRepository(context, _mapper);

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
            teamRepository = new TeamRepository(context, _mapper);

            //Act
            var result = teamRepository.UpdateAsync(new TeamBusiness() { Id = 1, TeamName = "1Name", TeamOwner = "1Onwer" }).Result;

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void UpdateAsync_TeamRepositoryReturnsFalse_ReturnsFalse()
        {
            //Arrange
            teamRepository = new TeamRepository(context, _mapper);

            //Act
            var result = teamRepository.UpdateAsync(new TeamBusiness() { Id = 101, TeamName = "1Name", TeamOwner = "1Onwer" }).Result;

            //Assert
            Assert.IsFalse(result);
        }
        #endregion
    }
}