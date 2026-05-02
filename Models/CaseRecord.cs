

//|      Field |                      Purpose |
//| ------------------ | ----------------------------   |
//| `OnsetDate` ✅      | Symptom start (epidemiology) |
//| `DateReported`     | Facility visit               |
//| `CreatedDate`      | System entry                 |
//| `LabConfirmedDate` | Diagnosis  result out        |

//OnsetDate        → when illness started ✅
//DateReported     → when patient reached facility
//CreatedDate      → when system recorded
//LabConfirmedDate → when diagnosis confirmed


using System.ComponentModel.DataAnnotations;

namespace UPHC.SurveillanceDashboard.Models
{
    public enum CaseStatus
    {
        Suspected,
        Confirmed,
        Negative
    }

    public class CaseRecord
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Patient name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string PatientName { get; set; } = "";

        [Required(ErrorMessage = "Phone number is mandatory")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter valid 10-digit mobile number")]
        public string Phone { get; set; } = "";

        [Required(ErrorMessage = "Disease name is required")]
        [StringLength(100, ErrorMessage = "Disease name too long")]
        public string DiseaseName { get; set; } = "";

        [Required(ErrorMessage = "Symptoms are required")]
        [StringLength(500, ErrorMessage = "Symptoms too long (max 500 characters)")]
        public string Symptoms { get; set; } = "";

        [Required(ErrorMessage = "When exactly the symptoms started must be recorded")]
        public DateTime OnsetDate { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(300, ErrorMessage = "Address too long (max 300 characters)")]
        public string AddressOfPatient { get; set; } = "";

        public bool IsCommunicable { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Patient visit date is required")]
        public DateTime DateReported { get; set; }

        public DateTime? LabConfirmedDate { get; set; }

        public CaseStatus Status { get; set; } = CaseStatus.Suspected;

        [Required]
        public string UserId { get; set; } = "";

        public ApplicationUser User { get; set; } = default!;

        [Required(ErrorMessage = "Medical Facility is required")]
        public int FacilityId { get; set; }

        public Facility Facility { get; set; } = default!;

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
















//using System.ComponentModel.DataAnnotations;

//namespace UPHC.SurveillanceDashboard.Models
//{


//    public enum CaseStatus
//    {
//        Suspected,
//        Confirmed,
//        Negative
//    }

//    public class CaseRecord
//    {
//        public int Id { get; set; }

//        [Required(ErrorMessage = "Patient name is required")]
//        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
//        public string PatientName { get; set; } = "";

//        [Required(ErrorMessage = "Phone number is mandatory")]
//        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter valid 10-digit mobile number")]
//        public string Phone { get; set; } = "";



//        [Required(ErrorMessage = "Disease name is required")]
//        [StringLength(100, ErrorMessage = "Disease name too long")]
//        public string DiseaseName { get; set; } = "";


//        [StringLength(500, ErrorMessage = "Symptoms too long (max 500 characters)")]
//        [Required(ErrorMessage = "Symptom is  required")]
//        public string Symptoms { get; set; } = "";

//        [Required(ErrorMessage ="When exactly the symptoms started must be recorded")]
//        public DateTime OnsetDate { get; set; } // Docs always ask when your symptoms started

//        [Required(ErrorMessage = "Address is required")]
//        [StringLength(300, ErrorMessage = "Address too long (max 300 characters)")]
//        public string AddressOfPatient { get; set; } = "";
//        public bool IsCommunicable { get; set; }

//        public DateTime CreatedDate { get; set; } = DateTime.Now; //system entry date or when record is saved to database and can be equal to DateReported

//        [Required(ErrorMessage = "Patient visit date is required")]
//        public DateTime? DateReported { get; set; }  // Date when patient visited UPHC / was seen by doctor Or when the patient arrives at UPHC and  can be same as Created Date OR Consultation Date

//        public DateTime? LabConfirmedDate { get; set; } //Lab confirmation date when "Lab result is out"
//        public CaseStatus Status { get; set; } = CaseStatus.Suspected; // Suspected / Confirmed / Negative


//        [Required]
//        public string UserId { get; set; } = "";

//        public ApplicationUser? User { get; set; }



//        [Required(ErrorMessage = "Medical Facility is required")]

//        public int FacilityId { get; set; }
//        public Facility? Facility { get; set; }

//        // 🔗 One Case → Many Notifications
//        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();


//    }
//}
