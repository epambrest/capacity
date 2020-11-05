using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teams.Data;
using Teams.Models;
using Teams.Security;
using Teams.Services;

namespace Teams.Tests
{
    [TestFixture]
    class ManageSprintsServiceTest
    {
        private Mock<IRepository<Sprint, int>> _sprintRepository;
        private ManageSprintsService _manageSprintsService;
        private Mock<IManageTeamsService> _manageTeamsService;

        [SetUp]
        public void Setup()
        {
            _sprintRepository = new Mock<IRepository<Sprint, int>>();
            _manageTeamsService = new Mock<IManageTeamsService>();
            _manageSprintsService = new ManageSprintsService(_sprintRepository.Object, _manageTeamsService.Object);
        }

        [Test]
        public async System.Threading.Tasks.Task GetAllSprintsAsync_ManageSpiritsServiceReturnsListCount5_ReturnsListCount5()
        {

            //Arrange
            const int team_id = 1;
            var sprints = new List<Sprint>
            {
                new Sprint { Id = 1, TeamId = 1, Name = "Sprint1", DaysInSprint = 14, StorePointInHours = 4, IsActive = true },
                new Sprint { Id = 2, TeamId = 1, Name = "Sprint2", DaysInSprint = 16, StorePointInHours = 4, IsActive = false },
                new Sprint { Id = 3, TeamId = 1, Name = "Sprint3", DaysInSprint = 21, StorePointInHours = 2, IsActive = true }, 
                new Sprint { Id = 4, TeamId = 1, Name = "Sprint4", DaysInSprint = 10, StorePointInHours = 3, IsActive = true },
                new Sprint { Id = 5, TeamId = 1, Name = "Sprint5", DaysInSprint = 27, StorePointInHours = 5, IsActive = false} 
            };

            var mock = sprints.AsQueryable().BuildMock();
            _sprintRepository.Setup(x => x.GetAll()).Returns(mock.Object);

            //Act
            var result = new List<Sprint>(await _manageSprintsService.GetAllSprintsAsync(team_id, new DisplayOptions { }));

            //Assert
            Assert.AreEqual(5, result.Count());
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual(2, result[1].Id);
            Assert.AreEqual(3, result[2].Id);
            Assert.AreEqual(4, result[3].Id);
            Assert.AreEqual(5, result[4].Id);
        }

        [Test]
        public async System.Threading.Tasks.Task GetAllSprintsAsync_ManageSpiritsServiceReturnEmpty_ReturnEmpty()
        {
            //Arrange
            const int team_id = 10;
            var sprints = new List<Sprint>
            {
                new Sprint { Id = 1, TeamId = 1, Name = "Sprint1", DaysInSprint = 14, StorePointInHours = 4, IsActive = true },
                new Sprint { Id = 2, TeamId = 1, Name = "Sprint2", DaysInSprint = 16, StorePointInHours = 4, IsActive = true },
                new Sprint { Id = 3, TeamId = 1, Name = "Sprint3", DaysInSprint = 21, StorePointInHours = 2, IsActive = true },
                new Sprint { Id = 4, TeamId = 1, Name = "Sprint4", DaysInSprint = 10, StorePointInHours = 3, IsActive = true },
                new Sprint { Id = 5, TeamId = 1, Name = "Sprint5", DaysInSprint = 27, StorePointInHours = 5, IsActive = false}
            };

            var mock = sprints.AsQueryable().BuildMock();
            _sprintRepository.Setup(x => x.GetAll()).Returns(mock.Object);

            //Act
            var result = new List<Sprint>(await _manageSprintsService.GetAllSprintsAsync(team_id, new DisplayOptions { }));

            //Assert
            Assert.IsEmpty(result);
        }
    }
}
