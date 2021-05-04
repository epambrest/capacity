namespace Teams.Business.Security
{
    public interface ICurrentUser
    {
        UserDetails Current { get;}
    }
}
