using System;

namespace Teams.Web.ViewModels.Home
{
    public class HealthViewModel
    {
        public string Version { get; set; }
        public bool IsDbConnected { get; set; }
        public DateTime DataServerTime { get; set; }
    }
}
