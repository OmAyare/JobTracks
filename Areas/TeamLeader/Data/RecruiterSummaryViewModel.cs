using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobTracks.Areas.TeamLeader.Data
{
    public class RecruiterSummaryViewModel
    {
        public string RecruiterName { get; set; }
        public string ApplicantName { get; set; }
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public string Status { get; set; }
        public DateTime? AssignedDate { get; set; }
    }
}

