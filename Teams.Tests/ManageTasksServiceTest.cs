using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using Teams.Data;
using Teams.Services;
using Task = System.Threading.Tasks.Task;
using System.Collections.Generic;
using System.Linq;

namespace Teams.Tests
{
    [TestFixture]
    class ManageTasksServiceTest
    {
        private Mock<IRepository<Models.Task, int>> _taskReposotiry;
        private ManageTasksService _manageTasksService;

        [SetUp]
        public void Setup()
        {
            _taskReposotiry = new Mock<IRepository<Models.Task, int>>();
            _manageTasksService = new ManageTasksService(_taskReposotiry.Object);
        }

        [Test]
        public async Task GetTaskByIdAsync_ManageTasksServiceReturnsTaskWithId1_ReturnTaskWithId1()
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

            _taskReposotiry.Setup(x => x.GetAll()).Returns(mock.Object);

            // Act
            var result = await _manageTasksService.GetTaskByIdAsync(taskId);

            // Assert
            Assert.AreEqual(1, result.Id);
        }
    }
}