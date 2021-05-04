using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Teams.Business.Models;
using Teams.Business.Repository;
using Teams.Business.Services;
using Teams.Business.Security;

namespace Teams.Business.Tests
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
        public async System.Threading.Tasks.Task OwnerOrMemberAsync_FccessCheckServiceReturnsTrue_ReturnsTrue()
        {
            //Arrange
            const int teamId1 = 1;
            const int teamId2 = 2;
            const int teamId3 = 3;
            const string id = "abc-def";

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
