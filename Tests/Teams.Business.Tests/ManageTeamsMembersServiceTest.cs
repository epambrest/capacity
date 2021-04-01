using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Teams.Business.Services;
using Teams.Data;
using Teams.Data.Models;
using Teams.Security;

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
            _teamMemberRepository.Setup(x => x.GetAll()).Returns(mock.Object);

            //Act
            var result =await  _manageTeamsMembersService.GetMemberAsync(teamId, memberId);

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
            _teamMemberRepository.Setup(x => x.GetAll()).Returns(mock.Object);

            //Act
            var result1 =await _manageTeamsMembersService.GetMemberAsync(teamId, memberIdError);
            var result2 =await _manageTeamsMembersService.GetMemberAsync(teamIdError, memberId);

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
            _teamMemberRepository.Setup(x => x.GetAll()).Returns(mock.Object);

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
            _teamMemberRepository.Setup(x => x.GetAll()).Returns(mock.Object);

            //Act
            var result = await _manageTeamsMembersService.GetAllTeamMembersAsync(teamId, new DisplayOptions { });

            //Assert
            Assert.AreEqual(result.Count, 0);
        }

        private List<TeamMember> GetFakeDbTeamMembers()
        {
            var teamMembers = new List<TeamMember>
            {
                new TeamMember {Id =1, MemberId="def-abc", TeamId =1},
                new TeamMember {Id =2, MemberId="abc-def", TeamId =2},
                new TeamMember {Id =3, MemberId="asf-fgv", TeamId =3},
                new TeamMember {Id =4, MemberId="def-abc", Member= new User{ UserName = "b" }, TeamId =4},
                new TeamMember {Id =5, MemberId="abc-def", Member= new User{ UserName = "a" }, TeamId =4},
                new TeamMember {Id =6, MemberId="asf-fgv", TeamId =5},
                new TeamMember {Id =7, MemberId="asf-fgv", TeamId =6},
                new TeamMember {Id =8, MemberId="asf-fgv", TeamId =7},
                new TeamMember {Id =9, MemberId="asf-fgv", TeamId =8},
                new TeamMember {Id =10, MemberId="abc-def", TeamId =9},
                new TeamMember {Id =11, MemberId="asf-fgv", TeamId =10}
            };

            return teamMembers;
        }
    }
}
