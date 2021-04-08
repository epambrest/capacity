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
    class AddTeamMemberToTeamTests
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
            _teamRepository.Setup(t => t.GetAllAsync()).Returns(Task.FromResult(GetFakeDbTeam()));
            _teamMemberRepository.Setup(t => t.InsertAsync(It.IsAny<TeamMemberBusiness>())).ReturnsAsync(true);
            _teamsMembersService = new ManageTeamsMembersService(_teamRepository.Object, _teamMemberRepository.Object, _currentUser.Object);
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
            _currentUser.SetupGet(x => x.Current).Returns(user.Object);

            //Act
            bool result = await _teamsMembersService.AddAsync(teamId, memberId);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task AddMember_teamsMembersServiceAddMemberReturnFalse_ReturnFalse()
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


        private IEnumerable<TeamBusiness> GetFakeDbTeam()
        {
            var teams = new List<TeamBusiness>
            {
                new TeamBusiness 
                {
                    Id = 1, 
                    TeamOwner = "1", 
                    TeamMembers = new List<TeamMemberBusiness>() { new TeamMemberBusiness { Id = 1,MemberId = "1"} } 
                },

                new TeamBusiness 
                {
                    Id = 2, 
                    TeamOwner = "1", 
                    TeamMembers = new List<TeamMemberBusiness>() { new TeamMemberBusiness { Id = 2,MemberId = "2"} } 
                },
            };
            return teams;
        }
    }
}
