using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Teams.Models
{
    public class Team
    {
        [Column ("Id", TypeName = "int")]
        [Required(AllowEmptyStrings = false)]
        public int Id { get; set; }
        [Column("TeamName", TypeName = "nvarchar(MAX")]
        [Required(AllowEmptyStrings = true)]
        public string TeamName { get; set; }
        [Column("TeamOwner", TypeName = "nvarchar(MAX")]
        [Required(AllowEmptyStrings = true)]
        public string TeamOwner { get; set; }

        public ICollection<TeamMember> TeamMembers { get; set; }
    }
}
