namespace UPHC.SurveillanceDashboard.Models
{
    public class OutbreakDto
    {
        public string DiseaseName { get; set; } = "";
        public string FacilityName { get; set; } = "";
        public int Count { get; set; }
        public DateTime FirstCaseDate { get; set; }
    }
    //public class OutbreakDto
    //{
    //    public string DiseaseName { get; set; }
    //    public string FacilityName { get; set; }
    //    public int Count { get; set; }
    //}
}
