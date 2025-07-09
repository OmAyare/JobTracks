using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JobTracks.Areas.Admin.Data
{
    [MetadataType(typeof(JobApplicantMasterMetaData))]
    public partial class Job_Applicant_Master
    {
    }

    public class JobApplicantMasterMetaData
    {
        [Key]
        public int Job_Id { get; set; }

        [Required(ErrorMessage = "Recruiter is required.")]
        [ForeignKey("User")]
        [Display(Name = "Recruiter")]
        public int Recuriter_ID { get; set; }

        [Required(ErrorMessage = "Applicant is required.")]
        [Display(Name = "Applicant")]
        public int Applicant_ID { get; set; }

        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        public string Status { get; set; }

        [ForeignKey("Job_Master")]
        [Display(Name = "Job Reference")]
        public Nullable<int> JobRef_Id { get; set; }

        [ForeignKey("Applicant_ID")]
        public virtual Applicant_Master Applicant_Master { get; set; }
        [ForeignKey("Recuriter_ID")]
        public virtual User User { get; set; }
        [ForeignKey("JobRef_Id")]
        public virtual Job_Master Job_Master { get; set; }
    }
}