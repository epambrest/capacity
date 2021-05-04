namespace Teams.Web.ViewModels.Task
{
    public class GetResultErrorViewModel
    {
        public string ErrorMessage { get; set; }

        private GetResultErrorViewModel(string errorMessage) => 
            ErrorMessage = errorMessage;

        public static GetResultErrorViewModel Create(string errorMessage) => 
            new GetResultErrorViewModel(errorMessage);
    }
}
