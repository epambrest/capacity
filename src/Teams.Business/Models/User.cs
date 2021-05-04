namespace Teams.Business.Models
{
    public class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        private User(string id, string userName, string firstName, string lastName)
        {
            Id = id;
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
        }

        public User() 
        { 
        }
        
        public static User Create(string id, string userName, string firstName, string lastName) => 
            new User(id, userName, firstName, lastName);
    }
}
