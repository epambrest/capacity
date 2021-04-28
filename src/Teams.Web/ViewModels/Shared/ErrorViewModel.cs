namespace Teams.Web.ViewModels.Shared
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        private ErrorViewModel(string requestId)
        {
            RequestId = requestId;
        }

        public static ErrorViewModel Create(string requestId)
        {
            return new ErrorViewModel(requestId);
        }
    }
}
