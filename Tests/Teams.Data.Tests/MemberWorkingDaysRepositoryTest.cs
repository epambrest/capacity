using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Teams.Data.Models;
using Teams.Data.Repository;

namespace Teams.Data.Tests
{
    [TestFixture]
    class MemberWorkingDaysRepositoryTest
    {
        private ApplicationDbContext _context;
        private IRepository<MemberWorkingDays, int> _memberWorkingDaysRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "MemberWorkingDaysDatabase").Options;

            _context = new ApplicationDbContext(options);

            _memberWorkingDaysRepository = new MemberWorkingDaysRepository(_context);
        }

        private IQueryable<MemberWorkingDays> GetFakeMemberWorkingDaysDb()
        {
            var data = new List<MemberWorkingDays>
            {
                new MemberWorkingDays{ Id = 1, SprintId = 1, MemberId = 1, WorkingDays = 27 },
                new MemberWorkingDays{ Id = 2, SprintId = 2, MemberId = 1, WorkingDays = 18 },
                new MemberWorkingDays{ Id = 3, SprintId = 1, MemberId = 3, WorkingDays = 2 },
            }.AsQueryable();
            return data;
        }

        #region GetAll
        [Test]
        public async System.Threading.Tasks.Task GetAll_MemberWorkingDaysRepositoryReturnsListCount3_ListCount3()
        {
            //Arrange
            const int memberWorkingDaysCount = 3;
            _context.MemberWorkingDays.AddRange(GetFakeMemberWorkingDaysDb());
            _context.SaveChanges();

            //Act
            var result = await _memberWorkingDaysRepository.GetAll().ToListAsync();

            //Assert
            Assert.AreEqual(result.Count(), memberWorkingDaysCount);
        }

        [Test]
        public void GetAll_MemberWorkingDaysRepositoryReturnsEmptyList_ReturnsEmpty()
        {
            //Act
            var teams = _memberWorkingDaysRepository.GetAll();

            //Assert
            Assert.IsEmpty(teams);

        }
        #endregion

        #region GetByIdAsync
        [Test]
        public async System.Threading.Tasks.Task GetByIdAsync_MemberWorkingDaysRepositoryReturnsId1_Id1()
        {
            //Arrange
            const int memberWorkingDaysId = 1;

            //Act
            var result = await _memberWorkingDaysRepository.GetByIdAsync(memberWorkingDaysId);

            //Assert
            Assert.AreEqual(memberWorkingDaysId, result.Id);
        }

        [Test]
        public async System.Threading.Tasks.Task GetByIdAsync_MemberWorkingDaysRepositoryReturnsNull_ReturnsNull()
        {
            //Arrange
            const int memberWorkingDaysId = 10;

            //Act
            var result = await _memberWorkingDaysRepository.GetByIdAsync(memberWorkingDaysId);

            //Assert
            Assert.IsNull(result);
        }
        #endregion

        #region InsertAsync
        [Test]
        public async System.Threading.Tasks.Task InsertAsync_MemberWorkingDaysRepositoryReturnsTrue_ReturnsTrue()
        {
            //Arrange
            MemberWorkingDays memberWorkingDays = new MemberWorkingDays { Id = 4, SprintId = 2, MemberId = 1, WorkingDays = 21 };

            //Act
            var result = await _memberWorkingDaysRepository.InsertAsync(memberWorkingDays);

            //Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region DeleteAsync
        [Test]
        public async System.Threading.Tasks.Task DeleteAsync_MemberWorkingDaysRepositoryReturnsTrue_ReturnsTrue()
        {
            //Arrange
            MemberWorkingDays memberWorkingDays = new MemberWorkingDays { Id = 4, SprintId = 2, MemberId = 1, WorkingDays = 21 };
            _context.MemberWorkingDays.Add(memberWorkingDays);
            _context.SaveChanges();

            //Act
            var result = await _memberWorkingDaysRepository.DeleteAsync(memberWorkingDays);

            //Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region UpdateAsync
        [Test]
        public async System.Threading.Tasks.Task UpdateAsync_MemberWorkingDaysRepositoryReturnsTrue_ReturnsTrue()
        {
            //Arrange
            MemberWorkingDays memberWorkingDays = new MemberWorkingDays { Id = 1, SprintId = 1, MemberId = 1, WorkingDays = 2 };

            //Act
            var result = await _memberWorkingDaysRepository.UpdateAsync(memberWorkingDays);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task UpdateAsync_MemberWorkingDaysRepositoryReturnsFalse_ReturnsFalse()
        {
            //Arrange
            MemberWorkingDays memberWorkingDays = new MemberWorkingDays { Id = 7, SprintId = 1, MemberId = 1, WorkingDays = 2 };

            //Act
            var result = await _memberWorkingDaysRepository.UpdateAsync(memberWorkingDays);

            //Assert
            Assert.IsFalse(result);
        }
        #endregion
    }
}
