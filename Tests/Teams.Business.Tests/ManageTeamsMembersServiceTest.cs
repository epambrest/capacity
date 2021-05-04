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
    class ManageTeamsMembersServiceTest
    {
        private IManageTeamsMembersService _manageTeamsMembersService;

        private Mock<IRepository<TeamMember, int>> _teamMemberRepository;

        private Mock<IRepository<Team, int>> _teamRepository;

        private Mock<ICurrentUser> _currentUserMock;

        [SetUp]
        public void Setup()
        {
            _currentUserMock = new Mock<ICurrentUser>();
            _teamMemberRepository = new Mock<IRepository<TeamMember, int>>();
            _teamRepository = new Mock<IRepository<Team, int>>();
            _manageTeamsMembersService = new ManageTeamsMembersService(_teamRepository.Object, _teamMemberRepository.Object, _currentUserMock.Object);
        }

        [Test]
        public async System.Threading.Tasks.Task GetMembers_ManageTeamsMembersServiceReturnTeamMember_ReturnTeamMember()
        {
            //Arrange
            const int teamId = 4;
            const string memberId = "def-abc";

            var mock = GetFakeDbTeamMembers().AsQueryable().BuildMock();
            _teamMemberRepository.Setup(x => x.GetAllAsync())
                .Returns(System.Threading.Tasks.Task.FromResult(GetFakeDbTeamMembers()));

            //Act
            var result = await  _manageTeamsMembersService.GetMemberAsync(teamId, memberId);

            //Assert
            Assert.AreEqual(result.TeamId, teamId);
            Assert.AreEqual(result.MemberId, memberId);
        }

        [Test]
        public async System.Threading.Tasks.Task GetMembers_ManageTeamsMembersServiceReturnNull_ReturnNull()     //Team or member not exist
        {
            //Arrange
            const int teamId = 4;
            const int teamIdError = 99;
            const string memberId = "def-abc";
            const string memberIdError = "eror-id";

            var mock = GetFakeDbTeamMembers().AsQueryable().BuildMock();
            _teamMemberRepository.Setup(x => x.GetAllAsync())
                .Returns(System.Threading.Tasks.Task.FromResult(GetFakeDbTeamMembers()));

            //Act
            var result1 = await _manageTeamsMembersService.GetMemberAsync(teamId, memberIdError);
            var result2 = await _manageTeamsMembersService.GetMemberAsync(teamIdError, memberId);

            //Assert
            Assert.AreEqual(result1, null);
            Assert.AreEqual(result2, null);
        }

        [Test]
        public async System.Threading.Tasks.Task GetAllTeamMembersAsync_ManageTeamsMembersServiceReturnList_ReturnList()
        {
            //Arrange
            const int teamId = 4;

            var mock = GetFakeDbTeamMembers().AsQueryable().BuildMock();
            _teamMemberRepository.Setup(x => x.GetAllAsync())
                .Returns(System.Threading.Tasks.Task.FromResult(GetFakeDbTeamMembers()));

            //Act
            var result = await _manageTeamsMembersService.GetAllTeamMembersAsync(teamId, new DisplayOptions { });

            //Assert
            Assert.AreEqual(result[0].Id, 5);
            Assert.AreEqual(result[1].Id, 4);
        }

        [Test]
        public async System.Threading.Tasks.Task GetAllTeamMembersAsync_ManageTeamsMembersServiceReturnNull_ReturnNull()
        {
            //Arrange
            const int teamId = 40;

            var mock = GetFakeDbTeamMembers().AsQueryable().BuildMock();
            _teamMemberRepository.Setup(x => x.GetAllAsync())
                .Returns(System.Threading.Tasks.Task.FromResult(GetFakeDbTeamMembers()));

            //Act
            var result = await _manageTeamsMembersService.GetAllTeamMembersAsync(teamId, new DisplayOptions { });

            //Assert
            Assert.AreEqual(result.Count, 0);
        }

        private IEnumerable<TeamMember> GetFakeDbTeamMembers()
        {
            var teamMembers = new List<TeamMember>
            {
                TeamMember.Create(1, 1, "def-abc", User.Create("def-abc", "vasya@mail.ru", "vasya", "ivanov")),
                TeamMember.Create(2, 2, "abc-def", User.Create("abc-def", "vasya@mail.ru", "vasya", "ivanov")),
                TeamMember.Create(3, 3, "asf-fgv", User.Create("asf-fgv", "vasya@mail.ru", "vasya", "ivanov")),
                TeamMember.Create(4, 4, "def-abc", User.Create("def-abc", "b", "vasya", "ivanov")),
                TeamMember.Create(5, 4, "abc-def", User.Create("abc-def", "a", "vasya", "ivanov")),
                TeamMember.Create(6, 5, "asf-fgv", User.Create("asf-fgv", "vasya@mail.ru", "vasya", "ivanov")),
                TeamMember.Create(7, 6, "asf-fgv", User.Create("asf-fgv", "vasya@mail.ru", "vasya", "ivanov")),
                TeamMember.Create(8, 7, "asf-fgv", User.Create("asf-fgv", "vasya@mail.ru", "vasya", "ivanov")),
                TeamMember.Create(9, 8, "asf-fgv", User.Create("asf-fgv", "vasya@mail.ru", "vasya", "ivanov")),
                TeamMember.Create(10, 9, "abc-def", User.Create("abc-def", "vasya@mail.ru", "vasya", "ivanov")),
                TeamMember.Create(11, 10, "asf-fgv", User.Create("asf-fgv", "vasya@mail.ru", "vasya", "ivanov")),
            };

            return teamMembers;
        }
    }
}
