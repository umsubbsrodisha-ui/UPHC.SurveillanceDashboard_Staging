namespace UPHC.SurveillanceDashboard.Models
{
    //public class UPHC { 
    //    public int Id { get; set; } 
    //    public string Name { get; set; } = ""; 
    //    public string Location { get; set; } = ""; 




    //}


    public class UPHC
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";
        public string Location { get; set; } = "";

        // 🔗 One UPHC → Many Cases
        public ICollection<CaseRecord> CaseRecords { get; set; } = new List<CaseRecord>();

       public ICollection<Notification> notifications { get; set; }

    }
}
