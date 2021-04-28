using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
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
            _teamRepository.Setup(t => t.GetAllAsync())
                .Returns(System.Threading.Tasks.Task.FromResult(GetFakeDbTeam()));
            _teamMemberRepository.Setup(t => t.InsertAsync(It.IsAny<Business.Models.TeamMember>())).ReturnsAsync(true);
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


        private IEnumerable<Team> GetFakeDbTeam()
        {
            var teams = new List<Team>
            {
                Team.Create(8, "2", "1234", new List<TeamMember>() { 
                    TeamMember.Create(1, 8, "1", User.Create("1", "vasya@mail.ru", "vasya", "ivanov")) }),
                Team.Create(2, "1", "1234", new List<TeamMember>() { 
                    TeamMember.Create(2, 2, "2", User.Create("2", "vasya@mail.ru", "vasya", "ivanov")) }),
            };
            return teams;
        }
    }
}
