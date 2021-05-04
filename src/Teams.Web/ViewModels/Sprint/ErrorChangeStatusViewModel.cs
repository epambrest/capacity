namespace Teams.Web.ViewModels.Sprint
{
    public class ErrorChangeStatusViewModel
    {
        public string SprintName { get; set; }
        public string ErrorMessage { get; set; }

        private ErrorChangeStatusViewModel(string sprintName, string errorMessage)
        {
            SprintName = sprintName;
            ErrorMessage = errorMessage;
        }

        public ErrorChangeStatusViewModel()
        {
        }

        public static ErrorChangeStatusViewModel Create(string sprintName, string errorMessage) => 
            new ErrorChangeStatusViewModel(sprintName, errorMessage);
    }
}
