using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Business.Models;
using Teams.Business.Repository;
using Teams.Business.Services;
using Teams.Security;

namespace Teams.Business.Tests
{
    [TestFixture]
    class AccessCheckServiceTest
    {
        private IAccessCheckService _accessCheckService;

        private Mock<ICurrentUser> _currentUser;

        private Mock<IRepository<TeamBusiness, int>> _teamRepository;

        [SetUp]
        public void Setup()
        {
            _teamRepository = new Mock<IRepository<TeamBusiness, int>>();

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

            var teams = new List<TeamBusiness>
            {
                new TeamBusiness 
                { 
                    Id = 1, 
                    TeamOwner = "abc-def", 
                    TeamName = "Team1", 
                    TeamMembers = new List<TeamMemberBusiness> { new TeamMemberBusiness { MemberId="def-abc", TeamId =1 }}
                },
                
                new TeamBusiness 
                { 
                    Id = 2, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team2", 
                    TeamMembers = new List<TeamMemberBusiness> { new TeamMemberBusiness { MemberId="abc-def", TeamId =2 }}
                },
                
                new TeamBusiness 
                { 
                    Id = 3, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team3", 
                    TeamMembers = new List<TeamMemberBusiness> { new TeamMemberBusiness { MemberId="asf-fgv", TeamId = 3 }}
                },
                
                new TeamBusiness 
                { 
                    Id = 4, 
                    TeamOwner = "abc-def", 
                    TeamName = "Team4", 
                    TeamMembers = new List<TeamMemberBusiness> { new TeamMemberBusiness { MemberId="abc-def", TeamId =4 }}
                },
                
                new TeamBusiness 
                { 
                    Id = 5, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team5", 
                    TeamMembers = new List<TeamMemberBusiness> { new TeamMemberBusiness { MemberId="asf-fgv", TeamId = 5 }}
                },
                
                new TeamBusiness 
                { 
                    Id = 6, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team6", 
                    TeamMembers = new List<TeamMemberBusiness> { new TeamMemberBusiness { MemberId="asf-fgv", TeamId = 6 }}
                },
                
                new TeamBusiness 
                { 
                    Id = 7, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team7", 
                    TeamMembers = new List<TeamMemberBusiness> { new TeamMemberBusiness { MemberId="asf-fgv", TeamId = 7 }}
                },
                
                new TeamBusiness 
                { 
                    Id = 8, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team8", 
                    TeamMembers = new List<TeamMemberBusiness> { new TeamMemberBusiness { MemberId="asf-fgv", TeamId = 8 }}
                },
                
                new TeamBusiness 
                { 
                    Id = 9, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team9", 
                    TeamMembers = new List<TeamMemberBusiness> { new TeamMemberBusiness { MemberId="abc-def", TeamId =9 }}
                },
                
                new TeamBusiness 
                { 
                    Id = 10, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team10", 
                    TeamMembers = new List<TeamMemberBusiness> { new TeamMemberBusiness { MemberId="asf-fgv", TeamId = 10 }}
                }
            };

            var mock = teams.AsQueryable().BuildMock();
            _teamRepository.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(teams.AsEnumerable()));

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

            var teams = new List<TeamBusiness>
            {
                new TeamBusiness 
                { 
                    Id= 1, 
                    TeamOwner = "abc-def", 
                    TeamName = "Team1", 
                    TeamMembers=new List<TeamMemberBusiness> { new TeamMemberBusiness { MemberId="def-abc", TeamId =1 }}
                },

                new TeamBusiness 
                { 
                    Id= 2, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team2", 
                    TeamMembers = new List<TeamMemberBusiness> { new TeamMemberBusiness { MemberId="abc-def", TeamId =2 }}
                },

                new TeamBusiness 
                { 
                    Id= 3, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team3", 
                    TeamMembers = new List<TeamMemberBusiness> { new TeamMemberBusiness { MemberId="asf-fgv", TeamId = 3 }}
                },

                new TeamBusiness 
                { 
                    Id= 4, 
                    TeamOwner = "abc-def", 
                    TeamName = "Team4", 
                    TeamMembers = new List<TeamMemberBusiness> { new TeamMemberBusiness { MemberId="abc-def", TeamId =4 }}
                },

                new TeamBusiness 
                { 
                    Id= 5, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team5", 
                    TeamMembers = new List<TeamMemberBusiness>{ new TeamMemberBusiness { MemberId="asf-fgv", TeamId = 5 }}
                },

                new TeamBusiness 
                { 
                    Id= 6, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team6", 
                    TeamMembers = new List<TeamMemberBusiness> { new TeamMemberBusiness { MemberId="asf-fgv", TeamId = 6 }}
                },

                new TeamBusiness 
                { 
                    Id= 7, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team7", 
                    TeamMembers = new List<TeamMemberBusiness>{ new TeamMemberBusiness { MemberId="asf-fgv", TeamId = 7 }}
                },

                new TeamBusiness 
                { 
                    Id= 8, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team8", 
                    TeamMembers = new List<TeamMemberBusiness>{ new TeamMemberBusiness { MemberId="asf-fgv", TeamId = 8 }}
                },

                new TeamBusiness 
                { 
                    Id = 9, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team9", 
                    TeamMembers = new List<TeamMemberBusiness> { new TeamMemberBusiness { MemberId="abc-def", TeamId =9 }}
                },

                new TeamBusiness 
                { 
                    Id= 10, 
                    TeamOwner = "def-abc", 
                    TeamName = "Team10", 
                    TeamMembers = new List<TeamMemberBusiness>{ new TeamMemberBusiness { MemberId="asf-fgv", TeamId = 10 }}
                }
            };

            var mock = teams.AsQueryable().BuildMock();
            _teamRepository.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(teams.AsEnumerable()));

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
