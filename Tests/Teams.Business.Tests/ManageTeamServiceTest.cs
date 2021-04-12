using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Business.Models;
using Teams.Business.Services;
using Teams.Data.Models;
using Teams.Data.Repository;
using Teams.Security;

namespace Teams.Business.Tests
{
    [TestFixture]
    class ManageTeamServiceTest
    {
            private Mock<IRepository<Data.Models.Team, Models.Team, int>> _teamRepository;
            private Mock<ICurrentUser> _currentUser;
            private IManageTeamsService _manageTeamsService;

            [SetUp]
            public void Setup()
            {
                _currentUser = new Mock<ICurrentUser>();
            _teamRepository = new Mock<IRepository<Data.Models.Team, Models.Team, int>>();
                _manageTeamsService = new ManageTeamsService(_currentUser.Object, _teamRepository.Object);
            }

            [Test]
            public async System.Threading.Tasks.Task RemoveAsync_ManageTeamsServiceReturnsTrue_ReturnsTrue()
            {
                // Arrange
                const string teamOwner = "1234";
                const int teamId = 1;
                var teams = new List<Models.Team>
                {
                    new Models.Team { Id= 1, TeamOwner = "1234", TeamName = "First_Team"},
                    new Models.Team { Id= 2, TeamOwner = "1234", TeamName = "Second_Team"}
                };
                var user = new Mock<UserDetails>(null);
                user.Setup(x => x.Id()).Returns(teamOwner);
                _currentUser.SetupGet(x => x.Current).Returns(user.Object);
                var mock = teams.AsQueryable().BuildMock();
                _teamRepository.Setup(x => x.GetAllAsync())
                    .Returns(System.Threading.Tasks.Task.FromResult(teams.AsEnumerable()));
                _teamRepository.Setup(x => x.DeleteAsync(It.IsAny<int>()))
                    .ReturnsAsync(true);

                //Act
                var result = await _manageTeamsService.RemoveAsync(teamId);
              
                //Assert
                Assert.IsTrue(result); 
            }

            [Test]
            public async System.Threading.Tasks.Task RemoveAsync_ManageTeamsServiceReturnsFalse_ReturnsFalse()
            {
                // Arrange
                const string teamOwner = "1234";
                const int teamId1 = 4;
                const int teamId2 = 3;
                var teams = new List<Models.Team>
                {
                    new Models.Team 
                    {
                        Id= 1, 
                        TeamOwner = "1234",
                        TeamName = "First_Team"
                    },

                    new Models.Team 
                    { 
                        Id= 2, 
                        TeamOwner = "1234", 
                        TeamName = "Second_Team"
                    },

                    new Models.Team 
                    { 
                        Id= 3,
                        TeamOwner = "4152", 
                        TeamName = "Third_Team"
                    },
                };
                var user = new Mock<UserDetails>(null);
                user.Setup(x => x.Id()).Returns(teamOwner);
                _currentUser.SetupGet(x => x.Current).Returns(user.Object);
                var mock = teams.AsQueryable().BuildMock();
                _teamRepository.Setup(x => x.GetAllAsync())
                    .Returns(System.Threading.Tasks.Task.FromResult(teams.AsEnumerable()));
                _teamRepository.Setup(x => x.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

                //Act
                var result1 = await _manageTeamsService.RemoveAsync(teamId1);
                var result2 = await _manageTeamsService.RemoveAsync(teamId2);

                //Assert
                Assert.IsFalse(result1);
                Assert.IsFalse(result2);
            }

    }
    
}




