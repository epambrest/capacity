using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Http;
using Teams.Security;
using System.Security.Claims;
using Teams.Data;
using Teams.Models;
using Microsoft.EntityFrameworkCore;
using Teams.Repository;
using Teams.Services;
using System.Security.Authentication;

namespace Teams.Tests
{
    [TestFixture]
    class AddTeamMemberToTeamTests
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
        public void AddMember_teamsMembersServiceAddMemberReturnTrue_ReturnTrue()
        {

            //Arrange
            string memberId = "1234";
            int teamId = 1;
            _httpContextAccessor.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
               .Returns(new Claim("UserName", memberId));
            _httpContextAccessor.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            teamRepository.InsertAsync(new Team { TeamName = "first team", TeamOwner = memberId });

            //Act
            bool result = teamsMembersService.AddAsync(teamId, memberId).Result;

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void AddMember_teamsMembersServiceAddMemberReturnFalse_ReturnFalse()
        {

            //Arrange
            string memberId = "1111";
            int teamId = 1;
            _httpContextAccessor.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
               .Returns(new Claim("UserName", memberId));
            _httpContextAccessor.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(true);

            //Act
            bool result = teamsMembersService.AddAsync(teamId, memberId).Result;

            //Assert
            Assert.IsFalse(result);
        }

    }
}
