using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Teams.Data;
using Teams.Models;
using Teams.Security;
using Teams.Services;

namespace Teams.Tests
{
    class ManageTeamsMembersServiceTest
    {
        public IManageTeamsMembersService _manageTeamsMembersService;

        public Mock<IRepository<Team, int>> _teamRepository;

        public Mock<IRepository<TeamMember, int>> _teamMemberRepository;

        public Mock<ICurrentUser> _currentUser;

        [SetUp]
        public void Setup()
        {
            _teamRepository = new Mock<IRepository<Team, int>>();

            _teamMemberRepository = new Mock<IRepository<TeamMember, int>>();

            _currentUser = new Mock<ICurrentUser>();

            _manageTeamsMembersService = new ManageTeamsMembersService(_currentUser.Object, _teamRepository.Object, _teamMemberRepository.Object);

        }

        [Test]
        public void Get_ManageTeamsMembersServiceReturnTeamMember_ReturnTeamMember()
        {
            //Arrange
            const int team_id = 4;
            const string member_id = "def-abc";
            const string current_user_id = "abc-def";
            var teams = new List<Team>
            {
                new Team { Id= 1, TeamOwner = "abc-def", TeamName = "Team1", TeamMembers=new List<TeamMember>{new TeamMember {MemberId="def-abc", TeamId =1}}},
                new Team { Id= 2, TeamOwner = "def-abc", TeamName = "Team2", TeamMembers=new List<TeamMember>{new TeamMember {MemberId="abc-def", TeamId =2}}},
                new Team { Id= 3, TeamOwner = "def-abc", TeamName = "Team3", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv"} }},
                new Team { Id= 4, TeamOwner = "abc-def", TeamName = "Team4", TeamMembers=new List<TeamMember>{new TeamMember {MemberId="abc-def", TeamId =4},new TeamMember {MemberId="def-abc", TeamId =4}}},
                new Team { Id= 5, TeamOwner = "def-abc", TeamName = "Team5", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv"}}},
                new Team { Id= 6, TeamOwner = "def-abc", TeamName = "Team6", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv"}}},
                new Team { Id= 7, TeamOwner = "def-abc", TeamName = "Team7", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv"}}},
                new Team { Id= 8, TeamOwner = "def-abc", TeamName = "Team8", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv"}}},
                new Team { Id= 9, TeamOwner = "def-abc", TeamName = "Team9", TeamMembers=new List<TeamMember>{new TeamMember {MemberId="abc-def", TeamId =9}}},
                new Team { Id= 10, TeamOwner = "def-abc", TeamName = "Team10", TeamMembers=new List<TeamMember>{ new TeamMember{MemberId="asf-fgv"}}}
            };
            var teamMembers = new List<TeamMember>
            {
                new TeamMember {MemberId="def-abc", TeamId =1},
                new TeamMember {MemberId="abc-def", TeamId =2},
                new TeamMember {MemberId="asf-fgv", TeamId =3},
                new TeamMember {MemberId="def-abc", TeamId =4},
                new TeamMember {MemberId="abc-def", TeamId =4},
                new TeamMember {MemberId="asf-fgv", TeamId =5},
                new TeamMember {MemberId="asf-fgv", TeamId =6},
                new TeamMember {MemberId="asf-fgv", TeamId =7},
                new TeamMember {MemberId="asf-fgv", TeamId =8},
                new TeamMember {MemberId="abc-def", TeamId =9},
                new TeamMember {MemberId="asf-fgv", TeamId =10}
            };

            _teamMemberRepository.Setup(x => x.GetAll()).Returns(teamMembers.AsQueryable());
            _teamRepository.Setup(x => x.GetAll()).Returns(teams.AsQueryable());

            var ud = new Mock<UserDetails>(null);
            ud.Setup(x => x.Id()).Returns(current_user_id);
            ud.Setup(x => x.Name()).Returns("name");
            _currentUser.SetupGet(x => x.Current).Returns(ud.Object);

            //Act
            var result = _manageTeamsMembersService.GetMember(team_id, member_id);

            //Assert
            Assert.AreEqual(result.TeamId, team_id);
            Assert.AreEqual(result.MemberId, member_id);
        }
    }
}
