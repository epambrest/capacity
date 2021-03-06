﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Teams.Business.Models;
using Teams.Business.Repository;
using Teams.Data.Mappings;
using Teams.Data.Repository;

namespace Teams.Data.Tests
{
    [TestFixture]
    class MemberWorkingDaysRepositoryTest
    {
        private ApplicationDbContext _context;
        private IRepository<MemberWorkingDays, int> _memberWorkingDaysRepository;
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            var serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase()
                .AddEntityFrameworkProxies().BuildServiceProvider();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "MemberWorkingDaysDatabase").UseInternalServiceProvider(serviceProvider)
                .UseLazyLoadingProxies().Options;

            _context = new ApplicationDbContext(options);
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.ShouldUseConstructor = mc => !mc.IsPrivate;
                mc.AddProfile(new MemberWorkingDaysProfile());
                mc.AddProfile(new SprintProfile());
                mc.AddProfile(new TeamMemberProfile());
                mc.AddProfile(new UserProfile());
                mc.AddProfile(new TeamProfile());
                mc.AddProfile(new TaskProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            _mapper = mapper;

            _memberWorkingDaysRepository = new Repository<Models.MemberWorkingDays, MemberWorkingDays, int>(_context, _mapper);
        }

        private IQueryable<Models.MemberWorkingDays> GetFakeMemberWorkingDaysDb()
        {
            var data = new List<Models.MemberWorkingDays>
            {
                new Models.MemberWorkingDays 
                { 
                    Id = 1, 
                    SprintId = 1, 
                    MemberId = 1, 
                    WorkingDays = 27,
                    Sprint = new Models.Sprint(),
                    TeamMember = new Models.TeamMember()
                },

                new Models.MemberWorkingDays 
                { 
                    Id = 2, 
                    SprintId = 2, 
                    MemberId = 1, 
                    WorkingDays = 18,
                    Sprint = new Models.Sprint(),
                    TeamMember = new Models.TeamMember()
                },

                new Models.MemberWorkingDays 
                { 
                    Id = 3, 
                    SprintId = 1, 
                    MemberId = 3, 
                    WorkingDays = 2,
                    Sprint = new Models.Sprint(),
                    TeamMember = new Models.TeamMember()
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
            _context.MemberWorkingDays.AddRange(GetFakeMemberWorkingDaysDb());
            _context.SaveChanges();
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
            Sprint sprint = Sprint.Create(2, new List<Task>());
            MemberWorkingDays memberWorkingDays = MemberWorkingDays.Create(1, 2, sprint, 21);

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
                        Sprint sprint = Sprint.Create(2, new List<Task>());
            Models.MemberWorkingDays memberWorkingDays = new Models.MemberWorkingDays 
            { 
                Id = 4, 
                SprintId = 2,
                MemberId = 1,
                WorkingDays = 21
            };
            _context.MemberWorkingDays.Add(memberWorkingDays);
            _context.SaveChanges();

            //Act
            var result = await _memberWorkingDaysRepository.DeleteAsync(memberWorkingDays.Id);

            //Assert
            Assert.IsTrue(result);
        }
        #endregion

        #region UpdateAsync
        [Test]
        public async System.Threading.Tasks.Task UpdateAsync_MemberWorkingDaysRepositoryReturnsTrue_ReturnsTrue()
        {
            //Arrange
            Sprint sprint = Sprint.Create(1, new List<Task>());
            MemberWorkingDays memberWorkingDays = MemberWorkingDays.Create(1, 1, 1, sprint, 2);

            _context.MemberWorkingDays.AddRange(GetFakeMemberWorkingDaysDb());
            _context.SaveChanges();

            //Act
            var result = await _memberWorkingDaysRepository.UpdateAsync(memberWorkingDays);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task UpdateAsync_MemberWorkingDaysRepositoryReturnsFalse_ReturnsFalse()
        {
            //Arrange
            Sprint sprint = Sprint.Create(1, new List<Task>());
            MemberWorkingDays memberWorkingDays = MemberWorkingDays.Create(1, 1, sprint, 2);

            //Act
            var result = await _memberWorkingDaysRepository.UpdateAsync(memberWorkingDays);

            //Assert
            Assert.IsFalse(result);
        }
        #endregion
    }
}
