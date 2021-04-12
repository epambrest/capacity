using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Teams.Business.Models;
using Teams.Business.Repository;
using Teams.Data.Mappings;

namespace Teams.Data.Tests
{
    class TeamMembersRepositoryTest
    {
        [TestFixture]
        class TeamRepositoryTest
        {
            private ApplicationDbContext context;
            private IRepository<TeamMember, int> teamMemberRepository;
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

            private IQueryable<Models.TeamMember> GenerateData(int num)
            {
                var lst = new List<Models.TeamMember>();
                for (int i = 1; i < num; i++)
                {
                    lst.Add(new Models.TeamMember
                    {
                        Id = i,
                        MemberId = i.ToString(),
                        Member = new Models.User(),
                        TeamId = i
                    });
                }
                return lst.AsQueryable();
            }

            #region GetAll
            [Test]
            public async System.Threading.Tasks.Task GetAll_TeamRepositoryReturnsListCount100_ListCount100()
            {
                //Arrange
                context.TeamMembers.AddRange(GenerateData(100));
                context.SaveChanges();
                teamMemberRepository = new Repository.Repository<Models.TeamMember, TeamMember, int>(context, _mapper);

                //Act
                var teamMembers = await teamMemberRepository.GetAllAsync();

                //Assert
                Assert.AreEqual(context.TeamMembers.Count(), teamMembers.Count());

            }

            [Test]
            public async System.Threading.Tasks.Task GetAll_TeamRepositoryReturnsEmptyList_ReturnsEmpty()
            {
                //Arrange
                teamMemberRepository = new Repository.Repository<Models.TeamMember, TeamMember, int>(context, _mapper);

                //Act
                var teams = await teamMemberRepository.GetAllAsync();

                //Assert
                Assert.IsEmpty(teams);

            }
            #endregion

            #region GetByIdAsync
            [Test]
            public void GetByIdAsync_TeamRepositoryReturnsTeamId1_ReturnsTeamId1()
            {
                //Arrange
                if (context.TeamMembers.Count() < 1)
                    context.TeamMembers.AddRange(GenerateData(100));
                teamMemberRepository = new Repository.Repository<Models.TeamMember, TeamMember, int>(context, _mapper);

                //Act
                var team = teamMemberRepository.GetByIdAsync(1).Result;

                //Assert
                Assert.AreEqual(1, team.Id);

            }

            [Test]
            public void GetByIdAsync_TeamRepositoryReturnsNull_ReturnsNull()
            {
                //Arrange
                teamMemberRepository = new Repository.Repository<Models.TeamMember, TeamMember, int>(context, _mapper);

                //Act
                var team = teamMemberRepository.GetByIdAsync(101).Result;

                //Assert
                Assert.IsNull(team);

            }
            #endregion

            #region InsertAsync
            [Test]
            public void InsertAsync_TeamRepositoryReturnsTrue_ReturnsTrue()
            {
                //Arrange
                teamMemberRepository = new Repository.Repository<Models.TeamMember, TeamMember, int>(context, _mapper);
                TeamMember teamMember = new TeamMember() { MemberId = "100" };

                //Act
                var result = teamMemberRepository.InsertAsync(teamMember).Result;

                //Assert
                Assert.IsTrue(result);

            }
            #endregion

            #region DeleteAsync
            [Test]
            public void DeleteAsync_TeamRepositoryReturnsTrue_ReturnsTrue()
            {
                //Arrange
                context.TeamMembers.AddRange(GenerateData(2));
                context.SaveChanges();
                teamMemberRepository = new Repository.Repository<Models.TeamMember, TeamMember, int>(context, _mapper);
                //Act
                var result = teamMemberRepository.DeleteAsync(teamMemberRepository.GetByIdAsync(1).Result.Id).Result;

                //Assert
                Assert.IsTrue(result);
            }

            [Test]
            public void DeleteAsync_TeamRepositoryReturnsNull_ReturnsNull()
            {
                //Arrange
                teamMemberRepository = new Repository.Repository<Models.TeamMember, TeamMember, int>(context, _mapper);

                //Act&Assert
                try
                {
                    var result = teamMemberRepository.DeleteAsync(0);
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
                if (context.TeamMembers.Count() < 1)
                {
                    context.TeamMembers.AddRange(GenerateData(3));
                    context.SaveChanges();
                }
                teamMemberRepository = new Repository.Repository<Models.TeamMember, TeamMember, int>(context, _mapper);

                //Act
                var result = teamMemberRepository.UpdateAsync(new TeamMember() { Id = 1, MemberId = "01" }).Result;

                //Assert
                Assert.IsTrue(result);

            }

            [Test]
            public void UpdateAsync_TeamRepositoryReturnsFalse_ReturnsFalse()
            {
                //Arrange
                teamMemberRepository = new Repository.Repository<Models.TeamMember, TeamMember, int>(context, _mapper);

                //Act
                var result = teamMemberRepository.UpdateAsync(new TeamMember() { Id = 101, MemberId = "101" }).Result;

                //Assert
                Assert.IsFalse(result);

            }
            #endregion
        }

    }
}