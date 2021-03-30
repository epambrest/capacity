using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Teams.Data.Models
{
    public class Team
    {
        [Key]
        [Column ("Id"), Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("TeamName"), Required]
        [StringLength(255, ErrorMessage = "Name length must be less than 255")]
        public string TeamName { get; set; }
        [ForeignKey(nameof(Owner)), Required]
        public string TeamOwner { get; set; }
        public virtual IdentityUser Owner { get; set; }
        public virtual ICollection<TeamMember> TeamMembers { get; set; }
    }
}
