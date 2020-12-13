using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Teams.Web.ViewModels.MemberWorkingDays
{
    public class MemberWorkingDaysViewModels
    {       
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int SprintId { get; set; }
        public int WorkingDays { get; set; }
    }
}
