using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Teams.Models
{
    [Table("team_members")]
    public partial class TeamMembers
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Column("team_id")]
        [StringLength(450)]
        public string TeamId { get; set; }
        [Required]
        [Column("member_id")]
        [StringLength(450)]
        public string MemberId { get; set; }

        [ForeignKey(nameof(MemberId))]
        [InverseProperty(nameof(AspNetUsers.TeamMembers))]
        public virtual AspNetUsers Member { get; set; }
        [ForeignKey(nameof(TeamId))]
        [InverseProperty("TeamMembers")]
        public virtual Team Team { get; set; }
    }
}
