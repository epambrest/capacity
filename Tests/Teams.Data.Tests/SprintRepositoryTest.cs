using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Teams.Business.Models;
using Teams.Business.Repository;
using Teams.Data.Mappings;

namespace Teams.Data.Tests
{
    [TestFixture]
    class SprintRepositoryTest
    {
        private ApplicationDbContext _context;
        private IRepository<Sprint, int> _sprintRepository;
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
                mc.ShouldUseConstructor = mc => !mc.IsPrivate;
                mc.AddProfile(new MemberWorkingDaysProfile());
                mc.AddProfile(new SprintProfile());
                mc.AddProfile(new TaskProfile());
                mc.AddProfile(new TeamProfile());
                mc.AddProfile(new UserProfile());
                mc.AddProfile(new TeamMemberProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            _mapper = mapper;

            _context = new ApplicationDbContext(options);

            _sprintRepository = new Repository.Repository<Data.Models.Sprint, Business.Models.Sprint, int>(_context, _mapper);
        }

        private IEnumerable<Models.Sprint> GetFakeSprintDb()
        {
            var data = new List<Models.Sprint>
            {
                new Models.Sprint
                {
                    Id = 1,
                    DaysInSprint = 3,
                    Name = "Sprint1",
                    TeamId = 1,
                    StoryPointInHours = 4,
                    Status = PossibleStatuses.ActiveStatus,
                    Team = new Models.Team() {},
                    Tasks = new List<Models.Task>(),
                    MemberWorkingDays = new List<Models.MemberWorkingDays>() {new Models.MemberWorkingDays() }  
                },

                new Models.Sprint
                {
                    Id = 2, 
                    DaysInSprint = 3, 
                    Name = "Sprint2", 
                    TeamId = 2, 
                    StoryPointInHours = 4, 
                    Status = PossibleStatuses.CompletedStatus,
                    Team = new Models.Team() {},
                    Tasks = new List<Models.Task>(),
                    MemberWorkingDays = new List<Models.MemberWorkingDays>() {new Models.MemberWorkingDays() }
                },

                new Models.Sprint 
                { 
                    Id = 3, 
                    DaysInSprint = 3, 
                    Name = "Sprint3", 
                    TeamId = 3, 
                    StoryPointInHours = 4, 
                    Status = PossibleStatuses.CompletedStatus, 
                    Team = new Models.Team() {}, 
                    Tasks = new List<Models.Task>(),
                    MemberWorkingDays = new List<Models.MemberWorkingDays>() {new Models.MemberWorkingDays() }
                },

                new Models.Sprint 
                { 
                    Id = 4, 
                    DaysInSprint = 3, 
                    Name = "Sprint4", 
                    TeamId = 4, 
                    StoryPointInHours = 4, 
                    Status = PossibleStatuses.CompletedStatus, 
                    Team = new Models.Team() {}, 
                    Tasks = new List<Models.Task>(),
                    MemberWorkingDays = new List<Models.MemberWorkingDays>() {new Models.MemberWorkingDays() }
                },

                new Models.Sprint 
                { 
                    Id = 5, 
                    DaysInSprint = 3, 
                    Name = "Sprint5", 
                    TeamId = 5, 
                    StoryPointInHours = 4,
                    Status = PossibleStatuses.CompletedStatus,
                    Team = new Models.Team() {},
                    Tasks = new List<Models.Task>(),
                    MemberWorkingDays = new List<Models.MemberWorkingDays>() {new Models.MemberWorkingDays()}
                }
            };
            return data;
        }

        [Test]
        public async System.Threading.Tasks.Task GetAll_SprintRepositoryReturns_ListCount5()
        {
            //Arrange
            const int sprintCount = 5;
            _context.Sprint.AddRange(GetFakeSprintDb());
            _context.SaveChanges();
            
            //Act
            var result = await _sprintRepository.GetAllAsync();

            //Assert
            Assert.AreEqual(result.Count(), sprintCount);
        }

        [Test]
        public async System.Threading.Tasks.Task GetByIdAsync_SprintRepositoryReturns_Id3()
        {
            //Arrange
            const int sprintId = 3;
            _context.Sprint.AddRange(GetFakeSprintDb());
            _context.SaveChanges();

            //Act
            var result = await _sprintRepository.GetByIdAsync(sprintId);

            //Assert
            Assert.AreEqual(sprintId, result.Id);
        }

        [Test]
        public async System.Threading.Tasks.Task GetByIdAsync_SprintRepositoryReturns_Null()
        {
            //Arrange
            const int sprintId = 30;

            //Act
            var result = await _sprintRepository.GetByIdAsync(sprintId);

            //Assert
            Assert.IsNull(result);
        }

        [Test]
        public async System.Threading.Tasks.Task InsertAsync_SprintRepositoryReturns_True()
        {
            //Arrange
            Team team = Team.Create(1, "1234", "1234", new List<TeamMember>());
            Sprint sprint = Sprint.Create(6, 6, team, "Sprint", 3, 4, PossibleStatuses.CompletedStatus); 

            //Act
            var result = await _sprintRepository.InsertAsync(sprint);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task DeleteAsync_SprintRepositoryReturns_True()
        {
            //Arrange
            Models.Sprint sprint = new Models.Sprint 
            {
                Id = 3,
                DaysInSprint = 3, 
                Name = "Sprint3",
                TeamId = 3,
                StoryPointInHours = 4,
                Status = PossibleStatuses.CompletedStatus 
            };
            _context.Sprint.Add(sprint);
            _context.SaveChanges();

            //Act
            var result = await _sprintRepository.DeleteAsync(sprint.Id);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task UpdateAsync_SprintRepositoryReturns_True()
        {
            //Arrange
            Team team = Team.Create(1, "1234", "1234", new List<TeamMember>());
            Sprint sprint = Sprint.Create(2, 5, team, "Update", 3, 4, PossibleStatuses.CompletedStatus);

            _context.Sprint.AddRange(GetFakeSprintDb());
            _context.SaveChanges();

            //Act
            var result = await _sprintRepository.UpdateAsync(sprint);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task UpdateAsync_SprintRepositoryReturns_False()
        {
            //Arrange
            Team team = Team.Create(1, "1234", "1234", new List<TeamMember>());
            Sprint sprint = Sprint.Create(10, 6, team, "Sprint5", 3, 4, PossibleStatuses.CompletedStatus);

            //Act
            var result = await _sprintRepository.UpdateAsync(sprint);

            //Assert
            Assert.IsFalse(result);
        }
    }
}