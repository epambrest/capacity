using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Teams.Models
{
    public class TeamMember
    {
        [Column("Id", TypeName = "int"), Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("TeamId", TypeName = "int"), ForeignKey(nameof(Team)), Required] 
        public int TeamId { get; set; }
        public virtual Team Team { get; set; }
        [Column("MemberId", TypeName = "nvarchar(450)"), ForeignKey("AspNetUsers"), Required]
        public virtual string MemberId { get; set; }
    }
}
