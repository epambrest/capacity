using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Teams.Models
{
    public class Team
    {
        [Key]
        [Column ("Id", TypeName = "int"), Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("TeamName", TypeName = "nvarchar(MAX"), Required]
        public string TeamName { get; set; }
        [Column("TeamOwner", TypeName = "nvarchar(MAX"), Required(AllowEmptyStrings = false)]
        public string TeamOwner { get; set; }

        public ICollection<TeamMember> TeamMembers { get; set; }
    }
}
