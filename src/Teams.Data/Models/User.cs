using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Teams.Data.Models
{
    public class User : IdentityUser
    {
        [MaxLength(35)]
        public string FirstName { get; set; }
        
        [MaxLength(35)]
        public string LastName { get; set; }
    }
}
