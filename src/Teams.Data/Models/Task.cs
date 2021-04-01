using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Teams.Data.Models
{
    public class Task
    {
        [Key]
        [Column("Id"), Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("TeamId"), ForeignKey(nameof(Team))]
        public int TeamId { get; set; }
        public virtual Team Team { get; set; }
        [Column("Name"), Required]
        public string Name { get; set; }
        [Column("SprintId"), ForeignKey(nameof(Sprint))]
        public int? SprintId { get; set; }
        public virtual Sprint Sprint { get; set; }
        [Column("MemberId"), ForeignKey(nameof(TeamMember))]
        public int? MemberId{ get; set; }
        public virtual TeamMember TeamMember { get; set; }
        [Column("Link"), Required]
        public string Link { get; set; }
        [Column("StoryPoints"), Required]
        public int StoryPoints { get; set; }
        [Column("Completed"), Required, DefaultValue(false)]
        public bool Completed { get; set; }
    }
}
