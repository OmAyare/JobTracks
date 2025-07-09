using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobTracks.Common;

namespace JobTracks.Areas.Admin.Data
{
    [MetadataType(typeof(JobMasterMetaData))]
    public partial class Job_Master
    {
      
    }

    public class JobMasterMetaData
    {
        [Key]
        public int Job_id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Job Title")]
        public string Title { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Job Description")]
        public string Description { get; set; }

        [Display(Name = "Technology Stack")]
        [StringLength(100)]
        public string Tech_Stack { get; set; }
        public string status { get; set; }

        [Required]
        [DisplayName("Company Name")]
        public Nullable<int> Company_Id { get; set; }

        [DisplayName("Recuriter")]
        public Nullable<int> TeamLeader_Id { get; set; }

        [Required]
        [DisplayName("Team Leader")]
        public Nullable<int> Recruiter_Id { get; set; }
        [Required]
        [CurrentDate(ErrorMessage = "Date cannot be in the future.")]
        [DisplayName("Created Date")]
        public System.DateTime CreatedDate { get; set; }

        [DateRange("01/01/2000", ErrorMessage = "Tentative date must be later than 01-Jan-2000.")]
        [DisplayName("Tentative Date")]
        public Nullable<System.DateTime> TentativeDate { get; set; }

        [ForeignKey("Company_Id")]
        [DisplayName("Company Name")]
        public virtual Company_Master Company_Master { get; set; }

        [ForeignKey("Recruiter_Id")]
        [DisplayName("Recuriter")]
        public virtual User User { get; set; }

        [ForeignKey("TeamLeader_Id")]
        [DisplayName("Team Leader")]
        public virtual User User1 { get; set; }
    }
}