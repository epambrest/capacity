using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
using Task = System.Threading.Tasks.Task;

namespace Teams.Tests
{
    [TestFixture]
    class ManageSprintsServiceTest
    {
        private Mock<IRepository<Sprint, int>> _sprintRepository;
        private ManageSprintsService _manageSprintsService;
        private Mock<IManageTeamsService> _manageTeamsService;
        private Mock<ICurrentUser> _currentUser;

        [SetUp]
        public void Setup()
        {
            _currentUser = new Mock<ICurrentUser>();
            _sprintRepository = new Mock<IRepository<Sprint, int>>();
            _manageTeamsService = new Mock<IManageTeamsService>();
            _manageSprintsService = new ManageSprintsService(_sprintRepository.Object, _manageTeamsService.Object, _currentUser.Object);
        }

        [Test]
        public async Task GetAllSprintsAsync_ManageSpiritsServiceReturnsListCount5_ReturnsListCount5()
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
        public async Task GetAllSprintsAsync_ManageSpiritsServiceReturnEmpty_ReturnEmpty()
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

        [Test]
        public async Task GetSprintAsync_ManageSpiritsServiceReturnsSprintWithId1_ReturnsSprintWithId1()
        {
            //Arrange
            const int sprint_id = 1;
            var sprint = new Sprint { Id = 1, TeamId = 1, Name = "Sprint1", DaysInSprint = 1, StorePointInHours = 1, IsActive = true };

            _sprintRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(sprint);

            //Act
            var result = await _manageSprintsService.GetSprintAsync(sprint_id);

            //Assert
            Assert.AreEqual(1, result.Id);
        }

        [Test]
        public async Task AddSprintsAsync_ManageSpiritsServiceReturns_True()
        {
            //Arrange
            var sprint = new Sprint { Id = 1, TeamId = 1, Name = "Sprint1", DaysInSprint = 14, StorePointInHours = 4, IsActive = true };

            _sprintRepository.Setup(x => x.InsertAsync(It.IsAny<Sprint>())).ReturnsAsync(true);

            //Act
            var result = await _manageSprintsService.AddSprintAsync(sprint);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task AddSprintsAsync_ManageSpiritsServiceReturns_False()
        {
            //Arrange
            var sprint1 = new Sprint { Id = 1, TeamId = 1, Name = "Spr int1", DaysInSprint = 14, StorePointInHours = 4, IsActive = true };
            var sprint2 = new Sprint { Id = 1, TeamId = 1, Name = "Sprint2", DaysInSprint = -14, StorePointInHours = 4, IsActive = true };
            var sprint3 = new Sprint { Id = 1, TeamId = 1, Name = "Sprint3", DaysInSprint = 14, StorePointInHours = -4, IsActive = true };

            _sprintRepository.Setup(x => x.InsertAsync(It.IsAny<Sprint>())).ReturnsAsync(true);

            //Act
            var result1 = await _manageSprintsService.AddSprintAsync(sprint1); //Incorrect name
            var result2 = await _manageSprintsService.AddSprintAsync(sprint2); //Incorrect days count
            var result3 = await _manageSprintsService.AddSprintAsync(sprint3); //Incorrect SP count

            //Assert
            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
            Assert.IsFalse(result3);
        }


        [Test]
        public async Task RemoveAsync_ManageSprintsServiceReturnsTrue_ReturnsTrue()
        {
            // Arrange
            const string team_owner = "1234";
            const int sprint_id = 1;
            var sprints = new List<Sprint>
            {
                new Sprint { Id = 1, TeamId = 1, Name = "Sprint1", DaysInSprint = 14, StorePointInHours = 4, IsActive = true, Team = new Team{TeamOwner = team_owner}},
                new Sprint { Id = 2, TeamId = 1, Name = "Sprint2", DaysInSprint = 16, StorePointInHours = 4, IsActive = true, Team = new Team{TeamOwner = team_owner}},
                new Sprint { Id = 3, TeamId = 1, Name = "Sprint3", DaysInSprint = 21, StorePointInHours = 2, IsActive = true, Team = new Team{TeamOwner = team_owner}},
                new Sprint { Id = 4, TeamId = 1, Name = "Sprint4", DaysInSprint = 10, StorePointInHours = 3, IsActive = true, Team = new Team{TeamOwner = team_owner}},
                new Sprint { Id = 5, TeamId = 1, Name = "Sprint5", DaysInSprint = 27, StorePointInHours = 5, IsActive = false, Team = new Team{TeamOwner = team_owner}}
            };
            var user = new Mock<UserDetails>(null);
            user.Setup(x => x.Id()).Returns(team_owner);
            _currentUser.SetupGet(x => x.Current).Returns(user.Object);
            var mock = sprints.AsQueryable().BuildMock();
            _sprintRepository.Setup(x => x.GetAll()).Returns(mock.Object);
            _sprintRepository.Setup(x => x.DeleteAsync(It.IsAny<Sprint>()))
            .ReturnsAsync(true);

            //Act
            var result = await _manageSprintsService.RemoveAsync(sprint_id);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task RemoveAsync_ManageSprintsServiceReturnsFalse_ReturnsFalse()
        {
            // Arrange
            const string team_owner = "1234";
            const int sprint_id = 10;
            var sprints = new List<Sprint>
            {
                new Sprint { Id = 1, TeamId = 1, Name = "Sprint1", DaysInSprint = 14, StorePointInHours = 4, IsActive = true, Team = new Team{TeamOwner = team_owner}},
                new Sprint { Id = 2, TeamId = 1, Name = "Sprint2", DaysInSprint = 16, StorePointInHours = 4, IsActive = true, Team = new Team{TeamOwner = team_owner}},
                new Sprint { Id = 3, TeamId = 1, Name = "Sprint3", DaysInSprint = 21, StorePointInHours = 2, IsActive = true, Team = new Team{TeamOwner = team_owner}},
                new Sprint { Id = 4, TeamId = 1, Name = "Sprint4", DaysInSprint = 10, StorePointInHours = 3, IsActive = true, Team = new Team{TeamOwner = team_owner}},
                new Sprint { Id = 5, TeamId = 1, Name = "Sprint5", DaysInSprint = 27, StorePointInHours = 5, IsActive = false, Team = new Team{TeamOwner = "123"}}
            };
            var user = new Mock<UserDetails>(null);
            user.Setup(x => x.Id()).Returns(team_owner);
            _currentUser.SetupGet(x => x.Current).Returns(user.Object);
            var mock = sprints.AsQueryable().BuildMock();
            _sprintRepository.Setup(x => x.GetAll()).Returns(mock.Object);
            _sprintRepository.Setup(x => x.DeleteAsync(It.IsAny<Sprint>()))
            .ReturnsAsync(true);

            //Act
            var result1 = await _manageSprintsService.RemoveAsync(sprint_id);
            var result2 = await _manageSprintsService.RemoveAsync(5);

            //Assert
            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
        }
    }
}
