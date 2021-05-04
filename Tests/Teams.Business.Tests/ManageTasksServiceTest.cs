using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Teams.Business.Annotations;
using Teams.Business.Models;
using Teams.Business.Repository;
using Teams.Business.Services;
using Teams.Business.Structures;
using Teams.Security;

namespace Teams.Business.Tests
{
    [TestFixture]
    class ManageTasksServiceTest
    {
        private Mock<IRepository<Task, int>> _tasksRepository;
        private Mock<IRepository<Sprint, int>> _sprintRepository;
        private ManageTasksService _manageTasksService;
        private Mock<ICurrentUser> _currentUser;

        [SetUp]
        public void Setup()
        {
            _currentUser = new Mock<ICurrentUser>();
            _tasksRepository = new Mock<IRepository<Task, int>>();
            _sprintRepository = new Mock<IRepository<Sprint, int>>();
            _manageTasksService = new ManageTasksService(_tasksRepository.Object, _sprintRepository.Object, _currentUser.Object);
            var mock = getFakeDbTasks().AsQueryable().BuildMock();
            _tasksRepository.Setup(x => x.GetAllAsync()).Returns(System.Threading.Tasks.Task.FromResult(getFakeDbTasks()));
            _tasksRepository.Setup(t => t.DeleteAsync(It.IsAny<int>())).ReturnsAsync(true);
            _sprintRepository.Setup(x => x.GetAllAsync()).Returns(System.Threading.Tasks.Task.FromResult(getFakeDbSprints()));
        }

