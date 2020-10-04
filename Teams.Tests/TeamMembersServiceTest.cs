using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using Teams.Data;
using Teams.Models;
using Teams.Repository;
using Teams.Security;
using Teams.Services;

namespace Teams.Tests
{
    class TeamMembersServiceTest
    {
        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private ICurrentUser _currentUser;
        private ApplicationDbContext context;
        private Mock<IRepository<TeamMember, int>> teamMemberRepository;
        private Mock<IRepository<Team, int>> teamRepository;
        private IManageTeamsMembersService teamsMembersService;

        [SetUp]
        public void Setup()
        {
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _currentUser = new CurrentUser(_httpContextAccessor.Object);
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: "TeamMemberListDatabase")
               .Options;
            context = new ApplicationDbContext(options);
            teamMemberRepository = new Mock<IRepository<TeamMember, int>>();
            teamRepository = new Mock<IRepository<Team, int>>();
            
        }

        [Test]
        public void RemoveMember_teamsMembersServiceRemoveMemberReturnTrue_ReturnTrue()
        {

            //Arrange
            string Id = "1234";
            string memberId = "4321";
            int teamId = 1;
            TeamMember member = new TeamMember { TeamId = teamId, MemberId = memberId };
            context.TeamMembers.Add(member);
            context.Team.Add(new Team { TeamName = "first team", TeamOwner = Id});           
            context.SaveChanges();
            _httpContextAccessor.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
               .Returns(new Claim("UserName", Id));
            _httpContextAccessor.Setup(x => x.HttpContext.User.Identity.IsAuthenticated)
                .Returns(true);
            teamMemberRepository.Setup(x => x.DeleteAsync(It.IsAny<TeamMember>()))
                .ReturnsAsync(true);
            teamRepository.Setup(x => x.GetAll())
                .Returns(context.Team);
            teamsMembersService = new ManageTeamsMembersService(teamRepository.Object, teamMemberRepository.Object, _currentUser);
           

            //Act
            bool result = teamsMembersService.RemoveAsync(teamId, memberId).Result;

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void RemoveMember_teamsMembersServiceRemoveMemberReturnFalse_ReturnFalse()
        {

            //Arrange
            string Id = "123";
            string memberId = "4321";
            int teamId = 1;
            context.Team.Add(new Team { TeamName = "first team", TeamOwner = "1234" });
            context.TeamMembers.Add(new TeamMember { TeamId = teamId, MemberId = memberId });
            context.SaveChanges();
            _httpContextAccessor.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
               .Returns(new Claim("UserName", Id));
            _httpContextAccessor.Setup(x => x.HttpContext.User.Identity.IsAuthenticated)
                .Returns(true);
            teamMemberRepository.Setup(x => x.DeleteAsync(It.IsAny<TeamMember>()))
               .ReturnsAsync(true);
            teamRepository.Setup(x => x.GetAll())
                .Returns(context.Team);
            teamsMembersService = new ManageTeamsMembersService(teamRepository.Object, teamMemberRepository.Object, _currentUser);

            //Act
            bool result = teamsMembersService.RemoveAsync(teamId, memberId).Result;

            //Assert
            Assert.IsFalse(result);
        }
    }
}
