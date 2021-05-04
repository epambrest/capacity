using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Teams.Business.Annotations;
using Teams.Business.Models;
using Teams.Business.Repository;
using Teams.Business.Services;
using Teams.Business.Security;

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
            Team team = Team.Create(1, "1234", "1234", new List<TeamMember>());

            var sprints = new List<Sprint>
            {
                Sprint.Create(1, 1, team, "Sprint1", 14, 4, PossibleStatuses.CompletedStatus),

                Sprint.Create(2, 1, team, "Sprint2", 16, 4, PossibleStatuses.CompletedStatus),

                Sprint.Create(3, 1, team, "Sprint3", 21, 2, PossibleStatuses.ActiveStatus),

                Sprint.Create(4, 1, team, "Sprint4", 10, 3, PossibleStatuses.CompletedStatus),

                Sprint.Create(5, 1, team, "Sprint5", 27, 5, PossibleStatuses.CompletedStatus),
            };

            var mock = sprints.AsQueryable().BuildMock();
            _sprintRepository.Setup(x => x.GetAllAsync())
                .Returns(System.Threading.Tasks.Task.FromResult(sprints.AsEnumerable()));

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
            Team team = Team.Create(1, "1234", "1234", new List<TeamMember>());
            var sprints = new List<Sprint>
            {
                Sprint.Create(1, 1, team, "Sprint1", 14, 4, PossibleStatuses.ActiveStatus),

                Sprint.Create(2, 1, team, "Sprint2", 16, 4, PossibleStatuses.CompletedStatus),

                Sprint.Create(3, 1, team, "Sprint3", 21, 2, PossibleStatuses.CompletedStatus),

                Sprint.Create(4, 1, team, "Sprint4", 10, 3, PossibleStatuses.CompletedStatus),

                Sprint.Create(5, 1, team, "Sprint5", 27, 5, PossibleStatuses.CompletedStatus),
            };

            var mock = sprints.AsQueryable().BuildMock();
            _sprintRepository.Setup(x => x.GetAllAsync()).
                Returns(System.Threading.Tasks.Task.FromResult(sprints.AsEnumerable()));

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
            Team team = Team.Create(1, "1234", "1234", new List<TeamMember>());
            var sprint = Sprint.Create(1, 1, team, "Sprint1", 14, 4, PossibleStatuses.ActiveStatus);

            _sprintRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(sprint);

            //Act
            var result = await _manageSprintsService.GetSprintAsync(sprintId, false);

            //Assert
            Assert.AreEqual(1, result.Id);
        }

        [Test]
        public async System.Threading.Tasks.Task AddSprintsAsync_ManageSpiritsServiceReturns_True()
        {
            //Arrange
            Team team = Team.Create(1, "1234", "1234", new List<TeamMember>());
            var sprint = Sprint.Create(1, 1, team, "Sprint1", 14, 4, PossibleStatuses.ActiveStatus);

            _sprintRepository.Setup(x => x.InsertAsync(It.IsAny<Sprint>())).ReturnsAsync(true);

            //Act
            var result = await _manageSprintsService.AddSprintAsync(sprint);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task AddSprintsAsync_ManageSpiritsServiceReturns_False()
        {
            //Arrange
            Team team = Team.Create(1, "1234", "1234", new List<TeamMember>());
            var sprint1 = Sprint.Create(1, 1, team, "Spr  int1", 14, 4, PossibleStatuses.CompletedStatus);
            var sprint2 = Sprint.Create(1, 1, team, "Sprint2", -14, 4, PossibleStatuses.CompletedStatus);
            var sprint3 = Sprint.Create(1, 1, team, "Sprint3", 14, -4, PossibleStatuses.ActiveStatus);

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
        public async System.Threading.Tasks.Task AddSprintsAsync_ManageSpiritsServiceNameCheckReturns_False()
        {
            //Arrange
            Team team = Team.Create(1, "1234", "1234", new List<TeamMember>());
            var sprint1 = Sprint.Create(1, 1, team, "Spr  int1", 14, 4, PossibleStatuses.CompletedStatus);
            var sprint2 = Sprint.Create(1, 1, team, "Sprint#2", -14, 4, PossibleStatuses.CompletedStatus);
            var sprint3 = Sprint.Create(1, 1, team, "Sprint3 ", 14, 4, PossibleStatuses.ActiveStatus);
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
        public async System.Threading.Tasks.Task AddSprintsAsync_ManageSpiritsServiceNameCheckReturns_True()
        {
            //Arrange
            Team team = Team.Create(1, "1234", "1234", new List<TeamMember>());
            var sprint1 = Sprint.Create(1, 1, team, "Sprint1", 14, 4, PossibleStatuses.CompletedStatus);
            var sprint2 = Sprint.Create(1, 1, team, "Sprint 2", 14, 4, PossibleStatuses.CompletedStatus);
            var sprint3 = Sprint.Create(1, 1, team, "Sprint-3", 14, 4, PossibleStatuses.ActiveStatus);
            var sprint4 = Sprint.Create(1, 1, team, "Sprint_4", 14, 4, PossibleStatuses.ActiveStatus);

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
        public async System.Threading.Tasks.Task RemoveAsync_ManageSprintsServiceReturnsTrue_ReturnsTrue()
        {
            // Arrange
            const string teamOwner = "1234";
            const int sprintId = 1;
            Team team = Team.Create(1, "1234", "1234", new List<TeamMember>());
            var sprints = new List<Sprint>
            {
                Sprint.Create(1, 1, team, "Sprint1", 14, 4, PossibleStatuses.CompletedStatus),
                Sprint.Create(2, 1, team, "Sprint2", 16, 4, PossibleStatuses.CompletedStatus),
                Sprint.Create(3, 1, team, "Sprint3", 21, 2, PossibleStatuses.CompletedStatus),
                Sprint.Create(4, 1, team, "Sprint4", 10, 3, PossibleStatuses.CompletedStatus),
                Sprint.Create(5, 1, team, "Sprint5", 27, 5, PossibleStatuses.CompletedStatus),
            };
            var user = new Mock<UserDetails>(null);
            user.Setup(x => x.Id()).Returns(teamOwner);
            _currentUser.SetupGet(x => x.Current).Returns(user.Object);
            var mock = sprints.AsQueryable().BuildMock();
            _sprintRepository.Setup(x => x.GetAllAsync())
                .Returns(System.Threading.Tasks.Task.FromResult(sprints.AsEnumerable()));
            _sprintRepository.Setup(x => x.DeleteAsync(It.IsAny<int>()))
            .ReturnsAsync(true);

            //Act
            var result = await _manageSprintsService.RemoveAsync(sprintId);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task EditSprintsAsync_ManageSpiritsServiceReturns_True()
        {
            //Arrange
            Team team = Team.Create(1, "1234", "1234", new List<TeamMember>());
            var sprint = Sprint.Create(1, 1, team, "Sprint1", 14, 4, PossibleStatuses.ActiveStatus);

            _sprintRepository.Setup(x => x.UpdateAsync(It.IsAny<Sprint>())).ReturnsAsync(true);
            _sprintRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(sprint);

            //Act
            var result = await _manageSprintsService.EditSprintAsync(sprint);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task RemoveAsync_ManageSprintsServiceReturnsFalse_ReturnsFalse()
        {
            // Arrange
            const string teamOwner = "1234";
            const int sprintId = 10;
            Team team = Team.Create(1, "1234", "1234", new List<TeamMember>());
            Team teamAnotherOwner = Team.Create(1, "123", "1234", new List<TeamMember>());
            var sprints = new List<Sprint>
            {
                Sprint.Create(1, 1, team, "Sprint1", 14, 4, PossibleStatuses.CompletedStatus),
                Sprint.Create(2, 1, team, "Sprint2", 16, 4, PossibleStatuses.ActiveStatus),
                Sprint.Create(3, 1, team, "Sprint3", 21, 2, PossibleStatuses.CompletedStatus),
                Sprint.Create(4, 1, team, "Sprint4", 10, 3, PossibleStatuses.CompletedStatus),
                Sprint.Create(5, 1, teamAnotherOwner, "Sprint5", 27, 5, PossibleStatuses.CompletedStatus),
            };
            var user = new Mock<UserDetails>(null);
            user.Setup(x => x.Id()).Returns(teamOwner);
            _currentUser.SetupGet(x => x.Current).Returns(user.Object);
            var mock = sprints.AsQueryable().BuildMock();
            _sprintRepository.Setup(x => x.GetAllAsync())
                .Returns(System.Threading.Tasks.Task.FromResult(sprints.AsEnumerable()));
            _sprintRepository.Setup(x => x.DeleteAsync(It.IsAny<int>()))
            .ReturnsAsync(true);

            //Act
            var result1 = await _manageSprintsService.RemoveAsync(sprintId);
            var result2 = await _manageSprintsService.RemoveAsync(5);

            //Assert
            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
        }
        
        [Test]
        public async System.Threading.Tasks.Task EditSprintsAsync_ManageSpiritsServiceReturns_False()
        {
            //Arrange
            Team team = Team.Create(1, "1234", "1234", new List<TeamMember>());
            var sprint1 = Sprint.Create(1, 1, team, "Spr  int", 14, 4, PossibleStatuses.ActiveStatus);
            var sprint2 = Sprint.Create(1, 1, team, "Sprint", -14, 4, PossibleStatuses.ActiveStatus);
            var sprint3 = Sprint.Create(1, 1, team, "Sprint", 14, -4, PossibleStatuses.ActiveStatus);

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
        public async System.Threading.Tasks.Task EditSprintsAsync_ManageSpiritsServiceNameCheckReturns_False(string SprintName)
        {
            //Arrange
            Team team = Team.Create(1, "1234", "1234", new List<TeamMember>());
            var sprint = Sprint.Create(1, 1, team, SprintName, 14, 4, PossibleStatuses.ActiveStatus);

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
        public async System.Threading.Tasks.Task EditSprintsAsync_ManageSpiritsServiceNameCheckReturns_True(string SprintName)
        {
            //Arrange
            Team team = Team.Create(1, "1234", "1234", new List<TeamMember>());
            var sprint = Sprint.Create(1, 1, team, SprintName, 14, 4, PossibleStatuses.ActiveStatus);
            
            _sprintRepository.Setup(x => x.UpdateAsync(It.IsAny<Sprint>())).ReturnsAsync(true);
            _sprintRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(sprint);

            //Act
            var result = await _manageSprintsService.EditSprintAsync(sprint);

            //Assert
            Assert.IsTrue(result);
        }
    }
}
