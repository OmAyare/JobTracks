using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JobTracks.Areas.Admin.Data
{
    [MetadataType(typeof(ApplicantMasterMetaData))]
    public partial class Applicant_Master
    {
    }

    public class ApplicantMasterMetaData
    {
        [Key]
        [Display(Name = "Applicant ID")]
        public int AppLicant_id { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Last qualification is required.")]
        [StringLength(100, ErrorMessage = "Qualification cannot exceed 100 characters.")]
        [Display(Name = "Last Qualification")]
        public string Last_Qualification { get; set; }

        [Range(1900, 2100, ErrorMessage = "Invalid Pass Out Year")]
        [Display(Name = "Pass Out Year")]
        public Nullable<int> PassOutYear { get; set; }

        [Range(0, 40, ErrorMessage = "Experience must be between 0 and 40 years.")]
        [Display(Name = "Years of Experience")]
        public Nullable<int> YearOfExperience { get; set; }

        [StringLength(200)]
        public string Resume { get; set; }

        [StringLength(50)]
        [Display(Name = "Current Status")]
        public string Status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Job_Applicant_Master> Job_Applicant_Master { get; set; }
    }
}