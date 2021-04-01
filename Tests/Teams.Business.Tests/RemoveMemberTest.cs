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
    [TestFixture]
    class DeleteMemberFromTeamTest
    {
        private Mock<ICurrentUser> _currentUser;
        private Mock<IRepository<TeamMember, int>> _teamMemberRepository;
        private Mock<IRepository<Team, int>> _teamRepository;
        private ManageTeamsMembersService _teamsMembersService;

        [SetUp]
        public void Setup()
        {
            _currentUser = new Mock<ICurrentUser>();
            _teamMemberRepository = new Mock<IRepository<TeamMember, int>>();
            _teamRepository = new Mock<IRepository<Team, int>>();
            var mock = GetFakeDbTeam().AsQueryable().BuildMock();
            _teamMemberRepository.Setup(t => t.GetAll()).Returns(mock.Object);
            _teamMemberRepository.Setup(t => t.DeleteAsync(It.IsAny<TeamMember>())).ReturnsAsync(true);
            _teamsMembersService = new ManageTeamsMembersService(_teamRepository.Object, _teamMemberRepository.Object, _currentUser.Object);
        }

        [Test]
        public async System.Threading.Tasks.Task RemoveMember_teamsMembersServiceRemoveMemberReturnTrue_ReturnTrue()
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
            bool result = await _teamsMembersService.RemoveAsync(teamId, memberId);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task RemoveMember_teamsMembersServiceRemoveMemberReturnFalse_ReturnFalse()
        {

            //Arrange
            const string ownerId = "1";
            const int teamId = 2;
            var user = new Mock<UserDetails>(null);
            user.Setup(x => x.Id()).Returns(ownerId);
            user.Setup(x => x.Name()).Returns("name");
            _currentUser.SetupGet(x => x.Current).Returns(user.Object);

            //Act
            bool result = await _teamsMembersService.RemoveAsync(teamId, ownerId);

            //Assert
            Assert.IsFalse(result);
        }

        private List<TeamMember> GetFakeDbTeam()
        {
            var members = new List<TeamMember>
            {
                new TeamMember{ MemberId = "1234", TeamId =1,Team = new Team { TeamOwner="1"} },
                new TeamMember{ MemberId = "1", TeamId =2,Team = new Team { TeamOwner="1"} }
            };
            return members;
        }
    }
}