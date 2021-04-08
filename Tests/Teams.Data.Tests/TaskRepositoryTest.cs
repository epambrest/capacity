using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
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
    class TaskRepositoryTest
    {
        private ApplicationDbContext _context;
        private IRepository<TaskBusiness, int> _taskRepository;
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TaskDatabase").Options;

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

            _context = new ApplicationDbContext(options);

            _taskRepository = new TaskRepository(_context, _mapper);
        }

        private IQueryable<Task> GetFakeTaskDb()
        {
            var data = new List<Task>
            {
                new Task 
                { 
                    Id = 1, 
                    Name = "Task1", 
                    TeamId = 1, 
                    SprintId = 1, 
                    MemberId = 1, 
                    Link = "Link1", 
                    StoryPoints = 1, 
                    Sprint = new Sprint(), 
                    Team = new Team(), 
                    TeamMember = new TeamMember() { Member = new User() } 
                },

                new Task 
                { 
                    Id = 2, 
                    Name = "Task2", 
                    TeamId = 2, 
                    SprintId = 2, 
                    MemberId = 2, 
                    Link = "Link2", 
                    StoryPoints = 2,
                    Sprint = new Sprint(),
                    Team = new Team(),
                    TeamMember = new TeamMember() { Member = new User() }
                },

                new Task 
                { 
                    Id = 3, 
                    Name = "Task3", 
                    TeamId = 3, 
                    SprintId = 3, 
                    MemberId = 3, 
                    Link = "Link3", 
                    StoryPoints = 3,
                    Sprint = new Sprint(),
                    Team = new Team(),
                    TeamMember = new TeamMember() { Member = new User() }
                },

                new Task 
                { 
                    Id = 4, 
                    Name = "Task4", 
                    TeamId = 4, 
                    SprintId = 4, 
                    MemberId = 4, 
                    Link = "Link4", 
                    StoryPoints = 4,
                    Sprint = new Sprint(),
                    Team = new Team(),
                    TeamMember = new TeamMember() { Member = new User() }
                },

                new Task 
                { 
                    Id = 5, 
                    Name = "Task5", 
                    TeamId = 5, 
                    SprintId = 5, 
                    MemberId = 5, 
                    Link = "Link5", 
                    StoryPoints = 5,
                    Sprint = new Sprint(),
                    Team = new Team(),
                    TeamMember = new TeamMember() { Member = new User() }
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
            TaskBusiness task = new TaskBusiness 
            {
                Id = 6, 
                Name = "Task6",
                TeamId = 6,
                SprintId = 6,
                MemberId = 6, 
                Link = "Link6", 
                StoryPoints = 6
            };

            //Act
            var result = await _taskRepository.InsertAsync(task);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task DeleteAsync_TaskRepositoryReturns_True()
        {
            //Arrange
            Task task = new Task 
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
            var result = await _taskRepository.DeleteAsync(_mapper.Map<TaskBusiness>(task));

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task UpdateAsync_TaskRepositoryReturns_True()
        {
            //Arrange
            TaskBusiness task = new TaskBusiness
            { 
                Id = 2, 
                Name = "Task2",
                TeamId = 8, 
                SprintId = 8, 
                MemberId = 7,
                Link = "Link2",
                StoryPoints = 9 
            };

            //Act
            var result = await _taskRepository.UpdateAsync(task);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task UpdateAsync_TaskRepositoryReturns_False()
        {
            //Arrange
            TaskBusiness task = new TaskBusiness 
            { 
                Id = 8,
                Name = "Task2", 
                TeamId = 2,
                SprintId = 2,
                MemberId = 6, 
                Link = "Link2",
                StoryPoints = 2 
            };

            //Act
            var result = await _taskRepository.UpdateAsync(task);

            //Assert
            Assert.IsFalse(result);
        }
    }
}