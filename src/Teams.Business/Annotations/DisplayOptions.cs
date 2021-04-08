using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Teams.Business.Annotations
{
    public class DisplayOptions
    {
        [DefaultValue(0), Range(0, int.MaxValue)]
        public int Page { get; set; }

        [DefaultValue(25), Range(0, int.MaxValue)]
        public int PageSize { get; set; }

        [DefaultValue(SortDirection.Ascending)]
        public SortDirection SortDirection { get; set; }
    }
}
