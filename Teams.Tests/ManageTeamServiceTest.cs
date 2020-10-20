using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using Teams.Data;
using Teams.Models;
using Teams.Security;
using Moq;
using MockQueryable.Moq;
using Teams.Services;
using System.Threading.Tasks;

namespace Teams.Tests
{
    [TestFixture]
    class ManageTeamServiceTest
    {
            private Mock<IRepository<Team, int>> _teamRepository;
            private Mock<ICurrentUser> _currentUser;
            private IManageTeamsService _manageTeamsService;

            [SetUp]
            public void Setup()
            {
                _currentUser = new Mock<ICurrentUser>();
                _teamRepository = new Mock<IRepository<Team, int>>();
                _manageTeamsService = new ManageTeamsService(_currentUser.Object, _teamRepository.Object);
            }

            [Test]
            public async Task RemoveAsync_ManageTeamsServiceReturnsTrue_ReturnsTrue()
            {
                // Arrange
                const string team_owner = "1234";
                const int team_id = 1;
                var teams = new List<Team>
                {
                 new Team { Id= 1, TeamOwner = "1234", TeamName = "First_Team"},
                 new Team { Id= 2, TeamOwner = "1234", TeamName = "Second_Team"}
                };
                var user = new Mock<UserDetails>(null);
                user.Setup(x => x.Id()).Returns(team_owner);
                _currentUser.SetupGet(x => x.Current).Returns(user.Object);
                var mock = teams.AsQueryable().BuildMock();
                _teamRepository.Setup(x => x.GetAll()).Returns(mock.Object);
                _teamRepository.Setup(x => x.DeleteAsync(It.IsAny<Team>()))
                .ReturnsAsync(true);

                //Act
                var result = await _manageTeamsService.RemoveAsync(team_id);
              
                //Assert
                Assert.IsTrue(result); 
            }

            [Test]
            public async Task RemoveAsync_ManageTeamsServiceReturnsFalse_ReturnsFalse()
            {
                // Arrange
                const string team_owner = "1234";
                const int team_id1 = 4;
                const int team_id2 = 3;
                var teams = new List<Team>
                {
                 new Team { Id= 1, TeamOwner = "1234", TeamName = "First_Team"},
                 new Team { Id= 2, TeamOwner = "1234", TeamName = "Second_Team"},
                 new Team { Id= 3, TeamOwner = "4152", TeamName = "Third_Team"},
                };
                var user = new Mock<UserDetails>(null);
                user.Setup(x => x.Id()).Returns(team_owner);
                _currentUser.SetupGet(x => x.Current).Returns(user.Object);
                var mock = teams.AsQueryable().BuildMock();
                _teamRepository.Setup(x => x.GetAll()).Returns(mock.Object);
                _teamRepository.Setup(x => x.DeleteAsync(It.IsAny<Team>()))
                .ReturnsAsync(true);

                //Act
                var result1 = await _manageTeamsService.RemoveAsync(team_id1);
                var result2 = await _manageTeamsService.RemoveAsync(team_id2);

                //Assert
                Assert.IsFalse(result1);
                Assert.IsFalse(result2);
            }

    }
    
}




