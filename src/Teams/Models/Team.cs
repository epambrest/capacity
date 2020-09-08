using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Teams.Models
{
    [Table("team")]
    public partial class Team
    {
        public Team()
        {
            TeamMembers = new HashSet<TeamMembers>();
        }

        [Key]
        public string Id { get; set; }
        [Column("team_name")]
        [StringLength(256)]
        public string TeamName { get; set; }
        [Column("team_owner")]
        [StringLength(256)]
        public string TeamOwner { get; set; }

        [InverseProperty("Team")]
        public virtual ICollection<TeamMembers> TeamMembers { get; set; }
    }
}
