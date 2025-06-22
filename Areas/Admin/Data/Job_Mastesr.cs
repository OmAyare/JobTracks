using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobTracks.Areas.Admin.Data
{
    [MetadataType(typeof(JobMasterMetaData))]
    public partial class Job_Master
    {
      
    }

    public class JobMasterMetaData
    {
        public int Job_id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tech_Stack { get; set; }
        public string status { get; set; }

        [DisplayName("Company Name")]
        public Nullable<int> Company_Id { get; set; }
        [DisplayName("Recuriter")]
        public Nullable<int> TeamLeader_Id { get; set; }

        [DisplayName("Team Leader")]
        public Nullable<int> Recruiter_Id { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> TentativeDate { get; set; }

        [DisplayName("Company Name")]
        public virtual Company_Master Company_Master { get; set; }

        [DisplayName("Recuriter")]
        public virtual User User { get; set; }
  
        [DisplayName("Team Leader")]
        public virtual User User1 { get; set; }
    }
}