using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Teams.Models
{
    public partial class Team
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("team_name")]
        public string TeamName { get; set; }
        [Column("team_owner")]
        public string TeamOwner { get; set; }

        public ICollection<TeamMembers> TeamMembers { get; set; }
    }
}
