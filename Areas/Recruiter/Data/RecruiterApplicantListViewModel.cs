using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobTracks.Areas.Recruiter.Data
{
    public class RecruiterApplicantListViewModel
    {
        public int JobApplicantId { get; set; }
        public string ApplicantFullName { get; set; }
        public string LastQualification { get; set; }
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public DateTime? TentativeDate { get; set; }
        public string ApplicantStatus { get; set; }
        public string MappedJobStatus { get; set; }
    }

}