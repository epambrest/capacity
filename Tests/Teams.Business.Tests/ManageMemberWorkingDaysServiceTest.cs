using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.Business.Models;
using Teams.Business.Services;
using Teams.Data.Models;
using Teams.Data.Repository;

namespace Teams.Business.Tests
{
    [TestFixture]
    class ManageMemberWorkingDaysServiceTest
    {
        private Mock<IRepository<Data.Models.MemberWorkingDays, Models.MemberWorkingDays, int>> _memberWorkingDaysRepository;
        private ManageMemberWorkingDaysService _manageMemberWorkingDaysService;

        [SetUp]
        public void Setup()
        {
            _memberWorkingDaysRepository = new Mock<IRepository<Data.Models.MemberWorkingDays, Models.MemberWorkingDays, int>>();
            _manageMemberWorkingDaysService = new ManageMemberWorkingDaysService(_memberWorkingDaysRepository.Object);
        }

        [Test]
        public async System.Threading.Tasks.Task GetTaskByIdAsync_ManageMemberWorkingDaysServiceReturnsId1_ReturnsId1()
        {

            //Arrange
            const int memberWorkingDaysId = 1;
            var memberWorkingDays = new List<Models.MemberWorkingDays>()
            {  
                new Models.MemberWorkingDays { Id = 1, MemberId = 1, SprintId = 1, WorkingDays = 21 } 
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
            var memberWorkingDays = new Business.Models.MemberWorkingDays { Id = 1, MemberId = 1, SprintId = 1, WorkingDays = 21 };

            _memberWorkingDaysRepository.Setup(x => x.InsertAsync(It.IsAny<Business.Models.MemberWorkingDays>())).ReturnsAsync(true);

            //Act
            var result = await _manageMemberWorkingDaysService.AddMemberWorkingDaysAsync(memberWorkingDays);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task AddMemberWorkingDaysAsync_ManageMemberWorkingDaysServiceReturnsFalse_ReturnsFalse()
        {
            //Arrange
            var memberWorkingDays = new Business.Models.MemberWorkingDays { Id = 1, MemberId = 1, SprintId = 1, WorkingDays = -4 };
            _memberWorkingDaysRepository.Setup(x => x.InsertAsync(It.IsAny<Business.Models.MemberWorkingDays>())).ReturnsAsync(true);

            //Act
            var result = await _manageMemberWorkingDaysService.AddMemberWorkingDaysAsync(memberWorkingDays); 

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async System.Threading.Tasks.Task EditMemberWorkingDaysAsync_ManageMemberWorkingDaysServiceReturnsTrue_ReturnsTrue()
        {
            //Arrange
            var memberWorkingDays = new Business.Models.MemberWorkingDays { Id = 1, MemberId = 1, SprintId = 1, WorkingDays = 10 };
            _memberWorkingDaysRepository.Setup(x => x.UpdateAsync(It.IsAny<Business.Models.MemberWorkingDays>())).ReturnsAsync(true);
            _memberWorkingDaysRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(memberWorkingDays);

            //Act
            var result = await _manageMemberWorkingDaysService.EditMemberWorkingDaysAsync(memberWorkingDays);

            //Assert
            Assert.IsTrue(result);
        }
    }
}
