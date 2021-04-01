using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Teams.Business.Services;
using Teams.Data;
using Teams.Data.Models;
using Teams.Security;

namespace Teams.Business.Tests
{
    [TestFixture]
    class ManageTeamsServiceTest
    {
        private ManageTeamsService _manageTeamsService;
        private Mock <IRepository<Team, int>> _teamRepository;
        private Mock<ICurrentUser> _currentUserMock;

        [SetUp]
        public void Setup()
        {
            _currentUserMock =  new Mock<ICurrentUser>();
            _teamRepository = new Mock<IRepository<Team, int>>();
            var dbMock = new List<Team>().AsQueryable().BuildMock();
            _teamRepository.Setup(x => x.GetAll()).Returns(dbMock.Object);
            _teamRepository.Setup(x => x.InsertAsync(It.IsAny<Team>())).ReturnsAsync(true);
            string ownerId = Guid.NewGuid().ToString();
            var userDetails = new Mock<UserDetails>(null);
            userDetails.Setup(x => x.Id()).Returns(ownerId);
            _currentUserMock.SetupGet(x => x.Current).Returns(userDetails.Object);
            _manageTeamsService = new ManageTeamsService(_currentUserMock.Object, _teamRepository.Object);
        }

        [Test]
        public async System.Threading.Tasks.Task AddTeamAsync_AddTeamWithEmptyName_ReturnsFalse()
        {
            //Arrange
            string teamName = "";
            //Act
            var isValid = await _manageTeamsService.AddTeamAsync(teamName);
            //Assert
            Assert.That(isValid, Is.False);
        }

        [Test]
        public async System.Threading.Tasks.Task AddTeamAsync_AddTeamWithIllegalName_ReturnsFalse()
        {
            //Arrange
            string teamName = "alex wins:";
            //Act
            var isValid = await _manageTeamsService.AddTeamAsync(teamName);
            //Assert
            Assert.That(isValid, Is.False);
        }

        [Test]
        public async System.Threading.Tasks.Task AddTeamAsync_AddTeamWithLegalName_ReturnsTrue()
        {
            //Arrange
            string teamName = "Legal_Team.";
            string teamName2 = "Legal Team";
            //Act
            var isValid = await _manageTeamsService.AddTeamAsync(teamName);
            var result2 = await _manageTeamsService.AddTeamAsync(teamName2);
            //Assert
            Assert.That(isValid, Is.True);
            Assert.IsTrue(result2);
        }

        [Test]
        public async System.Threading.Tasks.Task AddTeamAsync_AddTeamWithExistingName_ReturnsFalseAsync()
        {
            //Arrange
            string teamName = "Su_per-Team.,";
            await _manageTeamsService.AddTeamAsync(teamName);
            var isValid = await _manageTeamsService.AddTeamAsync(teamName);
            //Assert
            Assert.That(isValid, Is.False);
        }
        
