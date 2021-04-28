using System;

namespace Teams.Web.ViewModels.Home
{
    public class HealthViewModel
    {
        public string Version { get; set; }
        public bool IsDbConnected { get; set; }
        public DateTime ServerDateTime { get; set; }

        private HealthViewModel(string version, bool isDbConnected)
        {
            Version = version;
            IsDbConnected = isDbConnected;
            ServerDateTime = DateTime.Now;
        }
        public static HealthViewModel Create(string version, bool isDbConnected)
        {
            HealthViewModel healthViewModel = new HealthViewModel(version, isDbConnected);
            return healthViewModel;
        }
    }
}
