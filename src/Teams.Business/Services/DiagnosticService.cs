using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;
using Teams.Data;

namespace Teams.Business.Services
{
    public class DiagnosticService : IDiagnosticService
    {
        private readonly string pathToConfig;
        private readonly ApplicationDbContext _dbContext;
        public DiagnosticService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            pathToConfig = "appsettings.json";     
        }

        public async Task<bool> CheckDbConnection()
        {
            try
            {
                if (await _dbContext.Database.CanConnectAsync())
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public string GetCurrentVersion()
        {
            string currentVersion;
            try
            {
                using (StreamReader sr = new StreamReader(pathToConfig))
                {
                    string json = sr.ReadToEnd();
                    var jsonData = (JObject)JsonConvert.DeserializeObject(json);
                    currentVersion = jsonData["Version"].ToString();
                    if (currentVersion != null && currentVersion != "")
                        return currentVersion;
                    else
                        throw new Exception("Version hasn't been got");
                }
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }

        public DateTime GetServerDataTime()
        {
            return DateTime.Now;
        }
    }
}
