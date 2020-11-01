using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Data;
using Teams.Models;
using Teams.Repository;

namespace Teams.Tests
{
    [TestFixture]
    class SprintRepositoryTest
    {
        private ApplicationDbContext _context;
        private IRepository<Sprint, int> _sprintRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "SprintDatabase").Options;

            _context = new ApplicationDbContext(options);

            _sprintRepository = new SprintRepository(_context);
        }

        private IQueryable<Sprint> GetFakeSprintDb()
        {
            var data = new List<Sprint>
            {
                new Sprint{ Id=1, DaysInSprint= 3, Name = "Sprint1", TeamId= 1, StorePointInHours= 4, IsActive = false },
                new Sprint{ Id=2, DaysInSprint= 3, Name = "Sprint2", TeamId= 2, StorePointInHours= 4, IsActive = false },
                new Sprint{ Id=3, DaysInSprint= 3, Name = "Sprint3", TeamId= 3, StorePointInHours= 4, IsActive = false },
                new Sprint{ Id=4, DaysInSprint= 3, Name = "Sprint4", TeamId= 4, StorePointInHours= 4, IsActive = false },
                new Sprint{ Id=5, DaysInSprint= 3, Name = "Sprint5", TeamId= 5, StorePointInHours= 4, IsActive = false }
            }.AsQueryable();
            return data;
        }

        [Test]
        public async System.Threading.Tasks.Task GetAll_SprintRepositoryReturns_ListCount5()
        {
            //Arrange
            const int sprint_count = 5;
            _context.Sprint.AddRange(GetFakeSprintDb());
            _context.SaveChanges();

            //Act
            var result = await _sprintRepository.GetAll().ToListAsync();

            //Assert
            Assert.AreEqual(result.Count(), sprint_count);
        }

        [Test]
        public async System.Threading.Tasks.Task GetByIdAsync_SprintRepositoryReturns_Id3()
        {
            //Arrange
            const int sprint_id = 3;

            //Act
            var result = await _sprintRepository.GetByIdAsync(sprint_id);

            //Assert
            Assert.AreEqual(sprint_id, result.Id);
        }

        [Test]
        public async System.Threading.Tasks.Task GetByIdAsync_SprintRepositoryReturns_Null()
        {
            //Arrange
            const int sprint_id = 30;

            //Act
            var result = await _sprintRepository.GetByIdAsync(sprint_id);

            //Assert
            Assert.IsNull(result);
        }

        [Test]
        public async System.Threading.Tasks.Task InsertAsync_SprintRepositoryReturns_True()
        {
            //Arrange
            Sprint sprint = new Sprint { Id = 6, DaysInSprint = 3, Name = "Sprint6", TeamId = 6, StorePointInHours = 4, IsActive = false };
           
            //Act
            var result = await _sprintRepository.InsertAsync(sprint);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task DeleteAsync_SprintRepositoryReturns_True()
        {
            //Arrange
            Sprint sprint = new Sprint { Id = 3, DaysInSprint = 3, Name = "Sprint3", TeamId = 3, StorePointInHours = 4, IsActive = false };
            _context.Sprint.Add(sprint);
            _context.SaveChanges();

            //Act
            var result = await _sprintRepository.DeleteAsync(sprint);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task UpdateAsync_SprintRepositoryReturns_True()
        {
            //Arrange
            Sprint sprint = new Sprint { Id = 2, DaysInSprint = 3, Name = "Update", TeamId = 5, StorePointInHours = 4, IsActive = false };
            
            //Act
            var result = await _sprintRepository.UpdateAsync(sprint);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task UpdateAsync_SprintRepositoryReturns_False()
        {
            //Arrange
            Sprint sprint = new Sprint { Id = 10, DaysInSprint = 3, Name = "Sprint5", TeamId = 6, StorePointInHours = 4, IsActive = false };
            
            //Act
            var result = await _sprintRepository.UpdateAsync(sprint);

            //Assert
            Assert.IsFalse(result);
        }
    }
}