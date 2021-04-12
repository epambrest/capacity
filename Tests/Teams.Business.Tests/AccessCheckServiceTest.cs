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
    class AccessCheckServiceTest
    {
        private IAccessCheckService _accessCheckService;

        private Mock<ICurrentUser> _currentUser;

        private Mock<IRepository<Data.Models.Team, Models.Team, int>> _teamRepository;

        [SetUp]
        public void Setup()
        {
            _teamRepository = new Mock<IRepository<Data.Models.Team, Models.Team, int>>();

            _currentUser = new Mock<ICurrentUser>();

            _accessCheckService = new AccessCheckService(_currentUser.Object, _teamRepository.Object);

        }

        [Test]
        public async System.Threading.Tasks.Task OwnerOrMemberAsync_FccessCheckServiceReturnsTrue_ReturnsTrue()
        {
            //Arrange
            const int teamId1 = 1;
            const int teamId2 = 2;
            const int teamId3 = 3;
            const string id = "abc-def";

            var teams = new List<Models.Team>
            {
                new Models.Team 
                { 
                    Id = 1, 
                    TeamOwner = "abc-def", 
                    TeamName = "Team1", 
                    TeamMembers = new List<Models.TeamMember> { new Models.TeamMember { MemberId="def-abc", TeamId =1 }}
                },
                
                new Models.Team 
                { 
                    Id = 2, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team2", 
                    TeamMembers = new List<Models.TeamMember> { new Models.TeamMember { MemberId="abc-def", TeamId =2 }}
                },
                
                new Models.Team 
                { 
                    Id = 3, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team3", 
                    TeamMembers = new List<Models.TeamMember> { new Models.TeamMember { MemberId="asf-fgv", TeamId = 3 }}
                },
                
                new Models.Team 
                { 
                    Id = 4, 
                    TeamOwner = "abc-def", 
                    TeamName = "Team4", 
                    TeamMembers = new List<Models.TeamMember> { new Models.TeamMember { MemberId="abc-def", TeamId =4 }}
                },
                
                new Models.Team 
                { 
                    Id = 5, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team5", 
                    TeamMembers = new List<Models.TeamMember> { new Models.TeamMember { MemberId="asf-fgv", TeamId = 5 }}
                },
                
                new Models.Team 
                { 
                    Id = 6, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team6", 
                    TeamMembers = new List<Models.TeamMember> { new Models.TeamMember { MemberId="asf-fgv", TeamId = 6 }}
                },
                
                new Models.Team 
                { 
                    Id = 7, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team7", 
                    TeamMembers = new List<Models.TeamMember> { new Models.TeamMember { MemberId="asf-fgv", TeamId = 7 }}
                },
                
                new Models.Team 
                { 
                    Id = 8, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team8", 
                    TeamMembers = new List<Models.TeamMember> { new Models.TeamMember { MemberId="asf-fgv", TeamId = 8 }}
                },
                
                new Models.Team 
                { 
                    Id = 9, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team9", 
                    TeamMembers = new List<Models.TeamMember> { new Models.TeamMember { MemberId="abc-def", TeamId =9 }}
                },
                
                new Models.Team 
                { 
                    Id = 10, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team10", 
                    TeamMembers = new List<Models.TeamMember> { new Models.TeamMember { MemberId="asf-fgv", TeamId = 10 }}
                }
            };

            var mock = teams.AsQueryable().BuildMock();
            _teamRepository.Setup(x => x.GetAllAsync())
                .Returns(System.Threading.Tasks.Task.FromResult(teams.AsEnumerable()));

            var ud = new Mock<UserDetails>(null);
            ud.Setup(x => x.Id()).Returns(id);
            ud.Setup(x => x.Name()).Returns("name");
            _currentUser.SetupGet(x => x.Current).Returns(ud.Object);

            //Act
            var result1 =await _accessCheckService.OwnerOrMemberAsync(teamId1);        //Owner
            var result2 =await _accessCheckService.OwnerOrMemberAsync(teamId2);       //Member
            var result3 =await _accessCheckService.OwnerOrMemberAsync(teamId3);       //Not Owner or member

            //Assert
            Assert.AreEqual(true, result1);
            Assert.AreEqual(true, result2);
            Assert.AreEqual(false, result3);
        }

        [Test]
        public async System.Threading.Tasks.Task IsOwnerAsync_AccessCheckServiceReturnsTrue_ReturnsTrue()
        {
            //Arrange
            const int teamId1 = 1;
            const int teamId2 = 3;
            const string id = "abc-def";

            var teams = new List<Models.Team>
            {
                new Models.Team 
                { 
                    Id= 1, 
                    TeamOwner = "abc-def", 
                    TeamName = "Team1", 
                    TeamMembers=new List<Models.TeamMember> { new Models.TeamMember { MemberId="def-abc", TeamId =1 }}
                },

                new Models.Team 
                { 
                    Id= 2, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team2", 
                    TeamMembers = new List<Models.TeamMember> { new Models.TeamMember { MemberId="abc-def", TeamId =2 }}
                },

                new Models.Team 
                { 
                    Id= 3, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team3", 
                    TeamMembers = new List<Models.TeamMember> { new Models.TeamMember { MemberId="asf-fgv", TeamId = 3 }}
                },

                new Models.Team 
                { 
                    Id= 4, 
                    TeamOwner = "abc-def", 
                    TeamName = "Team4", 
                    TeamMembers = new List<Models.TeamMember> { new Models.TeamMember { MemberId="abc-def", TeamId =4 }}
                },

                new Models.Team 
                { 
                    Id= 5, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team5", 
                    TeamMembers = new List<Models.TeamMember>{ new Models.TeamMember { MemberId="asf-fgv", TeamId = 5 }}
                },

                new Models.Team 
                { 
                    Id= 6, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team6", 
                    TeamMembers = new List<Models.TeamMember> { new Models.TeamMember { MemberId="asf-fgv", TeamId = 6 }}
                },

                new Models.Team 
                { 
                    Id= 7, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team7", 
                    TeamMembers = new List<Models.TeamMember>{ new Models.TeamMember { MemberId="asf-fgv", TeamId = 7 }}
                },

                new Models.Team 
                { 
                    Id= 8, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team8", 
                    TeamMembers = new List<Models.TeamMember>{ new Models.TeamMember { MemberId="asf-fgv", TeamId = 8 }}
                },

                new Models.Team 
                { 
                    Id = 9, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team9", 
                    TeamMembers = new List<Models.TeamMember> { new Models.TeamMember { MemberId="abc-def", TeamId =9 }}
                },

                new Models.Team 
                { 
                    Id= 10, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team10", 
                    TeamMembers = new List<Models.TeamMember>{ new Models.TeamMember { MemberId="asf-fgv", TeamId = 10 }}
                }
            };

            var mock = teams.AsQueryable().BuildMock();
            _teamRepository.Setup(x => x.GetAllAsync())
                .Returns(System.Threading.Tasks.Task.FromResult(teams.AsEnumerable()));

            var ud = new Mock<UserDetails>(null);
            ud.Setup(x => x.Id()).Returns(id);
            ud.Setup(x => x.Name()).Returns("name");
            _currentUser.SetupGet(x => x.Current).Returns(ud.Object);
                
            //Act
            var result1 = await _accessCheckService.IsOwnerAsync(teamId1);        //Owner
            var result2 = await _accessCheckService.IsOwnerAsync(teamId2);       //Not Owner

            //Assert>
            Assert.AreEqual(true, result1);
            Assert.AreEqual(false, result2);
        }
    }
}
