namespace Teams.Security
{
    public interface ICurrentUser
    {
        string GetName();
        string GetId();
    }
}
