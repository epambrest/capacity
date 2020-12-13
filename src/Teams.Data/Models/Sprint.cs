using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.Collections.Generic;

namespace Teams.Data.Models
{
    public class Sprint
    {
        [Key]
        [Required]
        [Column("Id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Column("TeamId"), ForeignKey(nameof(Team))]
        public int TeamId { get; set; }
        public virtual Team Team { get; set; }
        [Required(ErrorMessage = "Name required")]
        [Column("Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Days count required")]
        [Range(1, 99999, ErrorMessage = "Please enter a value greater than 0")]
        [Column("DaysInSprint")]
        public int DaysInSprint { get; set; }
        [Required(ErrorMessage = "Enter how much hours story point cost")]
        [Range(1, 99999, ErrorMessage = "Please enter a value greater than 0")]
        [Column("StoryPointInHours")]
        public int StoryPointInHours { get; set; }
        [Column("IsActive"), DefaultValue(false)]
        public bool IsActive { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        public virtual ICollection<MemberWorkingDays> MemberWorkingDays { get; set; }
    }
}
