﻿using AutoMapper;
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
    class SprintRepositoryTest
    {
        private ApplicationDbContext _context;
        private IRepository<SprintBusiness, int> _sprintRepository;
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "SprintDatabase").Options;

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MemberWorkingDaysProfile());
                mc.AddProfile(new SprintProfile());
                mc.AddProfile(new TaskProfile());
                mc.AddProfile(new TeamProfile());
                mc.AddProfile(new UserProfile());
                mc.AddProfile(new TeamMemberProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            _mapper = mapper;

            _context = new ApplicationDbContext(options);

            _sprintRepository = new SprintRepository(_context, _mapper);
        }

        private IEnumerable<Sprint> GetFakeSprintDb()
        {
            var data = new List<Sprint>
            {
                new Sprint
                {
                    Id = 1,
                    DaysInSprint = 3,
                    Name = "Sprint1",
                    TeamId = 1,
                    StoryPointInHours = 4,
                    Status = PossibleStatuses.ActiveStatus,
                    Team = new Team() {},
                    Tasks = new List<Models.Task>(),
                    MemberWorkingDays = new List<MemberWorkingDays>() {new MemberWorkingDays() }  
                },

                new Sprint
                {
                    Id = 2, 
                    DaysInSprint = 3, 
                    Name = "Sprint2", 
                    TeamId = 2, 
                    StoryPointInHours = 4, 
                    Status = PossibleStatuses.CompletedStatus,
                    Team = new Team() {},
                    Tasks = new List<Models.Task>(),
                    MemberWorkingDays = new List<MemberWorkingDays>() {new MemberWorkingDays() }
                },

                new Sprint 
                { 
                    Id = 3, 
                    DaysInSprint = 3, 
                    Name = "Sprint3", 
                    TeamId = 3, 
                    StoryPointInHours = 4, 
                    Status = PossibleStatuses.CompletedStatus, 
                    Team = new Team() {}, 
                    Tasks = new List<Models.Task>(),
                    MemberWorkingDays = new List<MemberWorkingDays>() {new MemberWorkingDays() }
                },

                new Sprint 
                { 
                    Id = 4, 
                    DaysInSprint = 3, 
                    Name = "Sprint4", 
                    TeamId = 4, 
                    StoryPointInHours = 4, 
                    Status = PossibleStatuses.CompletedStatus, 
                    Team = new Team() {}, 
                    Tasks = new List<Models.Task>(),
                    MemberWorkingDays = new List<MemberWorkingDays>() {new MemberWorkingDays() }
                },

                new Sprint 
                { 
                    Id = 5, 
                    DaysInSprint = 3, 
                    Name = "Sprint5", 
                    TeamId = 5, 
                    StoryPointInHours = 4,
                    Status = PossibleStatuses.CompletedStatus,
                    Team = new Team() {},
                    Tasks = new List<Models.Task>(),
                    MemberWorkingDays = new List<MemberWorkingDays>() {new MemberWorkingDays()}
                }
            };
            return data;
        }

        [Test]
        public async System.Threading.Tasks.Task GetAll_SprintRepositoryReturns_ListCount5()
        {
            //Arrange
            const int sprintCount = 5;
            _context.Sprint.AddRange(GetFakeSprintDb());
            _context.SaveChanges();
            
            //Act
            var result = await _sprintRepository.GetAllAsync();

            //Assert
            Assert.AreEqual(result.Count(), sprintCount);
        }

        [Test]
        public async System.Threading.Tasks.Task GetByIdAsync_SprintRepositoryReturns_Id3()
        {
            //Arrange
            const int sprintId = 3;

            //Act
            var result = await _sprintRepository.GetByIdAsync(sprintId);

            //Assert
            Assert.AreEqual(sprintId, result.Id);
        }

        [Test]
        public async System.Threading.Tasks.Task GetByIdAsync_SprintRepositoryReturns_Null()
        {
            //Arrange
            const int sprintId = 30;

            //Act
            var result = await _sprintRepository.GetByIdAsync(sprintId);

            //Assert
            Assert.IsNull(result);
        }

        [Test]
        public async System.Threading.Tasks.Task InsertAsync_SprintRepositoryReturns_True()
        {
            //Arrange
            SprintBusiness sprint = new SprintBusiness 
            { 
                Id = 6,
                DaysInSprint = 3, 
                Name = "Sprint6", 
                TeamId = 6,
                StoryPointInHours = 4,
                Status = PossibleStatuses.CompletedStatus 
            };

            //Act
            var result = await _sprintRepository.InsertAsync(sprint);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task DeleteAsync_SprintRepositoryReturns_True()
        {
            //Arrange
            Sprint sprint = new Sprint 
            {
                Id = 3,
                DaysInSprint = 3, 
                Name = "Sprint3",
                TeamId = 3,
                StoryPointInHours = 4,
                Status = PossibleStatuses.CompletedStatus 
            };
            _context.Sprint.Add(sprint);
            _context.SaveChanges();

            //Act
            var result = await _sprintRepository.DeleteAsync(_mapper.Map<SprintBusiness>(sprint));

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task UpdateAsync_SprintRepositoryReturns_True()
        {
            //Arrange
            SprintBusiness sprint = new SprintBusiness 
            { 
                Id = 2, 
                DaysInSprint = 3, 
                Name = "Update", 
                TeamId = 5, 
                StoryPointInHours = 4, 
                Status = PossibleStatuses.CompletedStatus 
            };

            //Act
            var result = await _sprintRepository.UpdateAsync(sprint);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task UpdateAsync_SprintRepositoryReturns_False()
        {
            //Arrange
            SprintBusiness sprint = new SprintBusiness 
            {
                Id = 10,
                DaysInSprint = 3, 
                Name = "Sprint5",
                TeamId = 6,
                StoryPointInHours = 4,
                Status = PossibleStatuses.CompletedStatus 
            };

            //Act
            var result = await _sprintRepository.UpdateAsync(sprint);

            //Assert
            Assert.IsFalse(result);
        }
    }
}