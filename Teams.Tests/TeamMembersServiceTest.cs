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
        private IRepository<TeamMember, int> teamMemberRepository;
        private IRepository<Team, int> teamRepository;
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
            teamMemberRepository = new TeamMemberRepository(context);
            teamRepository = new TeamRepository(context);
            teamsMembersService = new ManageTeamsMembersService(teamRepository, teamMemberRepository, _currentUser);
        }

        [Test]
        public void RemoveMember_teamsMembersServiceRemoveMemberReturnTrue_ReturnTrue()
        {

            //Arrange
            string Id = "1234";
            string memberId = "4321";
            int teamId = 1;
            _httpContextAccessor.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
               .Returns(new Claim("UserName", Id));
            _httpContextAccessor.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            teamRepository.InsertAsync(new Team { TeamName = "first team", TeamOwner = Id });
            teamMemberRepository.InsertAsync(new TeamMember { TeamId = teamId, MemberId = memberId });

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
            _httpContextAccessor.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
               .Returns(new Claim("UserName", Id));
            _httpContextAccessor.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            teamRepository.InsertAsync(new Team { TeamName = "first team", TeamOwner = "1234" });
            teamMemberRepository.InsertAsync(new TeamMember { TeamId = teamId, MemberId = memberId });

            //Act
            bool result = teamsMembersService.RemoveAsync(teamId, memberId).Result;

            //Assert
            Assert.IsFalse(result);
        }
    }
}
