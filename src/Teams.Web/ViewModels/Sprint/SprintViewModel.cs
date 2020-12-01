namespace Teams.Web.ViewModels.Sprint
{
    public class SprintViewModel
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string Name { get; set; }
        public int DaysInSprint { get; set; }
        public int StoryPointInHours { get; set; }
        public bool IsActive { get; set; }
    }
}
