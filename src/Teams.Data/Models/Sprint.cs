using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [StringLength(255, ErrorMessage = "Name length must be less than 255")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Days count required")]
        [Range(1, 99999, ErrorMessage = "Please enter a value greater than 0")]
        [Column("DaysInSprint")]
        public int DaysInSprint { get; set; }
        
        [Required(ErrorMessage = "Enter how much hours story point cost")]
        [Range(1, 99999, ErrorMessage = "Please enter a value greater than 0")]
        [Column("StoryPointInHours")]
        public int StoryPointInHours { get; set; }

        [Column("Status"), DefaultValue(0), Range(0 , 2, ErrorMessage = "Possible only 0, 1, 2 values")]
        public int Status { get; set; }

        [ForeignKey("SprintId")]
        public virtual ICollection<Task> Tasks { get; set; }
        public virtual ICollection<MemberWorkingDays> MemberWorkingDays { get; set; }
    }
}
