using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Teams.Business.Mappings;
using Teams.Business.Models;
using Teams.Data.Models;
using Teams.Data.Repository;

namespace Teams.Data.Tests
{
    [TestFixture]
    class TeamRepositoryTest
    {
        private ApplicationDbContext context;
        private IRepository<Models.Team, Business.Models.Team, int> teamRepository;
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            var serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase()
                .AddEntityFrameworkProxies().BuildServiceProvider();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "MemberWorkingDaysDatabase").UseInternalServiceProvider(serviceProvider)
                .UseLazyLoadingProxies().Options;

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

        private IQueryable<Data.Models.Team> GenerateData(int num)
        {
            var lst = new List<Data.Models.Team>();
            for (int i = 1; i < num; i++)
            {
                lst.Add(new Data.Models.Team
                {
                    Id = i,
                    TeamName = "Name" + i,
                    TeamOwner = "Owner" + i,
                    TeamMembers = new List<Data.Models.TeamMember>(),
                    Owner = new Data.Models.User()
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
            teamRepository = new Repository.Repository<Models.Team, Business.Models.Team, int>(context, _mapper);

            //Act
            var teams = await teamRepository.GetAllAsync();

            //Assert
            Assert.AreEqual(context.Team.Count(), teams.Count());

        }

        [Test]
        public async System.Threading.Tasks.Task GetAll_TeamRepositoryReturnsEmptyList_ReturnsEmpty()
        {
            //Arrange
            teamRepository = new Repository.Repository<Models.Team, Business.Models.Team, int>(context, _mapper);

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
            teamRepository = new Repository.Repository<Models.Team, Business.Models.Team, int>(context, _mapper);

            //Act
            var team = teamRepository.GetByIdAsync(1).Result;

            //Assert
            Assert.AreEqual(1, team.Id);

        }

        [Test]
        public void GetByIdAsync_TeamRepositoryReturnsNull_ReturnsNull()
        {
            //Arrange
            teamRepository = new Repository.Repository<Models.Team, Business.Models.Team, int>(context, _mapper);

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
            teamRepository = new Repository.Repository<Models.Team, Business.Models.Team, int>(context, _mapper);
            Business.Models.Team team = new Business.Models.Team() { TeamName = "Name101", TeamOwner = "Owner101" };

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
            teamRepository = new Repository.Repository<Models.Team, Business.Models.Team, int>(context, _mapper);

            //Act
            var result = teamRepository.DeleteAsync(teamRepository.GetByIdAsync(1).Result.Id).Result;

            //Assert
            Assert.IsTrue(result);

        }

        [Test]
        public void DeleteAsync_TeamRepositoryReturnsNull_ReturnsNull()
        {
            //Arrange
            teamRepository = new Repository.Repository<Models.Team, Business.Models.Team, int>(context, _mapper);

            //Act&Assert
            try
            {
                var result = teamRepository.DeleteAsync(0);
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
            teamRepository = new Repository.Repository<Models.Team, Business.Models.Team, int>(context, _mapper);

            //Act
            var result = teamRepository.UpdateAsync(new Business.Models.Team() { Id = 1, TeamName = "1Name", TeamOwner = "1Onwer" }).Result;

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void UpdateAsync_TeamRepositoryReturnsFalse_ReturnsFalse()
        {
            //Arrange
            teamRepository = new Repository.Repository<Models.Team, Business.Models.Team, int>(context, _mapper);

            //Act
            var result = teamRepository.UpdateAsync(new Business.Models.Team() { Id = 101, TeamName = "1Name", TeamOwner = "1Onwer" }).Result;

            //Assert
            Assert.IsFalse(result);
        }
        #endregion
    }
}