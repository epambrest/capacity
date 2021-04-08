using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Business.Models;
using Teams.Business.Repository;
using Teams.Business.Services;

namespace Teams.Business.Tests
{
    [TestFixture]
    class ManageMemberWorkingDaysServiceTest
    {
        private Mock<IRepository<MemberWorkingDaysBusiness, int>> _memberWorkingDaysRepository;
        private ManageMemberWorkingDaysService _manageMemberWorkingDaysService;

        [SetUp]
        public void Setup()
        {
            _memberWorkingDaysRepository = new Mock<IRepository<MemberWorkingDaysBusiness, int>>();
            _manageMemberWorkingDaysService = new ManageMemberWorkingDaysService(_memberWorkingDaysRepository.Object);
        }

        [Test]
        public async Task GetTaskByIdAsync_ManageMemberWorkingDaysServiceReturnsId1_ReturnsId1()
        {

            //Arrange
            const int memberWorkingDaysId = 1;
            var memberWorkingDays = new List<MemberWorkingDaysBusiness>()
            {  
                new MemberWorkingDaysBusiness { Id = 1, MemberId = 1, SprintId = 1, WorkingDays = 21 } 
            };
            var mock = memberWorkingDays.AsQueryable().BuildMock();

            _memberWorkingDaysRepository.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(memberWorkingDays.AsEnumerable()));

            //Act
            var result = await _manageMemberWorkingDaysService.GetWorkingDaysByIdAsync(memberWorkingDaysId);

            //Assert
            Assert.AreEqual(memberWorkingDaysId, result.Id);
        }

        [Test]
        public async Task AddMemberWorkingDaysAsync_ManageMemberWorkingDaysServiceReturnsTrue_ReturnsTrue()
        {
            //Arrange
            var memberWorkingDays = new MemberWorkingDaysBusiness { Id = 1, MemberId = 1, SprintId = 1, WorkingDays = 21 };

            _memberWorkingDaysRepository.Setup(x => x.InsertAsync(It.IsAny<MemberWorkingDaysBusiness>())).ReturnsAsync(true);

            //Act
            var result = await _manageMemberWorkingDaysService.AddMemberWorkingDaysAsync(memberWorkingDays);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task AddMemberWorkingDaysAsync_ManageMemberWorkingDaysServiceReturnsFalse_ReturnsFalse()
        {
            //Arrange
            var memberWorkingDays = new MemberWorkingDaysBusiness { Id = 1, MemberId = 1, SprintId = 1, WorkingDays = -4 };
            _memberWorkingDaysRepository.Setup(x => x.InsertAsync(It.IsAny<MemberWorkingDaysBusiness>())).ReturnsAsync(true);

            //Act
            var result = await _manageMemberWorkingDaysService.AddMemberWorkingDaysAsync(memberWorkingDays); 

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task EditMemberWorkingDaysAsync_ManageMemberWorkingDaysServiceReturnsTrue_ReturnsTrue()
        {
            //Arrange
            var memberWorkingDays = new MemberWorkingDaysBusiness { Id = 1, MemberId = 1, SprintId = 1, WorkingDays = 10 };
            _memberWorkingDaysRepository.Setup(x => x.UpdateAsync(It.IsAny<MemberWorkingDaysBusiness>())).ReturnsAsync(true);
            _memberWorkingDaysRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(memberWorkingDays);

            //Act
            var result = await _manageMemberWorkingDaysService.EditMemberWorkingDaysAsync(memberWorkingDays);

            //Assert
            Assert.IsTrue(result);
        }
    }
}
