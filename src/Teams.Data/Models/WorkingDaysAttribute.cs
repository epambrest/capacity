using System.ComponentModel.DataAnnotations;

namespace Teams.Data.Models
{
    class WorkingDaysAttribute : ValidationAttribute
    {
        public WorkingDaysAttribute()
        {
            ErrorMessage = "The number of working days for the member must be less than or equal to the number of days in the sprint and greater than or equal to 0.";
        }

        public override bool IsValid(object value)
        { 
            MemberWorkingDays memberWorkingDays = value as MemberWorkingDays;
            if(memberWorkingDays.WorkingDays >= 0 && memberWorkingDays.WorkingDays <= memberWorkingDays.Sprint.DaysInSprint)
                return true;
            return false;
        }
    }
}
