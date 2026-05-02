using System.ComponentModel.DataAnnotations;

namespace UPHC.SurveillanceDashboard.Models
{
    public class FacilityReferral
    {
        public int Id { get; set; }

        //  UPHC (child facility)
        [Required]
        public int UPHCId { get; set; }
        public Facility UPHC { get; set; } = default!;

        //  CHC (parent / referral facility)
        [Required]
        public int CHCId { get; set; }
        public Facility CHC { get; set; } = default!;
    }
}




















//namespace UPHC.SurveillanceDashboard.Models
//{
//    public class FacilityReferral
//    {
//        public int Id { get; set; }

//        //  UPHC (child facility)
//        public int UPHCId { get; set; }
//        public Facility? UPHC { get; set; }

//        //  CHC (parent / referral facility)
//        public int CHCId { get; set; }
//        public Facility? CHC { get; set; }


//    }
//}
