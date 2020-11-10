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
    }
}
