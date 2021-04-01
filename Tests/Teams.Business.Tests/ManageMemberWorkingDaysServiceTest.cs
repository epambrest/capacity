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
    class ManageMemberWorkingDaysServiceTest
    {
        private Mock<IRepository<MemberWorkingDays, int>> _memberWorkingDaysRepository;
        private ManageMemberWorkingDaysService _manageMemberWorkingDaysService;
        private Mock<ICurrentUser> _currentUser;

        [SetUp]
        public void Setup()
        {
            _currentUser = new Mock<ICurrentUser>();
            _memberWorkingDaysRepository = new Mock<IRepository<MemberWorkingDays, int>>();
            _manageMemberWorkingDaysService = new ManageMemberWorkingDaysService(_memberWorkingDaysRepository.Object, _currentUser.Object);
        }
        [Test]
        public async System.Threading.Tasks.Task GetTaskByIdAsync_ManageMemberWorkingDaysServiceReturnsId1_ReturnsId1()
        {

            //Arrange
            const int memberWorkingDaysId = 1;
            var memberWorkingDays = new List<MemberWorkingDays>()
            {  new MemberWorkingDays { Id = 1, MemberId = 1, SprintId = 1, WorkingDays = 21 } };
            var mock = memberWorkingDays.AsQueryable().BuildMock();

            _memberWorkingDaysRepository.Setup(x => x.GetAll()).Returns(mock.Object);

            //Act
            var result = await _manageMemberWorkingDaysService.GetWorkingDaysByIdAsync(memberWorkingDaysId);

            //Assert
            Assert.AreEqual(memberWorkingDaysId, result.Id);
        }

        [Test]
        public async System.Threading.Tasks.Task AddMemberWorkingDaysAsync_ManageMemberWorkingDaysServiceReturnsTrue_ReturnsTrue()
        {
            //Arrange
            var memberWorkingDays = new MemberWorkingDays { Id = 1, MemberId = 1, SprintId = 1, WorkingDays = 21 };

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
            var memberWorkingDays = new MemberWorkingDays { Id = 1, MemberId = 1, SprintId = 1, WorkingDays = -4 };
            _memberWorkingDaysRepository.Setup(x => x.InsertAsync(It.IsAny<MemberWorkingDays>())).ReturnsAsync(true);

            //Act
            var result = await _manageMemberWorkingDaysService.AddMemberWorkingDaysAsync(memberWorkingDays); 

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async System.Threading.Tasks.Task EditMemberWorkingDaysAsync_ManageMemberWorkingDaysServiceReturnsTrue_ReturnsTrue()
        {
            //Arrange
            var memberWorkingDays = new MemberWorkingDays { Id = 1, MemberId = 1, SprintId = 1, WorkingDays = 10 };
            _memberWorkingDaysRepository.Setup(x => x.UpdateAsync(It.IsAny<MemberWorkingDays>())).ReturnsAsync(true);
            _memberWorkingDaysRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(memberWorkingDays);

            //Act
            var result = await _manageMemberWorkingDaysService.EditMemberWorkingDaysAsync(memberWorkingDays);

            //Assert
            Assert.IsTrue(result);
        }
    }
}
