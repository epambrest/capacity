using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Teams.Models
{
    public class DisplayOptions
    {
        [DefaultValue(0), Range(0, int.MaxValue)]
        public int Page { get; set; }

        [DefaultValue(25), Range(0, int.MaxValue)]
        public int PageSize { get; set; }

        [DefaultValue(0), Range(0, 1)]
        public int SortDirection { get; set; }
    }
}
