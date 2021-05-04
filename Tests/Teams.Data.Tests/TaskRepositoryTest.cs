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
    class TaskRepositoryTest
    {
        private ApplicationDbContext _context;
        private IRepository<Task, int> _taskRepository;
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

            _taskRepository = new Repository.Repository<Data.Models.Task, Business.Models.Task, int>(_context, _mapper);
        }

        private IQueryable<Models.Task> GetFakeTaskDb()
        {
            var data = new List<Models.Task>
            {
                new Models.Task 
                { 
                    Id = 1, 
                    Name = "Task1", 
                    TeamId = 1, 
                    SprintId = 1, 
                    MemberId = 1, 
                    Link = "Link1", 
                    StoryPoints = 1, 
                    Sprint = new Models.Sprint(), 
                    Team = new Models.Team(), 
                    TeamMember = new Models.TeamMember() { Member = new Models.User() } 
                },

                new Models.Task 
                { 
                    Id = 2, 
                    Name = "Task2", 
                    TeamId = 2, 
                    SprintId = 2, 
                    MemberId = 2, 
                    Link = "Link2", 
                    StoryPoints = 2,
                    Sprint = new Models.Sprint(),
                    Team = new Models.Team(),
                    TeamMember = new Models.TeamMember() { Member = new Models.User() }
                },

                new Models.Task 
                { 
                    Id = 3, 
                    Name = "Task3", 
                    TeamId = 3, 
                    SprintId = 3, 
                    MemberId = 3, 
                    Link = "Link3", 
                    StoryPoints = 3,
                    Sprint = new Models.Sprint(),
                    Team = new Models.Team(),
                    TeamMember = new Models.TeamMember() { Member = new Models.User() }
                },

                new Models.Task 
                { 
                    Id = 4, 
                    Name = "Task4", 
                    TeamId = 4, 
                    SprintId = 4, 
                    MemberId = 4, 
                    Link = "Link4", 
                    StoryPoints = 4,
                    Sprint = new Models.Sprint(),
                    Team = new Models.Team(),
                    TeamMember = new Models.TeamMember() { Member = new Models.User() }
                },

                new Models.Task 
                { 
                    Id = 5, 
                    Name = "Task5", 
                    TeamId = 5, 
                    SprintId = 5, 
                    MemberId = 5, 
                    Link = "Link5", 
                    StoryPoints = 5,
                    Sprint = new Models.Sprint(),
                    Team = new Models.Team(),
                    TeamMember = new Models.TeamMember() { Member = new Models.User() }
                }
            }.AsQueryable();
            return data;
        }

        [Test]
        public async System.Threading.Tasks.Task GetAll_TaskRepositoryReturns_ListCount5()
        {
            //Arrange
            const int taskCount = 5;
            _context.Task.AddRange(GetFakeTaskDb());
            _context.SaveChanges();

            //Act
            var result = await _taskRepository.GetAllAsync();

            //Assert
            Assert.AreEqual(result.ToList().Count(), taskCount);
        }

        [Test]
        public async System.Threading.Tasks.Task GetByIdAsync_TaskRepositoryReturns_Id3()
        {
            //Arrange
            const int taskId = 5;
            _context.Task.AddRange(GetFakeTaskDb());
            _context.SaveChanges();

            //Act
            var result = await _taskRepository.GetByIdAsync(taskId);

            //Assert
            Assert.AreEqual(taskId, result.Id);
        }

        [Test]
        public async System.Threading.Tasks.Task GetByIdAsync_TaskRepositoryReturns_Null()
        {
            //Arrange
            const int taskId = 6;

            //Act
            var result = await _taskRepository.GetByIdAsync(taskId);

            //Assert
            Assert.IsNull(result);
        }

        [Test]
        public async System.Threading.Tasks.Task InsertAsync_TaskRepositoryReturns_True()
        {
            //Arrange
            Team team = Team.Create(6, "2", "1234", new List<TeamMember>());
            Task task = Task.Create(1, 6, team, "Task5", 6, "Link1", 4, 4); 

            //Act
            var result = await _taskRepository.InsertAsync(task);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task DeleteAsync_TaskRepositoryReturns_True()
        {
            //Arrange
            Models.Task task = new Models.Task 
            {
                Id = 2, 
                Name = "Task2",
                TeamId = 2,
                SprintId = 2, 
                MemberId = 2,
                Link = "Link2", 
                StoryPoints = 2 
            };
            _context.Task.Add(task);
            _context.SaveChanges();

            //Act
            var result = await _taskRepository.DeleteAsync(task.Id);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task UpdateAsync_TaskRepositoryReturns_True()
        {
            //Arrange
            Team team = Team.Create(8, "2", "1234", new List<TeamMember>());
            Task task = Task.Create(2, 8, team, "Task2", 9, "Link2", 8, 7);
            _context.Task.AddRange(GetFakeTaskDb());
            _context.SaveChanges();

            //Act
            var result = await _taskRepository.UpdateAsync(task);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task UpdateAsync_TaskRepositoryReturns_False()
        {
            //Arrange
            Team team = Team.Create(2, "2", "1234", new List<TeamMember>());
            Task task = Task.Create(8, 2, team, "Task2", 2, "Link2", 2, 6);

            //Act
            var result = await _taskRepository.UpdateAsync(task);

            //Assert
            Assert.IsFalse(result);
        }
    }
}