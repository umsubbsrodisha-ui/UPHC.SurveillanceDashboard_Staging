

using System.ComponentModel.DataAnnotations;

namespace UPHC.SurveillanceDashboard.Models
{
    public enum NotificationType
    {
        NewCase = 1,
        ConfirmedPositive = 2,
        ConfirmedNegative = 3
    }

    public class Notification
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string DiseaseName { get; set; } = "";

        public bool IsChecked { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public NotificationType Type { get; set; } = NotificationType.NewCase;

        // 🔗 FK → CaseRecord
        public int CaseRecordId { get; set; }
        public CaseRecord CaseRecord { get; set; } = default!;

        // 🔗 FK → Facility
        public int FacilityId { get; set; }
        public Facility Facility { get; set; } = default!;
    }
}
























//namespace UPHC.SurveillanceDashboard.Models
//{

//    public enum NotificationType
//    {
//        NewCase=1,
//        ConfirmedPositive=2,
//        ConfirmedNegetive=3
//    }

//    public class Notification
//    {
//        public int Id { get; set; }

//        public string DiseaseName { get; set; } = "";

//        public bool IsChecked { get; set; }

//        public DateTime Timestamp { get; set; } = DateTime.Now;

//        public NotificationType Type { get; set; } = NotificationType.NewCase;


//        // 🔗 FK → CaseRecord
//        public int CaseRecordId { get; set; }
//        public CaseRecord? CaseRecord { get; set; }

//        public int FacilityId { get; set; }
//        public Facility? Facility { get; set; }
//    }

//}
