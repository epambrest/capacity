using System.ComponentModel.DataAnnotations.Schema;

namespace Teams.Models
{
    public partial class TeamMembers
    {
        public int Id { get; set; }
        [ForeignKey("team_Id")]
        [Column("team_id")]
        public int TeamId { get; set; }
        [ForeignKey("AspNetUsers_Id")]
        [Column("member_id")]
        public string MemberId { get; set; }
    }
}
