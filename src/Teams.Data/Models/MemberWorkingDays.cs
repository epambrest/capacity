using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Teams.Data.Models
{
    public class MemberWorkingDays
    {
        [Key]
        [Column("Id"), Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Column("SprintId"), ForeignKey(nameof(Sprint)), Required]
        public int SprintId { get; set; }
        public virtual Sprint Sprint { get; set; }
        
        [Column("MemberId"), ForeignKey(nameof(TeamMember)), Required]
        public int MemberId { get; set; }
        public virtual TeamMember TeamMember { get; set; }
        
        [Column("WorkingDays"), Required]
        public int WorkingDays { get; set; }
    }
}
