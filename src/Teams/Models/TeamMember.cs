using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Teams.Models
{
    public class TeamMember
    {
        [Column("Id", TypeName = "int")]
        [Required(AllowEmptyStrings = false)]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("TeamId", TypeName = "int")]
        [Required(AllowEmptyStrings = false)]
        [ForeignKey("Team_Id")]
        public virtual Team Team { get; set; }
        [Column("MemberId", TypeName = "nvarchar(450)")]
        [ForeignKey("AspNetUsers_Id")]
        [Required(AllowEmptyStrings = false)]
        public virtual string MemberId { get; set; }
    }
}
