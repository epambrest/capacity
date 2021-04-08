using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Teams.Data.Models
{
    public class TeamMember 
    {
        [Column("Id", TypeName = "int"), Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Column("TeamId"), ForeignKey(nameof(Team))] 
        public int? TeamId { get; set; }
        public virtual Team Team { get; set; }

        [ForeignKey(nameof(Member)), Required]
        public string MemberId { get; set; }
        public virtual User Member { get; set; }
    }
}
