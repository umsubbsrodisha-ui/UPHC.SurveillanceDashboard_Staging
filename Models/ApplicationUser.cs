
using Microsoft.AspNetCore.Identity;

namespace UPHC.SurveillanceDashboard.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? FacilityId { get; set; }  //many to one with facility table 
        public Facility? Facility { get; set; }
    }
}