namespace UPHC.SurveillanceDashboard.Models
{
    public class UPHCStat
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";
        public string Location { get; set; } = "";

       
        public int TotalCases { get; set; } 
    }
}
