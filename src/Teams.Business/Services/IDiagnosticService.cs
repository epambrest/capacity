using System;
using System.Threading.Tasks;

namespace Teams.Business.Services
{
    public interface IDiagnosticService
    {
        string GetCurrentVersion();
        Task<bool> CheckDbConnection();
        DateTime GetServerDataTime();
    }
}
