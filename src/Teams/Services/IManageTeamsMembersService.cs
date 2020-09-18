namespace Teams.Services
{
    public interface IManageTeamsMembersService
    {
        public void Add(int team_id, string member_id);
    }
}
