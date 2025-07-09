using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobTracks.Areas.Admin.Data
{
    [MetadataType(typeof(CompanyMasterMetaData))]
    public partial class Company_Master
    {
    }
    public class CompanyMasterMetaData
    {
        [Key]
        public int Company_id { get; set; }

        [Required(ErrorMessage = "Company name is required.")]
        [Remote("IsCompanynameAvailable", "Admin", ErrorMessage = "Company Name already Exist")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Company name must be between 2 and 100 characters.")]
        [Display(Name = "Company Name")]
        public string Company_Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        [Display(Name = "Company Description")]
        public string Description { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Job_Master> Job_Master { get; set; }

    }
}