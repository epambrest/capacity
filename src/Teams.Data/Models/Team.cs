using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Teams.Data.Models
{
    public class Team : IModel<Team>
    {
        [Key]
        [Column ("Id"), Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Column("TeamName"), Required]
        [StringLength(255, ErrorMessage = "Name length must be less than 255")]
        public string TeamName { get; set; }
        
        [ForeignKey(nameof(Owner)), Required]
        public string TeamOwner { get; set; }
        public virtual User Owner { get; set; }
        public virtual List<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();

        public void Update(Team model)
        {
            TeamName = model.TeamName;
            TeamOwner = model.TeamOwner;
        }

        public static async System.Threading.Tasks.Task DeleteDependEntentities(ApplicationDbContext _dbContext, int id)
        {
            var tasks = await _dbContext.Task.Where(t => t.TeamId == id).ToListAsync();

            foreach (var task in tasks)
            {
                _dbContext.Task.Remove(task);
            }

            var teamMembers = await _dbContext.TeamMembers.Where(t => t.TeamId == id).ToListAsync();

            foreach (var teamMember in teamMembers)
            {
                _dbContext.TeamMembers.Remove(teamMember);
            }
        }
    }
}
