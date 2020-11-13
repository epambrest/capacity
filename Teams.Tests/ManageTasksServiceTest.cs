using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teams.Data;
using Teams.Models;
using Teams.Security;
using Teams.Services;
using Task = Teams.Models.Task;

namespace Teams.Tests
{
    [TestFixture]
    class ManageTasksServiceTest
    {
        private Mock<IRepository<Task, int>> _tasksRepository;
        private ManageTasksService _manageTasksService;

        [SetUp]
        public void Setup()
        {
            _tasksRepository = new Mock<IRepository<Task, int>>();
            _manageTasksService = new ManageTasksService(_tasksRepository.Object);
            var mock = getFakeDbTasks().AsQueryable().BuildMock();
            _tasksRepository.Setup(x => x.GetAll()).Returns(mock.Object);
        }

        [Test]
        public async System.Threading.Tasks.Task GetAllTasksInTeamAsync_RepositoryContainsTwoTasksForSelectedTeam_ReturnsTwoTasks()
        {
            //Arrange
            const int teamId = 1;

            //Act
            var result = new List<Task>(await _manageTasksService.GetAllTasksForTeamAsync(teamId, new DisplayOptions { }));

            //Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async System.Threading.Tasks.Task GetAllTasksInTeamAsync_RepositoryContainsZeroTasksForSelectedTeam_ReturnsNull()
        {
            //Arrange
            const int teamId = 2;

            //Act
            var result = new List<Task>(await _manageTasksService.GetAllTasksForTeamAsync(teamId, new DisplayOptions { }));

            //Assert
            Assert.IsEmpty(result);
        }

        private List<Task> getFakeDbTasks()
        {
            var tasks = new List<Task>()
            {
                new Task{Id = 1,SprintId = 1,TeamId = 1},
                new Task{Id = 2,SprintId = 1,TeamId = 1},
                new Task{Id = 3,SprintId = 3,TeamId = 3},
                new Task{Id = 4,SprintId = 3,TeamId = 3},
            };
            return tasks;
        }
        
        [Test]
        public async System.Threading.Tasks.Task GetTaskByIdAsync_ManageTasksServiceReturnsTaskWithId1_ReturnTaskWithId1()
        {
            // Arrange
            const int taskId = 1;

            var task = new List<Models.Task>()
            {
                new Models.Task
                {
                    Id = 1,
                    Link = "google.com",
                    MemberId = 1,
                    Name = "Implement 4.5 issue",
                    StoryPoints = 3,
                    TeamMember = null,
                    Sprint = null,
                    SprintId = 9,
                    Team = null,
                    TeamId = 9
                }
            };

            var mock = task.AsQueryable().BuildMock();

            _tasksRepository.Setup(x => x.GetAll()).Returns(mock.Object);

            // Act
            var result = await _manageTasksService.GetTaskByIdAsync(taskId);

            // Assert
            Assert.AreEqual(1, result.Id);
        }

        [Test]
        public async System.Threading.Tasks.Task EditTaskAsync_ManageTaskServiceReturns_True()
        {
            //Arrange
            var task = new Task { Id = 1, TeamId = 1, Name = "Task", MemberId = 1, Link = "vk.com", StoryPoints = 3, SprintId =3 };

            _tasksRepository.Setup(x => x.UpdateAsync(It.IsAny<Task>())).ReturnsAsync(true);
            _tasksRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(task);

            //Act
            var result = await _manageTasksService.EditTaskAsync(task);

            //Assert
            Assert.IsTrue(result);
        }

        public async System.Threading.Tasks.Task EditTaskAsync_ManageSpiritsServiceReturns_False()
        {
            //Arrange
            var task1 = new Task { Id = 1, TeamId = 1, Name = "Ta sk", MemberId = 1, Link = "vk.com", StoryPoints = 3, SprintId = 3 };
            var task2 = new Task { Id = 1, TeamId = 1, Name = "Task", MemberId = 1, Link = "vkcom", StoryPoints = 3, SprintId = 3 };
            var task3 = new Task { Id = 1, TeamId = 1, Name = "Task", MemberId = 1, Link = "vk.com", StoryPoints = -3, SprintId = 3 };

            _tasksRepository.Setup(x => x.UpdateAsync(It.IsAny<Task>())).ReturnsAsync(true);
            _tasksRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(task2);

            //Act
            var result1 = await _manageTasksService.EditTaskAsync(task1); //Incorrect name
            var result2 = await _manageTasksService.EditTaskAsync(task2); //Incorrect link count
            var result3 = await _manageTasksService.EditTaskAsync(task3); //Incorrect SP count


            //Assert
            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
            Assert.IsFalse(result3);
        }
    }
}
