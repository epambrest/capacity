using Microsoft.AspNetCore.Http;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Teams.Data;
using Teams.Models;
using Teams.Security;
using Teams.Services;

namespace Teams.Tests
{
    class ManageTeamsMembersServiceTest
    {
        public IManageTeamsMembersService _manageTeamsMembersService;

        public Mock<IRepository<TeamMember, int>> _teamMemberRepository;

        [SetUp]
        public void Setup()
        {
            _teamMemberRepository = new Mock<IRepository<TeamMember, int>>();

            _manageTeamsMembersService = new ManageTeamsMembersService(_teamMemberRepository.Object);
        }

        [Test]
        public async Task GetMembers_ManageTeamsMembersServiceReturnTeamMember_ReturnTeamMember()
        {
            //Arrange
            const int team_id = 4;
            const string member_id = "def-abc";

            var mock = GetFakeDbTeamMembers().AsQueryable().BuildMock();
            _teamMemberRepository.Setup(x => x.GetAll()).Returns(mock.Object);

            //Act
            var result =await  _manageTeamsMembersService.GetMemberAsync(team_id, member_id);

            //Assert
            Assert.AreEqual(result.TeamId, team_id);
            Assert.AreEqual(result.MemberId, member_id);
        }

        [Test]
        public async Task GetMembers_ManageTeamsMembersServiceReturnNull_ReturnNull()     //Team or member not exist
        {
            //Arrange
            const int team_id = 4;
            const int team_id_eror = 99;
            const string member_id = "def-abc";
            const string member_id_eror = "eror-id";

            var mock = GetFakeDbTeamMembers().AsQueryable().BuildMock();
            _teamMemberRepository.Setup(x => x.GetAll()).Returns(mock.Object);

            //Act
            var result1 =await _manageTeamsMembersService.GetMemberAsync(team_id, member_id_eror);
            var result2 =await _manageTeamsMembersService.GetMemberAsync(team_id_eror, member_id);

            //Assert
            Assert.AreEqual(result1, null);
            Assert.AreEqual(result2, null);
        }

        [Test]
        public async Task GetAllTeamMembersAsync_ManageTeamsMembersServiceReturnList_ReturnList()
        {
            //Arrange
            const int team_id = 4;

            var mock = GetFakeDbTeamMembers().AsQueryable().BuildMock();
            _teamMemberRepository.Setup(x => x.GetAll()).Returns(mock.Object);

            //Act
            var result = await _manageTeamsMembersService.GetAllTeamMembersAsync(team_id, new DisplayOptions { });

            //Assert
            Assert.AreEqual(result[0].Id, 5);
            Assert.AreEqual(result[1].Id, 4);
        }

        [Test]
        public async Task GetAllTeamMembersAsync_ManageTeamsMembersServiceReturnNull_ReturnNull()
        {
            //Arrange
            const int team_id = 40;

            var mock = GetFakeDbTeamMembers().AsQueryable().BuildMock();
            _teamMemberRepository.Setup(x => x.GetAll()).Returns(mock.Object);

            //Act
            var result = await _manageTeamsMembersService.GetAllTeamMembersAsync(team_id, new DisplayOptions { });

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
                new TeamMember {Id =4, MemberId="def-abc", Member= new Microsoft.AspNetCore.Identity.IdentityUser{ UserName = "b" }, TeamId =4},
                new TeamMember {Id =5, MemberId="abc-def", Member= new Microsoft.AspNetCore.Identity.IdentityUser{ UserName = "a" }, TeamId =4},
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
