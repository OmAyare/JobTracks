using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobTracks.Areas.Recruiter.Data
{
    public class EditApplicantStatusViewModel
    {
        public int JobApplicantId { get; set; }
        public string ApplicantName { get; set; }
        public string CurrentJobTitle { get; set; }
        public int SelectedJobId { get; set; }
        public string SelectedStatus { get; set; }

        public IEnumerable<SelectListItem> JobOptions { get; set; }
        public IEnumerable<SelectListItem> StatusOptions { get; set; }

        public int? PlacedCount { get; set; }
        public int? RequiredCount { get; set; }
    }
}