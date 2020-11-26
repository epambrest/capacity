using NUnit.Framework;
using Moq;
using Teams.Security;
using System.Security.Claims;
using Teams.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using MockQueryable.Moq;
using Teams.Business.Services;
using Teams.Data.Models;

namespace Teams.Tests
{
    [TestFixture]
    class AddTeamMemberToTeamTests
    {
        private Mock<ICurrentUser> _currentUser;
        private Mock<IRepository<TeamMember, int>> _teamMemberRepository;
        private Mock<IRepository<Team, int>> _teamRepository;
        private ManageTeamsMembersService _teamsMembersService;

        [SetUp]
        public void Setup()
        {
           _currentUser = new Mock<ICurrentUser>();
            _teamMemberRepository =new Mock<IRepository<TeamMember, int>>();
            _teamRepository = new Mock<IRepository<Team, int>>();
            var mock = GetFakeDbTeam().AsQueryable().BuildMock();
            _teamRepository.Setup(t => t.GetAll()).Returns(mock.Object);
            _teamMemberRepository.Setup(t => t.InsertAsync(It.IsAny<TeamMember>())).ReturnsAsync(true);
            _teamsMembersService = new ManageTeamsMembersService(_teamRepository.Object, _teamMemberRepository.Object, _currentUser.Object);
        }

        [Test]
        public async System.Threading.Tasks.Task AddMember_teamsMembersServiceAddMemberReturnTrue_ReturnTrue()
        {

            //Arrange
            const string ownerId = "1";
            const string memberId = "1234";
            const int teamId = 1;
            var user = new Mock<UserDetails>(null);
            user.Setup(x => x.Id()).Returns(ownerId);
            user.Setup(x => x.Name()).Returns("name");
            _currentUser.SetupGet(x => x.Current).Returns(user.Object);

            //Act
            bool result = await _teamsMembersService.AddAsync(teamId, memberId);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task AddMember_teamsMembersServiceAddMemberReturnFalse_ReturnFalse()
        {

            //Arrange
            const string ownerId = "1";
            const string memberId = "2";
            var user = new Mock<UserDetails>(null);
            user.Setup(x => x.Id()).Returns(ownerId);
            user.Setup(x => x.Name()).Returns("name");
            _currentUser.SetupGet(x => x.Current).Returns(user.Object);

            //Act
            bool result1 = await _teamsMembersService.AddAsync(2, memberId);
            bool result2 = await _teamsMembersService.AddAsync(1, ownerId);

            //Assert
            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
        }


        private List<Team> GetFakeDbTeam()
        {
            var teams = new List<Team>
            {
                new Team {Id =1, TeamOwner = "1",TeamMembers = new List<TeamMember>(){ new TeamMember { Id=1,MemberId="1"} } },
                new Team {Id =2, TeamOwner = "1",TeamMembers = new List<TeamMember>(){ new TeamMember { Id=2,MemberId="2"} } },
            };
            return teams;
        }
    }
}
