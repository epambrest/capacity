using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;

namespace Teams.Models
{
    public class Sprint
    {
        [Key]
        [Column("Id"), Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("TeamId"), ForeignKey(nameof(Team)), Required]
        public int TeamId { get; set; }
        public virtual Team Team { get; set; }
        [Column("Name"), Required]
        public string Name { get; set; }
        [Column("DaysInSprint"), Required]
        public int DaysInSprint { get; set; }
        [Column("StorePointInHours"), Required]
        public int StorePointInHours { get; set; }
        [Column("IsActive"), DefaultValue(false)]
        public bool IsActive { get; set; }
    }
}
