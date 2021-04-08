using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Teams.Business.Models;
using Teams.Business.Repository;
using Teams.Data.Mappings;
using Teams.Data.Models;
using Teams.Data.Repository;

namespace Teams.Data.Tests
{
    [TestFixture]
    class MemberWorkingDaysRepositoryTest
    {
        private ApplicationDbContext _context;
        private IRepository<MemberWorkingDaysBusiness, int> _memberWorkingDaysRepository;
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "MemberWorkingDaysDatabase").Options;

            _context = new ApplicationDbContext(options);

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MemberWorkingDaysProfile());
                mc.AddProfile(new SprintProfile());
                mc.AddProfile(new TeamMemberProfile());
                mc.AddProfile(new UserProfile());
                mc.AddProfile(new TeamProfile());
                mc.AddProfile(new TaskProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            _mapper = mapper;
          
            _memberWorkingDaysRepository = new MemberWorkingDaysRepository(_context, _mapper);
        }

        private IQueryable<MemberWorkingDays> GetFakeMemberWorkingDaysDb()
        {
            var data = new List<MemberWorkingDays>
            {
                new MemberWorkingDays 
                { 
                    Id = 1, 
                    SprintId = 1, 
                    MemberId = 1, 
                    WorkingDays = 27,
                    Sprint = new Sprint(),
                    TeamMember = new TeamMember()
                },

                new MemberWorkingDays 
                { 
                    Id = 2, 
                    SprintId = 2, 
                    MemberId = 1, 
                    WorkingDays = 18,
                    Sprint = new Sprint(),
                    TeamMember = new TeamMember()
                },

                new MemberWorkingDays 
                { 
                    Id = 3, 
                    SprintId = 1, 
                    MemberId = 3, 
                    WorkingDays = 2,
                    Sprint = new Sprint(),
                    TeamMember = new TeamMember()
                },

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
            var result = await _memberWorkingDaysRepository.GetAllAsync();

            //Assert
            Assert.AreEqual(result.Count(), memberWorkingDaysCount);
        }

        [Test]
        public async System.Threading.Tasks.Task GetAll_MemberWorkingDaysRepositoryReturnsEmptyList_ReturnsEmpty()
        {
            //Act
            var teams = await _memberWorkingDaysRepository.GetAllAsync();

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
            MemberWorkingDaysBusiness memberWorkingDays = new MemberWorkingDaysBusiness 
            { 
                Id = 4, 
                SprintId = 2,
                MemberId = 1, 
                WorkingDays = 21 
            };

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
            MemberWorkingDays memberWorkingDays = new MemberWorkingDays 
            { 
                Id = 4, 
                SprintId = 2,
                MemberId = 1,
                WorkingDays = 21
            };
            _context.MemberWorkingDays.Add(memberWorkingDays);
            _context.SaveChanges();

            //Act
            var result = await _memberWorkingDaysRepository.DeleteAsync(_mapper.Map<MemberWorkingDaysBusiness>(memberWorkingDays));

            //Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region UpdateAsync
        [Test]
        public async System.Threading.Tasks.Task UpdateAsync_MemberWorkingDaysRepositoryReturnsTrue_ReturnsTrue()
        {
            //Arrange
            MemberWorkingDaysBusiness memberWorkingDays = new MemberWorkingDaysBusiness 
            { 
                Id = 1,
                SprintId = 1,
                MemberId = 1, 
                WorkingDays = 2 
            };

            //Act
            var result = await _memberWorkingDaysRepository.UpdateAsync(memberWorkingDays);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task UpdateAsync_MemberWorkingDaysRepositoryReturnsFalse_ReturnsFalse()
        {
            //Arrange
            MemberWorkingDaysBusiness memberWorkingDays = new MemberWorkingDaysBusiness
            {
                Id = 7, 
                SprintId = 1,
                MemberId = 1,
                WorkingDays = 2 
            };

            //Act
            var result = await _memberWorkingDaysRepository.UpdateAsync(memberWorkingDays);

            //Assert
            Assert.IsFalse(result);
        }
        #endregion
    }
}
