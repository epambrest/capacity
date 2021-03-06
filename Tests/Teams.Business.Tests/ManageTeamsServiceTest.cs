using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Teams.Business.Models;
using Teams.Business.Repository;
using Teams.Business.Services;
using Teams.Business.Security;

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
            var dbMock = new List<Team>();
            _teamRepository.Setup(x => x.GetAllAsync())
                .Returns(System.Threading.Tasks.Task.FromResult(GetFakeDbTeams()));
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
            var teams = GetFakeDbTeams();

            var mock = teams.AsQueryable().BuildMock();
            _teamRepository.Setup(x => x.GetAllAsync()).Returns(System.Threading.Tasks.Task.FromResult(teams.AsEnumerable()));

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

            Team team = Team.Create(3, "def-abc", "Team3", new List<TeamMember>() {
                    TeamMember.Create(3, 3, "asf-fgv", User.Create("1234", "vasya@mail.ru", "vasya", "ivanov"))});

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
            Team team = Team.Create(4, "abc-def", "Team4", new List<TeamMember>() {
                    TeamMember.Create(4, 4, "abc-def", User.Create("1234", "vasya@mail.ru", "vasya", "ivanov"))});

            var mock = GetFakeDbTeams().AsQueryable().BuildMock();
            _teamRepository.Setup(x => x.GetAllAsync()).Returns(System.Threading.Tasks.Task.FromResult(GetFakeDbTeams()));
            _teamRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(team);
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
            Team team = Team.Create(4, "abc-def", "Team4", new List<TeamMember>() {
                    TeamMember.Create(4, 4, "abc-def", User.Create("1234", "vasya@mail.ru", "vasya", "ivanov"))});

            var mock = GetFakeDbTeams().AsQueryable().BuildMock();
            _teamRepository.Setup(x => x.GetAllAsync())
                .Returns(System.Threading.Tasks.Task.FromResult(GetFakeDbTeams()));
            _teamRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(team);
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
            Team team = Team.Create(4, "abc-def", "Team4", new List<TeamMember>() {
                    TeamMember.Create(4, 4, "abc-def", User.Create("1234", "vasya@mail.ru", "vasya", "ivanov"))});

            var mock = GetFakeDbTeams().AsQueryable().BuildMock();
            _teamRepository.Setup(x => x.GetAllAsync())
                .Returns(System.Threading.Tasks.Task.FromResult(GetFakeDbTeams()));
            _teamRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(team);
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

       
        private IEnumerable<Team> GetFakeDbTeams()
        {
            var teams = new List<Team>
            {
                Team.Create(1, "abc-def", "Team1", new List<TeamMember>() {
                    TeamMember.Create(1, 1, "def-abc", User.Create("1234", "vasya@mail.ru", "vasya", "ivanov"))}),

                Team.Create(2, "def-abc", "Team2", new List<TeamMember>() {
                    TeamMember.Create(2, 2, "abc-def", User.Create("1234", "vasya@mail.ru", "vasya", "ivanov"))}),

                Team.Create(3, "def-abc", "Team3", new List<TeamMember>() {
                    TeamMember.Create(3, 3, "asf-fgv", User.Create("1234", "vasya@mail.ru", "vasya", "ivanov"))}),

                Team.Create(4, "abc-def", "Team4", new List<TeamMember>() {
                    TeamMember.Create(4, 4, "abc-def", User.Create("1234", "vasya@mail.ru", "vasya", "ivanov"))}),

                Team.Create(5, "def-abc", "Team5", new List<TeamMember>() {
                    TeamMember.Create(5, 5, "asf-fgv", User.Create("1234", "vasya@mail.ru", "vasya", "ivanov"))}),

                Team.Create(6, "def-abc", "Team6", new List<TeamMember>() {
                    TeamMember.Create(6, 6, "asf-fgv", User.Create("1234", "vasya@mail.ru", "vasya", "ivanov"))}),

                Team.Create(7, "def-abc", "Team7", new List<TeamMember>() {
                    TeamMember.Create(7, 7, "asf-fgv", User.Create("1234", "vasya@mail.ru", "vasya", "ivanov"))}),

                Team.Create(8, "def-abc", "Team8", new List<TeamMember>() {
                    TeamMember.Create(8, 8, "asf-fgv", User.Create("1234", "vasya@mail.ru", "vasya", "ivanov"))}),

                Team.Create(9, "def-abc", "Team9", new List<TeamMember>() {
                    TeamMember.Create(9, 9, "abc-def", User.Create("1234", "vasya@mail.ru", "vasya", "ivanov"))}),

                Team.Create(10, "def-abc", "Team10", new List<TeamMember>() {
                    TeamMember.Create(10, 10, "asf-fgv", User.Create("1234", "vasya@mail.ru", "vasya", "ivanov"))}),
            };

            return teams;
        }
    }
}
