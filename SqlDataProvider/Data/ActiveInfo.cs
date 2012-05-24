using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class ActiveInfo
    {
        public int ActiveID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public string AwardContent { get; set; }

        public int HasKey { get; set; }

        public DateTime StartDate { get; set; }

        public System.Nullable<DateTime> EndDate { get; set; }

        public bool IsOnly { get; set; }

        public int Type { get; set; }

        public string ActionTimeContent { get; set; }
    }
}
