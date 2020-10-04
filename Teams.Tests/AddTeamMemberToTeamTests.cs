using NUnit.Framework;
using Moq;
using Teams.Security;
using System.Security.Claims;
using Teams.Data;
using Teams.Models;
using Teams.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using MockQueryable.Moq;

namespace Teams.Tests
{
    [TestFixture]
    class AddTeamMemberToTeamTests
    {
        private Mock<ICurrentUser> currentUser;
        private Mock<IRepository<TeamMember, int>> teamMemberRepository;
        private Mock<IRepository<Team, int>> teamRepository;
        private ManageTeamsMembersService teamsMembersService;

        [SetUp]
        public void Setup()
        {
            currentUser = new Mock<ICurrentUser>();
            teamMemberRepository =new Mock<IRepository<TeamMember, int>>();
            teamRepository = new Mock<IRepository<Team, int>>();
            var mock = GetFakeDbTeam().AsQueryable().BuildMock();
            teamRepository.Setup(t => t.GetAll()).Returns(mock.Object);
            teamMemberRepository.Setup(t => t.InsertAsync(It.IsAny<TeamMember>())).ReturnsAsync(true);
            teamsMembersService = new ManageTeamsMembersService(teamRepository.Object, teamMemberRepository.Object, currentUser.Object);
        }

        [Test]
        public async Task AddMember_teamsMembersServiceAddMemberReturnTrue_ReturnTrue()
        {

            //Arrange
            const string ownerId = "1";
            const string memberId = "1234";
            const int teamId = 1;
            var user = new Mock<UserDetails>(null);
            user.Setup(x => x.Id()).Returns(ownerId);
            user.Setup(x => x.Name()).Returns("name");
            currentUser.SetupGet(x => x.Current).Returns(user.Object);

            //Act
            bool result = await teamsMembersService.AddAsync(teamId, memberId);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task AddMember_teamsMembersServiceAddMemberReturnFalse_ReturnFalse()
        {

            //Arrange
            const string ownerId = "1";
            const string memberId = "1234";
            var user = new Mock<UserDetails>(null);
            user.Setup(x => x.Id()).Returns(ownerId);
            user.Setup(x => x.Name()).Returns("name");
            currentUser.SetupGet(x => x.Current).Returns(user.Object);

            //Act
            bool result1 = await teamsMembersService.AddAsync(2, memberId);
            bool result2 = await teamsMembersService.AddAsync(1, ownerId);

            //Assert
            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
        }


        private List<Team> GetFakeDbTeam()
        {
            var teams = new List<Team>
            {
                new Team {Id =1, TeamOwner = "1",TeamMembers = new List<TeamMember>(){ new TeamMember { Id=1,MemberId="2"} } },
            };
            return teams;
        }
    }
}