        [Test]
        public async System.Threading.Tasks.Task GetMyTeamsAsync_ManageTeamsServiceReturnsListCount4_ListCount4()
        {
            //Arrange
            const string id = "abc-def";
            var teams = new List<Team>
            {
                new Team { Id= 1, TeamOwner = "abc-def", TeamName = "Team1", TeamMembers=new List<TeamMember>{new TeamMember {MemberId="def-abc", TeamId =1}}},
                new Team { Id= 2, TeamOwner = "def-abc", TeamName = "Team2", TeamMembers=new List<TeamMember>{new TeamMember {MemberId="abc-def", TeamId =2}}},
                new Team { Id= 3, TeamOwner = "def-abc", TeamName = "Team3", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv"}}},
                new Team { Id= 4, TeamOwner = "abc-def", TeamName = "Team4", TeamMembers=new List<TeamMember>{new TeamMember {MemberId="abc-def", TeamId =4}}},
                new Team { Id= 5, TeamOwner = "def-abc", TeamName = "Team5", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv"}}},
                new Team { Id= 6, TeamOwner = "def-abc", TeamName = "Team6", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv"}}},
                new Team { Id= 7, TeamOwner = "def-abc", TeamName = "Team7", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv"}}},
                new Team { Id= 8, TeamOwner = "def-abc", TeamName = "Team8", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv"}}},
                new Team { Id= 9, TeamOwner = "def-abc", TeamName = "Team9", TeamMembers=new List<TeamMember>{new TeamMember {MemberId="abc-def", TeamId =9}}},
                new Team { Id= 10, TeamOwner = "def-abc", TeamName = "Team10", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv"}}}
            };

            var mock = teams.AsQueryable().BuildMock();
            _teamRepository.Setup(x => x.GetAll()).Returns(mock.Object);

            var ud = new Mock<UserDetails>(null);
            ud.Setup(x => x.Id()).Returns(id);
            ud.Setup(x => x.Name()).Returns("name");
            _currentUserMock.SetupGet(x => x.Current).Returns(ud.Object);

            //Act
            var result = new List<Team>(await _manageTeamsService.GetMyTeamsAsync());

            //Assert
            Assert.AreEqual(4, result.Count());
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual(4, result[1].Id);
            Assert.AreEqual(2, result[2].Id);
            Assert.AreEqual(9, result[3].Id);
        }

        [Test]
        public async System.Threading.Tasks.Task GetTeamAsync_ManageTeamsServiceReturnsTeam_Team()
        {
            //Arrange
            const string id = "abc-def";
            const int teamId = 3;
            Team team = new Team { Id= 3, TeamOwner = "def-abc", TeamName = "Team3", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv"}}};

            _teamRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(team);

            var ud = new Mock<UserDetails>(null);
            ud.Setup(x => x.Id()).Returns(id);
            ud.Setup(x => x.Name()).Returns("name");
            _currentUserMock.SetupGet(x => x.Current).Returns(ud.Object);

            //Act
            var result = await _manageTeamsService.GetTeamAsync(teamId);

            //Assert
            Assert.AreEqual(3, result.Id);
        }

        [Test]
        public async System.Threading.Tasks.Task EditTeamNameAsync_ManageTeamsServiceReturnsTrue_True()
        {
            //Arrange
            const string id = "abc-def";
            const int teamId = 4;
            const string teamName = "NewName";
            const string teamName2 = "New Name_Team-4";

            var mock = GetFakeDbTeams().AsQueryable().BuildMock();
            _teamRepository.Setup(x => x.GetAll()).Returns(mock.Object);
            _teamRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Team { Id = 4, TeamOwner = "abc-def", TeamName = "Team4", TeamMembers = new List<TeamMember> { new TeamMember { MemberId = "abc-def", TeamId = 4 }}});
            _teamRepository.Setup(x => x.UpdateAsync(It.IsAny<Team>())).ReturnsAsync(true);

            var ud = new Mock<UserDetails>(null);
            ud.Setup(x => x.Id()).Returns(id);
            ud.Setup(x => x.Name()).Returns("name");
            _currentUserMock.SetupGet(x => x.Current).Returns(ud.Object);

            //Act
            var result = await _manageTeamsService.EditTeamNameAsync(teamId, teamName);
            var result2 = await _manageTeamsService.EditTeamNameAsync(teamId, teamName2);

            //Assert
            Assert.IsTrue(result);
            Assert.IsTrue(result2);
        }

        [Test]
        public async System.Threading.Tasks.Task EditTeamNameAsync_ManageTeamsServiceReturnsFalse1_False1()
        {
            //Arrange
            const string id = "abc-def";
            const int teamId = 4;
            const string existTeamName = "Team5";
            const string errorTeamName = "ERR##$$OR";
            const string errorTeamName2 = "Team  Error";

            var mock = GetFakeDbTeams().AsQueryable().BuildMock();
            _teamRepository.Setup(x => x.GetAll()).Returns(mock.Object);
            _teamRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Team { Id = 4, TeamOwner = "abc-def", TeamName = "Team4", TeamMembers = new List<TeamMember> { new TeamMember { MemberId = "abc-def", TeamId = 4 } } });
            _teamRepository.Setup(x => x.UpdateAsync(It.IsAny<Team>())).ReturnsAsync(true);

            var ud = new Mock<UserDetails>(null);
            ud.Setup(x => x.Id()).Returns(id);
            ud.Setup(x => x.Name()).Returns("name");
            _currentUserMock.SetupGet(x => x.Current).Returns(ud.Object);

            //Act
            var result1 = await _manageTeamsService.EditTeamNameAsync(teamId, existTeamName);         //team with the same name already exists 
            var result2 = await _manageTeamsService.EditTeamNameAsync(teamId, errorTeamName);         //team name contains invalid characters
            var result3 = await _manageTeamsService.EditTeamNameAsync(teamId, errorTeamName2);        

            //Assert
            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
            Assert.IsFalse(result3);
        }

        [Test]
        public async System.Threading.Tasks.Task EditTeamNameAsync_ManageTeamsServiceReturnsFalse2_False2()
        {
            //Arrange
            const string id = "not-owner";
            const int teamId = 4;
            const string teamName = "NewName";

            var mock = GetFakeDbTeams().AsQueryable().BuildMock();
            _teamRepository.Setup(x => x.GetAll()).Returns(mock.Object);
            _teamRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Team { Id = 4, TeamOwner = "abc-def", TeamName = "Team4", TeamMembers = new List<TeamMember> { new TeamMember { MemberId = "abc-def", TeamId = 4 } } });
            _teamRepository.Setup(x => x.UpdateAsync(It.IsAny<Team>())).ReturnsAsync(true);

            var ud = new Mock<UserDetails>(null);
            ud.Setup(x => x.Id()).Returns(id);
            ud.Setup(x => x.Name()).Returns("name");
            _currentUserMock.SetupGet(x => x.Current).Returns(ud.Object);

            //Act
            var result = await _manageTeamsService.EditTeamNameAsync(teamId, teamName);        //current user is not a team owner

            //Assert
            Assert.IsFalse(result);
        }

       
        private List<Team> GetFakeDbTeams()
        {
            var teams = new List<Team>
            {
                new Team { Id= 1, TeamOwner = "abc-def", TeamName = "Team1", TeamMembers=new List<TeamMember>{new TeamMember {MemberId="def-abc", TeamId =1}}},
                new Team { Id= 2, TeamOwner = "def-abc", TeamName = "Team2", TeamMembers=new List<TeamMember>{new TeamMember {MemberId="abc-def", TeamId =2}}},
                new Team { Id= 3, TeamOwner = "def-abc", TeamName = "Team3", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv"}}},
                new Team { Id= 4, TeamOwner = "abc-def", TeamName = "Team4", TeamMembers=new List<TeamMember>{new TeamMember {MemberId="abc-def", TeamId =4}}},
                new Team { Id= 5, TeamOwner = "def-abc", TeamName = "Team5", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv"}}},
                new Team { Id= 6, TeamOwner = "def-abc", TeamName = "Team6", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv"}}},
                new Team { Id= 7, TeamOwner = "def-abc", TeamName = "Team7", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv"}}},
                new Team { Id= 8, TeamOwner = "def-abc", TeamName = "Team8", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv"}}},
                new Team { Id= 9, TeamOwner = "def-abc", TeamName = "Team9", TeamMembers=new List<TeamMember>{new TeamMember {MemberId="abc-def", TeamId =9}}},
                new Team { Id= 10, TeamOwner = "def-abc", TeamName = "Team10", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv"}}}
            };

            return teams;
        }
    }
}