        [Test]
        public async System.Threading.Tasks.Task GetAllTasksInTeamAsync_RepositoryContainsTwoTasksForSelectedTeam_ReturnsTwoTasks()
        {
            //Arrange
            const int teamId = 1;

            //Act
            var result = await _manageTasksService.GetAllTasksForTeamAsync(teamId, new DisplayOptions { });

            //Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async System.Threading.Tasks.Task GetAllTasksInTeamAsync_RepositoryContainsZeroTasksForSelectedTeam_ReturnsNull()
        {
            //Arrange
            const int teamId = 2;

            //Act
            var result = await _manageTasksService.GetAllTasksForTeamAsync(teamId, new DisplayOptions { });

            //Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public async System.Threading.Tasks.Task GetTaskByIdAsync_ManageTasksServiceReturnsTaskWithId1_ReturnTaskWithId1()
        {
            // Arrange
            const int taskId = 1;

            Team team = Team.Create(9, "2", "1234", new List<TeamMember>());
            var task = new List<Task>()
            {
                Task.Create(1, 9, team, "Implement 4.5 issue", 3, "google.com", 9, null),
            };

            var mock = task.AsQueryable().BuildMock();

            _tasksRepository.Setup(x => x.GetAllAsync()).Returns(System.Threading.Tasks.Task.FromResult(getFakeDbTasks()));

            // Act
            var result = await _manageTasksService.GetTaskByIdAsync(taskId);

            // Assert
            Assert.AreEqual(1, result.Id);
        }

        [Test]
        public async System.Threading.Tasks.Task RemoveAsync_ManageTasksServiceReturnsTrue_ReturnsTrue()
        {
            //Arrange
            const string ownerId = "1";
            const int taskId = 1;
            var user = new Mock<UserDetails>(null);
            user.Setup(x => x.Id()).Returns(ownerId);
            user.Setup(x => x.Name()).Returns("name");
            _currentUser.SetupGet(x => x.Current).Returns(user.Object);

            //Act
            bool result = await _manageTasksService.RemoveAsync(taskId);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task RemoveAsync_ManageTasksServiceReturnsFalse_ReturnsFalse()
        {
            //Arrange
            const string ownerId = "5";
            const int taskId = 1;
            var user = new Mock<UserDetails>(null);
            user.Setup(x => x.Id()).Returns(ownerId);
            user.Setup(x => x.Name()).Returns("name");
            _currentUser.SetupGet(x => x.Current).Returns(user.Object);

            //Act
            bool result = await _manageTasksService.RemoveAsync(taskId);

            //Assert
            Assert.IsFalse(result);
        }

        private IEnumerable<Sprint> getFakeDbSprints()
        {
            var team = Team.Create(1, "1", "1234", new List<TeamMember>());
            
            var tasks = new List<Task>()
            {
                Task.Create(1, 1, Team.Create(1, "1", "1234", new List<TeamMember>()) ,"Task1", 3, "link1", 1, 1, true),
                Task.Create(1, 1, Team.Create(1, "2", "1234", new List<TeamMember>()) ,"Task2", 1, "link1", 1, 1, false),
                Task.Create(1, 1, Team.Create(1, "2", "1234", new List<TeamMember>()) ,"Task3", 4, "link1", 1, 1, true),
            };

            var sprints = new List<Sprint>
            {
                Sprint.Create(1, 1, team, "Sprint1", 14, 4, PossibleStatuses.ActiveStatus, tasks),

                Sprint.Create(2, 1, team, "Sprint2", 16, 4, PossibleStatuses.CompletedStatus, new List<Task>()),
            };

            return sprints;
        }

        private IEnumerable<Task> getFakeDbTasks()
        {
            var tasks = new List<Task>()
            {
                Task.Create(1, 1, Team.Create(1, "1", "1234", new List<TeamMember>()) ,"Task1", 1, "link1", 1, 1),
                Task.Create(1, 1, Team.Create(1, "2", "1234", new List<TeamMember>()) ,"Task1", 1, "link1", 1, 1),
                Task.Create(1, 3, Team.Create(1, "3", "1234", new List<TeamMember>()) ,"Task1", 3, "link1", 1, 1),
                Task.Create(1, 4, Team.Create(1, "4", "1234", new List<TeamMember>()) ,"Task1", 3, "link1", 1, 1),
            };
            return tasks;
        }

        private static IEnumerable<TestCaseData> GetTasksAllParamsForMemberTestsData
        {
            get
            {
                yield return new TestCaseData(TasksAllParams.Create(7, 1, 8, 2, 1, 8),1, 1);
                yield return new TestCaseData(TasksAllParams.Create(0, 0, 0, 0, 0, 0), 1, 2);
                yield return new TestCaseData(null, 0, 0);
            }
        }

        [Test, TestCaseSource(nameof(GetTasksAllParamsForMemberTestsData))]
        public async System.Threading.Tasks.Task GetTasksAllParamsForMemberTests_FromManageTaskService(TasksAllParams allParamsExcepted, 
            int teamMemberId,
            int sprintId)
        {
            //Act
            var result = await _manageTasksService.GetTasksAllParamsForMember(teamMemberId, sprintId);

            //Assert
            Assert.AreEqual(allParamsExcepted, result);
        }

        [Test]
        public async System.Threading.Tasks.Task EditTaskAsync_ManageTaskServiceReturns_True()
        {
            //Arrange
            Team team = Team.Create(1, "2", "1234", new List<TeamMember>());
            var task = Task.Create(1, 1, team, "Task", 3, "vk.com", 3, null);

            _tasksRepository.Setup(x => x.UpdateAsync(It.IsAny<Task>())).ReturnsAsync(true);
            _tasksRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(task);

            //Act
            var result = await _manageTasksService.EditTaskAsync(task);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task EditTaskAsync_ManageSpiritsServiceReturns_False()
        {
            //Arrange
            Team team = Team.Create(1, "2", "1234", new List<TeamMember>());
            var task1 = Task.Create(1, 1, team, "Ta  sk", 3, "vk.com", 3, 1);
            var task2 = Task.Create(1, 1, team, "Task", 3, "vkcom", 3, 1);
            var task3 = Task.Create(1, 1, team, "Task", -3, "vk.com", 3, 1);

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

        [TestCase("Task")]
        [TestCase("Task 1")]
        [TestCase("Task_1")]
        [TestCase("Task-1.")]
        public async System.Threading.Tasks.Task EditTaskAsync_ManageSpiritsServiceNameCheckReturns_true(string TaskName)
        {
            //Arrange
            Team team = Team.Create(1, "2", "1234", new List<TeamMember>());
            var task = Task.Create(1, 1, team, TaskName, 3, "vk.com", 3, 1);

            _tasksRepository.Setup(x => x.UpdateAsync(It.IsAny<Task>())).ReturnsAsync(true);
            _tasksRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(task);

            //Act
            var result= await _manageTasksService.EditTaskAsync(task); 

            //Assert
            Assert.IsTrue(result);
        }

        [TestCase("Ta  sk")]
        [TestCase("Task 1 ")]
        [TestCase("Task@1")]
        [TestCase("Task!1")]
        public async System.Threading.Tasks.Task EditTaskAsync_ManageSpiritsServiceNameCheckReturns_false(string TaskName)
        {
            //Arrange
            Team team = Team.Create(1, "2", "1234", new List<TeamMember>());
            var task = Task.Create(1, 1, team, TaskName, 3, "vk.com", 3, 1);

            _tasksRepository.Setup(x => x.UpdateAsync(It.IsAny<Task>())).ReturnsAsync(true);
            _tasksRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(task);

            //Act
            var result = await _manageTasksService.EditTaskAsync(task);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async System.Threading.Tasks.Task AddTaskAsync_ManageTaskServiceReturns_True()
        {
            //Arrange
            Team team = Team.Create(1, "2", "1234", new List<TeamMember>());
            var task = Task.Create(1, 1, team, "Task1", 1, "https://github.com/", 3, 1);

            _tasksRepository.Setup(x => x.InsertAsync(It.IsAny<Task>())).ReturnsAsync(true);

            //Act
            var result = await _manageTasksService.AddTaskAsync(task);

            //Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task AddTasksAsync_ManageTasksServiceReturns_False()
        {
            //Arrange
            Team team = Team.Create(1, "2", "1234", new List<TeamMember>());
            var task1 = Task.Create(1, 1, team, "Task 1@@ ", 1, "https://github.com/", 3, 1);
            var task2 = Task.Create(2, 2, team, "Task2 ", -5, "https://github.com/", 3, 1);
            var task3 = Task.Create(3, 3, team, "Task3 ", 3, "https:/ qq/github.com/", 3, 1);

            _tasksRepository.Setup(x => x.InsertAsync(It.IsAny<Task>())).ReturnsAsync(true);

            //Act
            var result1 = await _manageTasksService.AddTaskAsync(task1); //Incorrect name
            var result2 = await _manageTasksService.AddTaskAsync(task2); //Incorrect SP count
            var result3 = await _manageTasksService.AddTaskAsync(task3); //Incorrect link

            //Assert
            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
            Assert.IsFalse(result3);
        }

        [Test]
        public async System.Threading.Tasks.Task AddTaskAsync_ManageSpiritsServiceNameCheckReturns_true()
        {
            //Arrange
            Team team = Team.Create(1, "2", "1234", new List<TeamMember>());
            var task1 = Task.Create(1, 1, team, "Task", 3, "vk.com", 3, 1);
            var task2 = Task.Create(1, 1, team, "Task 1", 3, "vk.com", 3, 1);
            var task3 = Task.Create(1, 1, team, "Task_1", 3, "vk.com", 3, 1);
            var task4 = Task.Create(1, 1, team, "Task-1.", 3, "vk.com", 3, 1);

            _tasksRepository.Setup(x => x.InsertAsync(It.IsAny<Task>())).ReturnsAsync(true);

            //Act
            var result1 = await _manageTasksService.AddTaskAsync(task1);
            var result2 = await _manageTasksService.AddTaskAsync(task2);
            var result3 = await _manageTasksService.AddTaskAsync(task3);
            var result4 = await _manageTasksService.AddTaskAsync(task4);

            //Assert
            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
            Assert.IsTrue(result3);
            Assert.IsTrue(result4);
        }

        [Test]
        public async System.Threading.Tasks.Task AddTaskAsync_ManageSpiritsServiceNameCheckReturns_false()
        {
            //Arrange
            Team team = Team.Create(1, "2", "1234", new List<TeamMember>());
            var task1 = Task.Create(1, 1, team, "Ta  sk", 3, "vk.com", 3, 1);
            var task2 = Task.Create(1, 1, team, "Task 1 ", 3, "vk.com", 3, 1);
            var task3 = Task.Create(1, 1, team, "Task@1", 3, "vk.com", 3, 1);
            var task4 = Task.Create(1, 1, team, "Task!1", 3, "vk.com", 3, 1);

            _tasksRepository.Setup(x => x.InsertAsync(It.IsAny<Task>())).ReturnsAsync(true);

            //Act
            var result1 = await _manageTasksService.AddTaskAsync(task1);
            var result2 = await _manageTasksService.AddTaskAsync(task2);
            var result3 = await _manageTasksService.AddTaskAsync(task3);
            var result4 = await _manageTasksService.AddTaskAsync(task4);


            //Assert
            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
            Assert.IsFalse(result3);
            Assert.IsFalse(result4);
        }
    }
}
