using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Teams.Business.Annotations;
using Teams.Business.Repository;
using Teams.Business.Security;
using Teams.Business.Structures;
using Teams.Security;


namespace Teams.Business.Services
{
    public class ManageTasksService : IManageTasksService
    {
        private readonly IRepository<Models.Task, int> _taskRepository;
        private readonly IRepository<Models.Sprint, int> _sprintRepository;
        private readonly ICurrentUser _currentUser;

        public ManageTasksService(IRepository<Models.Task, int> taskRepository, 
            IRepository<Models.Sprint, int> sprintRepository,
            ICurrentUser currentUser)
        {
            _taskRepository = taskRepository;
            _sprintRepository = sprintRepository;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<Models.Task>> GetAllTasksForTeamAsync(int teamId, DisplayOptions options)
        {
            var allTasks = await _taskRepository.GetAllAsync();
            var allTasksForTean = allTasks.Where(x => x.TeamId == teamId);

            if (options.SortDirection == SortDirection.Ascending)
                return allTasksForTean.OrderBy(x => x.Name);
            else
                return allTasksForTean.OrderByDescending(x => x.Name);
        }

        public async Task<Models.Task> GetTaskByIdAsync(int id)
        {
            var allTasks = await _taskRepository.GetAllAsync();
            //
            var task = allTasks.Where(t => t.Id == id).FirstOrDefault();
            return task;
        }

        public async Task<bool> EditTaskAsync(Models.Task task)
        {
            var taskForCheck = await _taskRepository.GetByIdAsync(task.Id);
            bool nameCheck;
            
            if (taskForCheck.Name == task.Name)
            {
                nameCheck = false;
            }
            else
            {
                var allTasks = await _taskRepository.GetAllAsync();
                nameCheck = allTasks.Where(x => x.TeamId == task.TeamId).Any(x => x.Name == task.Name);
            }

            if (nameCheck || task.StoryPoints <= 0 || !Regex.IsMatch(task.Link, @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$") 
                || !Regex.IsMatch(task.Name, "^[a-zA-Z0-9-_.]+( [a-zA-Z0-9-_.]+)*$"))
            {
                return false;
            }
            return await _taskRepository.UpdateAsync(task);
        }

        public async Task<bool> RemoveAsync(int taskId)
        {
            var allTasks = await _taskRepository.GetAllAsync();
            var task = allTasks.FirstOrDefault(x => x.Id == taskId && x.Team.TeamOwner == _currentUser.Current.Id());
            
            if (task == null) return false;
            
            var result = await _taskRepository.DeleteAsync(taskId);
            return result;
        }

        public async Task<bool> AddTaskAsync(Models.Task task)
        {
            var allTasks = await _taskRepository.GetAllAsync();
            if (allTasks.Where(t => t.SprintId == task.SprintId).Any(t => t.Name == task.Name || t.Link == task.Link) || task.StoryPoints <= 0
                || !Regex.IsMatch(task.Link, @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$")
                || !Regex.IsMatch(task.Name, "^[a-zA-Z0-9-_.]+( [a-zA-Z0-9-_.]+)*$"))
            {
                return false;
            }
            return await _taskRepository.InsertAsync(task);
        }

        public async Task<TasksAllParams> GetTasksAllParamsForMember(int teamMemberId, int sprintId)
        {
            var allSprints = await _sprintRepository.GetAllAsync();
            var currentSprint = allSprints.FirstOrDefault(s => s.Id == sprintId);
            if (currentSprint == null) return TasksAllParams.Create(0, 0, 0, 0, 0, 0, 0);
            else if (currentSprint.Tasks == null) return TasksAllParams.Create(0, 0, 0, 0, 0, 0, 0);

            var allMemberTasks = currentSprint.Tasks.Where(t => t.MemberId == teamMemberId).ToList();
            var sprint = allSprints.Where(t => t.Id == sprintId).FirstOrDefault(t => t.Id == sprintId);
            var spCompletedTasks = 0;
            var spUnCompletedTasks = 0;
            var totalStoryPoints = 0;

            foreach (var memberTask in allMemberTasks)
            {
                if (memberTask.Completed == true) spCompletedTasks += memberTask.StoryPoints;
                else spUnCompletedTasks += memberTask.StoryPoints;

                totalStoryPoints += memberTask.StoryPoints;
            }

            var quantityСompletedTasks = allMemberTasks.Count(t => t.Completed == true);
            var quantityUnСompletedTasks = allMemberTasks.Count(t => t.Completed == false);
            var teamMemberTotalSp = spCompletedTasks + spUnCompletedTasks;
            var tasksAllParams = TasksAllParams.Create(spCompletedTasks, spUnCompletedTasks, totalStoryPoints, quantityСompletedTasks, quantityUnСompletedTasks, teamMemberTotalSp);

            return tasksAllParams;
        }
    }
}
