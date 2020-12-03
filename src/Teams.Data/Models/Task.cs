using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Teams.Data.Models
{
    public class Task
    {
        [Key]
        [Column("Id"), Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("TeamId"), ForeignKey(nameof(Team)), Required]
        public int TeamId { get; set; }
        public virtual Team Team { get; set; }
        [Column("Name"), Required]
        public string Name { get; set; }
        [Column("SprintId"), ForeignKey(nameof(Sprint)), Required]
        public int SprintId { get; set; }
        public virtual Sprint Sprint { get; set; }
        [Column("MemberId"), ForeignKey(nameof(TeamMember)), Required]
        public int MemberId{ get; set; }
        public virtual TeamMember TeamMember { get; set; }
        [Column("Link"), Required]
        public string Link { get; set; }
        [Column("StoryPoints"), Required]
        public int StoryPoints { get; set; }
        [Column("Completed"), Required]
        public bool Completed { get; set; }
    }
}
