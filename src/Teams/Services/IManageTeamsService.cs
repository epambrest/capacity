using System.Threading.Tasks;

    public interface IManageTeamsService
    {
        Task<bool> RemoveAsync(int team_id);
    }