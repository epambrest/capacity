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
    class AccessCheckServiceTest
    {
        private IAccessCheckService _accessCheckService;

        private Mock<ICurrentUser> _currentUser;

        private Mock<IRepository<Team, int>> _teamRepository;

        [SetUp]
        public void Setup()
        {
            _teamRepository = new Mock<IRepository<Team, int>>();

            _currentUser = new Mock<ICurrentUser>();

            _accessCheckService = new AccessCheckService(_currentUser.Object, _teamRepository.Object);

        }

        [Test]
        public async Task OwnerOrMemberAsync_FccessCheckServiceReturnsTrue_ReturnsTrue()
        {
            //Arrange
            const int team_id1 = 1;
            const int team_id2 = 2;
            const int team_id3 = 3;
            const string id = "abc-def";

            var teams = new List<Team>
            {
                new Team { Id= 1, TeamOwner = "abc-def", TeamName = "Team1", TeamMembers=new List<TeamMember>{ new TeamMember {MemberId="def-abc", TeamId =1 }}},
                new Team { Id= 2, TeamOwner = "def-abc", TeamName = "Team2", TeamMembers=new List<TeamMember>{ new TeamMember {MemberId="abc-def", TeamId =2 }}},
                new Team { Id= 3, TeamOwner = "def-abc", TeamName = "Team3", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv", TeamId = 3 }}},
                new Team { Id= 4, TeamOwner = "abc-def", TeamName = "Team4", TeamMembers=new List<TeamMember>{ new TeamMember {MemberId="abc-def", TeamId =4 }}},
                new Team { Id= 5, TeamOwner = "def-abc", TeamName = "Team5", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv", TeamId = 5 }}},
                new Team { Id= 6, TeamOwner = "def-abc", TeamName = "Team6", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv", TeamId = 6 }}},
                new Team { Id= 7, TeamOwner = "def-abc", TeamName = "Team7", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv", TeamId = 7 }}},
                new Team { Id= 8, TeamOwner = "def-abc", TeamName = "Team8", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv", TeamId = 8 }}},
                new Team { Id= 9, TeamOwner = "def-abc", TeamName = "Team9", TeamMembers=new List<TeamMember>{ new TeamMember {MemberId="abc-def", TeamId =9 }}},
                new Team { Id= 10, TeamOwner = "def-abc", TeamName = "Team10", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv", TeamId = 10 }}}
            };

            var mock = teams.AsQueryable().BuildMock();
            _teamRepository.Setup(x => x.GetAll()).Returns(mock.Object);

            var ud = new Mock<UserDetails>(null);
            ud.Setup(x => x.Id()).Returns(id);
            ud.Setup(x => x.Name()).Returns("name");
            _currentUser.SetupGet(x => x.Current).Returns(ud.Object);

            //Act
            var result1 =await _accessCheckService.OwnerOrMemberAsync(team_id1);        //Owner
            var result2 =await _accessCheckService.OwnerOrMemberAsync(team_id2);       //Member
            var result3 =await _accessCheckService.OwnerOrMemberAsync(team_id3);       //Not Owner or member

            //Assert
            Assert.AreEqual(true, result1);
            Assert.AreEqual(true, result2);
            Assert.AreEqual(false, result3);
        }

        [Test]
        public async Task IsOwnerAsync_AccessCheckServiceReturnsTrue_ReturnsTrue()
        {
            //Arrange
            const int team_id1 = 1;
            const int team_id2 = 3;
            const string id = "abc-def";

            var teams = new List<Team>
            {
                new Team { Id= 1, TeamOwner = "abc-def", TeamName = "Team1", TeamMembers=new List<TeamMember>{ new TeamMember {MemberId="def-abc", TeamId =1 }}},
                new Team { Id= 2, TeamOwner = "def-abc", TeamName = "Team2", TeamMembers=new List<TeamMember>{ new TeamMember {MemberId="abc-def", TeamId =2 }}},
                new Team { Id= 3, TeamOwner = "def-abc", TeamName = "Team3", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv", TeamId = 3 }}},
                new Team { Id= 4, TeamOwner = "abc-def", TeamName = "Team4", TeamMembers=new List<TeamMember>{ new TeamMember {MemberId="abc-def", TeamId =4 }}},
                new Team { Id= 5, TeamOwner = "def-abc", TeamName = "Team5", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv", TeamId = 5 }}},
                new Team { Id= 6, TeamOwner = "def-abc", TeamName = "Team6", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv", TeamId = 6 }}},
                new Team { Id= 7, TeamOwner = "def-abc", TeamName = "Team7", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv", TeamId = 7 }}},
                new Team { Id= 8, TeamOwner = "def-abc", TeamName = "Team8", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv", TeamId = 8 }}},
                new Team { Id= 9, TeamOwner = "def-abc", TeamName = "Team9", TeamMembers=new List<TeamMember>{ new TeamMember {MemberId="abc-def", TeamId =9 }}},
                new Team { Id= 10, TeamOwner = "def-abc", TeamName = "Team10", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv", TeamId = 10 }}}
            };

            var mock = teams.AsQueryable().BuildMock();
            _teamRepository.Setup(x => x.GetAll()).Returns(mock.Object);

            var ud = new Mock<UserDetails>(null);
            ud.Setup(x => x.Id()).Returns(id);
            ud.Setup(x => x.Name()).Returns("name");
            _currentUser.SetupGet(x => x.Current).Returns(ud.Object);

            //Act
            var result1 = await _accessCheckService.IsOwnerAsync(team_id1);        //Owner
            var result2 = await _accessCheckService.IsOwnerAsync(team_id2);       //Not Owner

            //Assert
            Assert.AreEqual(true, result1);
            Assert.AreEqual(false, result2);
        }
    }
}
