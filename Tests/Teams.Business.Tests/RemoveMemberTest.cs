using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Business.Models;
using Teams.Business.Repository;
using Teams.Business.Services;
using Teams.Security;


namespace Teams.Business.Tests
{
    [TestFixture]
    class DeleteMemberFromTeamTest
    {
        private Mock<ICurrentUser> _currentUser;
        private Mock<IRepository<TeamMemberBusiness, int>> _teamMemberRepository;
        private Mock<IRepository<TeamBusiness, int>> _teamRepository;
        private ManageTeamsMembersService _teamsMembersService;

        [SetUp]
        public void Setup()
        {
            _currentUser = new Mock<ICurrentUser>();
            _teamMemberRepository = new Mock<IRepository<TeamMemberBusiness, int>>();
            _teamRepository = new Mock<IRepository<TeamBusiness, int>>();
            var mock = GetFakeDbTeam().AsQueryable().BuildMock();
            _teamMemberRepository.Setup(t => t.GetAllAsync()).Returns(Task.FromResult(GetFakeDbTeam()));
            _teamMemberRepository.Setup(t => t.DeleteAsync(It.IsAny<TeamMemberBusiness>())).ReturnsAsync(true);
            _teamsMembersService = new ManageTeamsMembersService(_teamRepository.Object, _teamMemberRepository.Object, _currentUser.Object);
        }

        [Test]
        public async Task RemoveMember_teamsMembersServiceRemoveMemberReturnTrue_ReturnTrue()
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
        public async Task RemoveMember_teamsMembersServiceRemoveMemberReturnFalse_ReturnFalse()
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

        private IEnumerable<TeamMemberBusiness> GetFakeDbTeam()
        {
            var members = new List<TeamMemberBusiness>
            {
                new TeamMemberBusiness 
                { 
                    MemberId = "1234", 
                    TeamId = 1,
                    Team = new TeamBusiness { TeamOwner = "1"} 
                },

                new TeamMemberBusiness
                { 
                    MemberId = "1",
                    TeamId = 2,
                    Team = new TeamBusiness { TeamOwner = "1"}
                }
            };
            return members;
        }
    }
}