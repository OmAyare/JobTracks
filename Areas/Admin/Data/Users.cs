using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobTracks.Areas.Admin.Data
{
    [MetadataType(typeof(UserMetaData))]
    public partial class User
    {
    }

    public class UserMetaData
    {
        [Key]
        public int User_id { get; set; }


        [Required(ErrorMessage = "The Name is reuired")]
        [Remote("IsUsernameAvailable", "Admin", ErrorMessage = "Username already Exist")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [Remote("IsEmailAvailable", "Admin", ErrorMessage = "Email already Exist")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Need to Assign the Role to the User")]
        public Nullable<int> Role_id { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Job_Applicant_Master> Job_Applicant_Master { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Job_Master> Job_Master { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Job_Master> Job_Master1 { get; set; }
        [ForeignKey("Role_id")]
        public virtual Role Role { get; set; }
    }

}