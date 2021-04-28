using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Teams.Business.Models;
using Teams.Business.Repository;
using Teams.Business.Services;

namespace Teams.Business.Tests
{
    [TestFixture]
    class ManageMemberWorkingDaysServiceTest
    {
        private Mock<IRepository<MemberWorkingDays, int>> _memberWorkingDaysRepository;
        private ManageMemberWorkingDaysService _manageMemberWorkingDaysService;

        [SetUp]
        public void Setup()
        {
            _memberWorkingDaysRepository = new Mock<IRepository<MemberWorkingDays, int>>();
            _manageMemberWorkingDaysService = new ManageMemberWorkingDaysService(_memberWorkingDaysRepository.Object);
        }

        [Test]
        public async System.Threading.Tasks.Task GetTaskByIdAsync_ManageMemberWorkingDaysServiceReturnsId1_ReturnsId1()
        {

            //Arrange
            const int memberWorkingDaysId = 1;
            Sprint sprint = Sprint.Create(1, new List<Task>());
            var memberWorkingDays = new List<MemberWorkingDays>()
            {
                MemberWorkingDays.Create(1, 1, 1, sprint, 21)
            };
            var mock = memberWorkingDays.AsQueryable().BuildMock();

            _memberWorkingDaysRepository.Setup(x => x.GetAllAsync())
                .Returns(System.Threading.Tasks.Task.FromResult(memberWorkingDays.AsEnumerable()));

            //Act
            var result = await _manageMemberWorkingDaysService.GetWorkingDaysByIdAsync(memberWorkingDaysId);

            //Assert
            Assert.AreEqual(memberWorkingDaysId, result.Id);
        }

        [Test]
        public async System.Threading.Tasks.Task AddMemberWorkingDaysAsync_ManageMemberWorkingDaysServiceReturnsTrue_ReturnsTrue()
        {
            //Arrange
            Sprint sprint = Sprint.Create(1, new List<Task>());
            var memberWorkingDays = MemberWorkingDays.Create(1, 1, 1, sprint, 21);

            _memberWorkingDaysRepository.Setup(x => x.InsertAsync(It.IsAny<MemberWorkingDays>())).ReturnsAsync(true);

            //Act
            var result = await _manageMemberWorkingDaysService.AddMemberWorkingDaysAsync(memberWorkingDays);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task AddMemberWorkingDaysAsync_ManageMemberWorkingDaysServiceReturnsFalse_ReturnsFalse()
        {
            //Arrange
            Sprint sprint = Sprint.Create(1, new List<Task>());
            var memberWorkingDays = MemberWorkingDays.Create(1, 1, 1, sprint, -4);
            _memberWorkingDaysRepository.Setup(x => x.InsertAsync(It.IsAny<MemberWorkingDays>())).ReturnsAsync(true);

            //Act
            var result = await _manageMemberWorkingDaysService.AddMemberWorkingDaysAsync(memberWorkingDays); 

            //Assert
            Assert.IsFalse(result);
        }

        private static IEnumerable<TestCaseData> GetStoryPointsInDayForMemberTestData
        {
            get
            {
                yield return new TestCaseData(1, 1, 5, 0.71);
                yield return new TestCaseData(0, 0, 5, 0);
            }
        }

        [Test, TestCaseSource(nameof(GetStoryPointsInDayForMemberTestData))]
        public async System.Threading.Tasks.Task GetStoryPointsInDayForMember_TestsValue(int sprintId, 
            int teamMemberId, 
            int teamMemberTotalSp, 
            double excepted)
        {
            //Arrange
            var team = Team.Create(1, "1", "1234", new List<TeamMember>());
            var sprint = Sprint.Create(1, 1, team, "Sprint1", 14, 4, PossibleStatuses.ActiveStatus);

            var memberWorkingDays = new List<MemberWorkingDays>()
            {
                MemberWorkingDays.Create(1, 1, 1, sprint, 7),
            };

            _memberWorkingDaysRepository.Setup(x => x.GetAllAsync())
                .Returns(System.Threading.Tasks.Task.FromResult(memberWorkingDays.AsEnumerable()));

            //Act
            var result = await _manageMemberWorkingDaysService.GetStoryPointsInDayForMember(sprintId, teamMemberId, teamMemberTotalSp);

            //Assert
            Assert.AreEqual(excepted, System.Math.Round(result, 2));
        }

        [Test]
        public async System.Threading.Tasks.Task EditMemberWorkingDaysAsync_ManageMemberWorkingDaysServiceReturnsTrue_ReturnsTrue()
        {
            //Arrange
            Sprint sprint = Sprint.Create(1, new List<Task>());
            var memberWorkingDays = MemberWorkingDays.Create(1, 1, 1, sprint, 10);
            _memberWorkingDaysRepository.Setup(x => x.UpdateAsync(It.IsAny<MemberWorkingDays>())).ReturnsAsync(true);
            _memberWorkingDaysRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(memberWorkingDays);

            //Act
            var result = await _manageMemberWorkingDaysService.EditMemberWorkingDaysAsync(memberWorkingDays);

            //Assert
            Assert.IsTrue(result);
        }
    }
}
