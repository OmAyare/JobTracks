using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobTracks.Areas.Recruiter.Data
{
    public class AssignedJobViewModel
    {
        public int JobId { get; set; }
        public string Title { get; set; }
        public string TechStack { get; set; }
        public string Status { get; set; }
        public string CompanyName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? TentativeDate { get; set; }
    }

}