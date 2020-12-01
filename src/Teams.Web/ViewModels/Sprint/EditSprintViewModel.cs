namespace Teams.Web.ViewModels.Sprint
{
    public class EditSprintViewModel
    {
        public string SprintName { get; set; }
        public int SprintId { get; set; }
        public int SprintDaysInSprint { get; set; }
        public int SprintStorePointInHours { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsActive { get; set; }
    }
}
