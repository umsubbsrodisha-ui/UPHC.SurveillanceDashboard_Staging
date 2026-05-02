
using System.ComponentModel.DataAnnotations;

namespace UPHC.SurveillanceDashboard.Models
{
    public enum FacilityType
    {
        UPHC = 1,
        CHC = 2,
        PrivateEntity = 3,
        UHWC = 4
    }

    public class Facility
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(150)]
        public string FacilityName { get; set; } = "";

        [Required(ErrorMessage = "Address is required")]
        [StringLength(300)]
        public string FacilityAddress { get; set; } = "";

        [Required(ErrorMessage = "Select facility type")]
        public FacilityType Type { get; set; }

        // 🔗 Relationships

        public ICollection<CaseRecord> CaseRecords { get; set; } = new List<CaseRecord>();

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

        // Referrals
        public ICollection<FacilityReferral> UPHCReferrals { get; set; } = new List<FacilityReferral>();

        public ICollection<FacilityReferral> CHCReferrals { get; set; } = new List<FacilityReferral>();
    }
}






























//using System.ComponentModel.DataAnnotations;

//namespace UPHC.SurveillanceDashboard.Models
//{

//    public enum facilityType
//    {
//        UPHC = 1, CHC = 2, privateEntity = 3, UHWC=4


//    }

//    public class Facility
//    {
//        public int Id { get; set; }


//        [Required(ErrorMessage ="Name is required")]
//        public string facilityName { get; set; } = "";

//        [Required(ErrorMessage ="Address is required")]
//        public string facilityAddress { get; set; } = "";

//        [Required(ErrorMessage ="Select facility (UPHC , UCHC or PrivateEntity")]
//        public facilityType Type { get; set; }

//        // 🔗 One UPHC → Many Cases
//        public ICollection<CaseRecord> CaseRecords { get; set; } = new List<CaseRecord>();
//        public ICollection<Notification> Notifications { get; set; }= new List<Notification>();
//        public ICollection<ApplicationUser> Users { get; set; }=new List<ApplicationUser>();


//        // one facility with many referrals
//        public ICollection<FacilityReferral> UPHCReferrals { get; set; } = new List<FacilityReferral>();
//        public ICollection<FacilityReferral> CHCReferrals { get; set; } = new List<FacilityReferral>();

//    }
//}
