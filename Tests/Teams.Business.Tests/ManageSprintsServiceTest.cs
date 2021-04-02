using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Teams.Business.Services;
using Teams.Data;
using Teams.Data.Annotations;
using Teams.Data.Models;
using Teams.Security;
using Task = System.Threading.Tasks.Task;

namespace Teams.Business.Tests
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
        public async System.Threading.Tasks.Task GetAllSprintsAsync_ManageSpiritsServiceReturnsListCount5_ReturnsListCount5()
        {

            //Arrange
            const int teamId = 1;

            var sprints = new List<Sprint>
            {
                new Sprint { Id = 1, TeamId = 1, Name = "Sprint1", DaysInSprint = 14, StoryPointInHours = 4, Status = PossibleStatuses.CompletedStatus },
                new Sprint { Id = 2, TeamId = 1, Name = "Sprint2", DaysInSprint = 16, StoryPointInHours = 4, Status = PossibleStatuses.CompletedStatus },
                new Sprint { Id = 3, TeamId = 1, Name = "Sprint3", DaysInSprint = 21, StoryPointInHours = 2, Status = PossibleStatuses.ActiveStatus },
                new Sprint { Id = 4, TeamId = 1, Name = "Sprint4", DaysInSprint = 10, StoryPointInHours = 3, Status = PossibleStatuses.CompletedStatus },
                new Sprint { Id = 5, TeamId = 1, Name = "Sprint5", DaysInSprint = 27, StoryPointInHours = 5, Status = PossibleStatuses.CompletedStatus }
            };

            var mock = sprints.AsQueryable().BuildMock();
            _sprintRepository.Setup(x => x.GetAll()).Returns(mock.Object);

            //Act
            var result = new List<Sprint>(await _manageSprintsService.GetAllSprintsAsync(teamId, new DisplayOptions { }));

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
            const int teamId = 10;
            var sprints = new List<Sprint>
            {
                new Sprint { Id = 1, TeamId = 1, Name = "Sprint1", DaysInSprint = 14, StoryPointInHours = 4, Status = PossibleStatuses.ActiveStatus },
                new Sprint { Id = 2, TeamId = 1, Name = "Sprint2", DaysInSprint = 16, StoryPointInHours = 4, Status = PossibleStatuses.CompletedStatus },
                new Sprint { Id = 3, TeamId = 1, Name = "Sprint3", DaysInSprint = 21, StoryPointInHours = 2, Status = PossibleStatuses.CompletedStatus },
                new Sprint { Id = 4, TeamId = 1, Name = "Sprint4", DaysInSprint = 10, StoryPointInHours = 3, Status = PossibleStatuses.CompletedStatus },
                new Sprint { Id = 5, TeamId = 1, Name = "Sprint5", DaysInSprint = 27, StoryPointInHours = 5, Status = PossibleStatuses.CompletedStatus }
            };

            var mock = sprints.AsQueryable().BuildMock();
            _sprintRepository.Setup(x => x.GetAll()).Returns(mock.Object);

            //Act
            var result = new List<Sprint>(await _manageSprintsService.GetAllSprintsAsync(teamId, new DisplayOptions { }));

            //Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public async System.Threading.Tasks.Task GetSprintAsync_ManageSpiritsServiceReturnsSprintWithId1_ReturnsSprintWithId1()
        {

            //Arrange
            const int sprintId = 1;
            var sprint = new Sprint { Id = 1, TeamId = 1, Name = "Sprint1", DaysInSprint = 1, StoryPointInHours = 1, Status = PossibleStatuses.ActiveStatus };

            _sprintRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(sprint);

            //Act
            var result = await _manageSprintsService.GetSprintAsync(sprintId, false);

            //Assert
            Assert.AreEqual(1, result.Id);
        }

        [Test]
        public async Task AddSprintsAsync_ManageSpiritsServiceReturns_True()
        {
            //Arrange
            var sprint = new Sprint { Id = 1, TeamId = 1, Name = "Sprint1", DaysInSprint = 14, StoryPointInHours = 4, Status = PossibleStatuses.ActiveStatus };

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
            var sprint1 = new Sprint { Id = 1, TeamId = 1, Name = "Spr  int1", DaysInSprint = 14, StoryPointInHours = 4, Status = PossibleStatuses.CompletedStatus };
            var sprint2 = new Sprint { Id = 1, TeamId = 1, Name = "Sprint2", DaysInSprint = -14, StoryPointInHours = 4, Status = PossibleStatuses.CompletedStatus };
            var sprint3 = new Sprint { Id = 1, TeamId = 1, Name = "Sprint3", DaysInSprint = 14, StoryPointInHours = -4, Status = PossibleStatuses.ActiveStatus };

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
        public async Task AddSprintsAsync_ManageSpiritsServiceNameCheckReturns_False()
        {
            //Arrange
            var sprint1 = new Sprint { Id = 1, TeamId = 1, Name = "Spr  int1", DaysInSprint = 14, StoryPointInHours = 4, Status = PossibleStatuses.CompletedStatus };
            var sprint2 = new Sprint { Id = 1, TeamId = 1, Name = "Sprint#2", DaysInSprint = 14, StoryPointInHours = 4, Status = PossibleStatuses.CompletedStatus };
            var sprint3 = new Sprint { Id = 1, TeamId = 1, Name = "Sprint3 ", DaysInSprint = 14, StoryPointInHours = 4, Status = PossibleStatuses.ActiveStatus };

            _sprintRepository.Setup(x => x.InsertAsync(It.IsAny<Sprint>())).ReturnsAsync(true);

            //Act
            var result1 = await _manageSprintsService.AddSprintAsync(sprint1); 
            var result2 = await _manageSprintsService.AddSprintAsync(sprint2); 
            var result3 = await _manageSprintsService.AddSprintAsync(sprint3); 

            //Assert
            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
            Assert.IsFalse(result3);
        }

        [Test]
        public async Task AddSprintsAsync_ManageSpiritsServiceNameCheckReturns_True()
        {
            //Arrange
            var sprint1 = new Sprint { Id = 1, TeamId = 1, Name = "Sprint1", DaysInSprint = 14, StoryPointInHours = 4, Status = PossibleStatuses.CompletedStatus };
            var sprint2 = new Sprint { Id = 1, TeamId = 1, Name = "Sprint 2", DaysInSprint = 14, StoryPointInHours = 4, Status = PossibleStatuses.CompletedStatus };
            var sprint3 = new Sprint { Id = 1, TeamId = 1, Name = "Sprint-3", DaysInSprint = 14, StoryPointInHours = 4, Status = PossibleStatuses.ActiveStatus };
            var sprint4 = new Sprint { Id = 1, TeamId = 1, Name = "Sprint_4", DaysInSprint = 14, StoryPointInHours = 4, Status = PossibleStatuses.ActiveStatus };

            _sprintRepository.Setup(x => x.InsertAsync(It.IsAny<Sprint>())).ReturnsAsync(true);

            //Act
            var result1 = await _manageSprintsService.AddSprintAsync(sprint1);
            var result2 = await _manageSprintsService.AddSprintAsync(sprint2);
            var result3 = await _manageSprintsService.AddSprintAsync(sprint3);
            var result4 = await _manageSprintsService.AddSprintAsync(sprint4);

            //Assert
            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
            Assert.IsTrue(result3);
            Assert.IsTrue(result4);
        }

        [Test]
        public async Task RemoveAsync_ManageSprintsServiceReturnsTrue_ReturnsTrue()
        {
            // Arrange
            const string teamOwner = "1234";
            const int sprintId = 1;
            var sprints = new List<Sprint>
            {
                new Sprint { Id = 1, TeamId = 1, Name = "Sprint1", DaysInSprint = 14, StoryPointInHours = 4, Status = PossibleStatuses.ActiveStatus, Team = new Team{TeamOwner = teamOwner}},
                new Sprint { Id = 2, TeamId = 1, Name = "Sprint2", DaysInSprint = 16, StoryPointInHours = 4, Status = PossibleStatuses.CompletedStatus, Team = new Team{TeamOwner = teamOwner}},
                new Sprint { Id = 3, TeamId = 1, Name = "Sprint3", DaysInSprint = 21, StoryPointInHours = 2, Status = PossibleStatuses.CompletedStatus, Team = new Team{TeamOwner = teamOwner}},
                new Sprint { Id = 4, TeamId = 1, Name = "Sprint4", DaysInSprint = 10, StoryPointInHours = 3, Status = PossibleStatuses.CompletedStatus, Team = new Team{TeamOwner = teamOwner}},
                new Sprint { Id = 5, TeamId = 1, Name = "Sprint5", DaysInSprint = 27, StoryPointInHours = 5, Status = PossibleStatuses.CompletedStatus, Team = new Team{TeamOwner = teamOwner}}
            };
            var user = new Mock<UserDetails>(null);
            user.Setup(x => x.Id()).Returns(teamOwner);
            _currentUser.SetupGet(x => x.Current).Returns(user.Object);
            var mock = sprints.AsQueryable().BuildMock();
            _sprintRepository.Setup(x => x.GetAll()).Returns(mock.Object);
            _sprintRepository.Setup(x => x.DeleteAsync(It.IsAny<Sprint>()))
            .ReturnsAsync(true);

            //Act
            var result = await _manageSprintsService.RemoveAsync(sprintId);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task EditSprintsAsync_ManageSpiritsServiceReturns_True()
        {
            //Arrange
            var sprint = new Sprint { Id = 1, TeamId = 1, Name = "Sprint1", DaysInSprint = 14, StoryPointInHours = 4, Status = PossibleStatuses.ActiveStatus };

            _sprintRepository.Setup(x => x.UpdateAsync(It.IsAny<Sprint>())).ReturnsAsync(true);
            _sprintRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(sprint);

            //Act
            var result = await _manageSprintsService.EditSprintAsync(sprint);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task RemoveAsync_ManageSprintsServiceReturnsFalse_ReturnsFalse()
        {
            // Arrange
            const string teamOwner = "1234";
            const int sprintId = 10;
            var sprints = new List<Sprint>
            {
                new Sprint { Id = 1, TeamId = 1, Name = "Sprint1", DaysInSprint = 14, StoryPointInHours = 4, Status = PossibleStatuses.CompletedStatus, Team = new Team{TeamOwner = teamOwner}},
                new Sprint { Id = 2, TeamId = 1, Name = "Sprint2", DaysInSprint = 16, StoryPointInHours = 4, Status = PossibleStatuses.ActiveStatus, Team = new Team{TeamOwner = teamOwner}},
                new Sprint { Id = 3, TeamId = 1, Name = "Sprint3", DaysInSprint = 21, StoryPointInHours = 2, Status = PossibleStatuses.CompletedStatus, Team = new Team{TeamOwner = teamOwner}},
                new Sprint { Id = 4, TeamId = 1, Name = "Sprint4", DaysInSprint = 10, StoryPointInHours = 3, Status = PossibleStatuses.CompletedStatus, Team = new Team{TeamOwner = teamOwner}},
                new Sprint { Id = 5, TeamId = 1, Name = "Sprint5", DaysInSprint = 27, StoryPointInHours = 5, Status = PossibleStatuses.CompletedStatus, Team = new Team{TeamOwner = "123"}}
            };
            var user = new Mock<UserDetails>(null);
            user.Setup(x => x.Id()).Returns(teamOwner);
            _currentUser.SetupGet(x => x.Current).Returns(user.Object);
            var mock = sprints.AsQueryable().BuildMock();
            _sprintRepository.Setup(x => x.GetAll()).Returns(mock.Object);
            _sprintRepository.Setup(x => x.DeleteAsync(It.IsAny<Sprint>()))
            .ReturnsAsync(true);

            //Act
            var result1 = await _manageSprintsService.RemoveAsync(sprintId);
            var result2 = await _manageSprintsService.RemoveAsync(5);

            //Assert
            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
        }
        
        [Test]
        public async Task EditSprintsAsync_ManageSpiritsServiceReturns_False()
        {
            //Arrange
            var sprint1 = new Sprint { Id = 1, TeamId = 1, Name = "Spr  int", DaysInSprint = 14, StoryPointInHours = 4, Status = PossibleStatuses.ActiveStatus };
            var sprint2 = new Sprint { Id = 1, TeamId = 1, Name = "Sprint", DaysInSprint = -14, StoryPointInHours = 4, Status = PossibleStatuses.ActiveStatus };
            var sprint3 = new Sprint { Id = 1, TeamId = 1, Name = "Sprint", DaysInSprint = 14, StoryPointInHours = -4, Status = PossibleStatuses.ActiveStatus };

            _sprintRepository.Setup(x => x.UpdateAsync(It.IsAny<Sprint>())).ReturnsAsync(true);
            _sprintRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(sprint2);

            //Act
            var result1 = await _manageSprintsService.EditSprintAsync(sprint1); //Incorrect name
            var result2 = await _manageSprintsService.EditSprintAsync(sprint2); //Incorrect days count
            var result3 = await _manageSprintsService.EditSprintAsync(sprint3); //Incorrect SP count


            //Assert
            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
            Assert.IsFalse(result3);
        }

        [TestCase("Spr  int")]
        [TestCase("Sprint!")]
        [TestCase("Sprint ")]
        public async Task EditSprintsAsync_ManageSpiritsServiceNameCheckReturns_False(string SprintName)
        {
            //Arrange
            var sprint = new Sprint { Id = 1, TeamId = 1, Name = SprintName, DaysInSprint = 14, StoryPointInHours = 4, Status = PossibleStatuses.ActiveStatus };

            _sprintRepository.Setup(x => x.UpdateAsync(It.IsAny<Sprint>())).ReturnsAsync(true);
            _sprintRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(sprint);

            //Act
            var result = await _manageSprintsService.EditSprintAsync(sprint); 

            //Assert
            Assert.IsFalse(result);
        }

        [TestCase("Sprint")]
        [TestCase("Sprint 1")]
        [TestCase("Sprint-2_3")]
        public async Task EditSprintsAsync_ManageSpiritsServiceNameCheckReturns_True(string SprintName)
        {
            //Arrange
            var sprint = new Sprint { Id = 1, TeamId = 1, Name = SprintName, DaysInSprint = 14, StoryPointInHours = 4, Status = PossibleStatuses.ActiveStatus };
            
            _sprintRepository.Setup(x => x.UpdateAsync(It.IsAny<Sprint>())).ReturnsAsync(true);
            _sprintRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(sprint);

            //Act
            var result = await _manageSprintsService.EditSprintAsync(sprint);

            //Assert
            Assert.IsTrue(result);
        }
    }
}
