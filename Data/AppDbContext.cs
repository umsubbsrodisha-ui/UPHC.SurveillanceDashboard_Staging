using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UPHC.SurveillanceDashboard.Models;

namespace UPHC.SurveillanceDashboard.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<CaseRecord> CaseRecords { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<FacilityReferral> FacilityReferrals { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 🔗 CaseRecord → Facility
            builder.Entity<CaseRecord>()
                .HasOne(c => c.Facility)
                .WithMany(f => f.CaseRecords)
                .HasForeignKey(c => c.FacilityId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔗 Notification → CaseRecord
            builder.Entity<Notification>()
                .HasOne(n => n.CaseRecord)
                .WithMany(c => c.Notifications)
                .HasForeignKey(n => n.CaseRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔗 ApplicationUser → Facility
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Facility)
                .WithMany(f => f.Users)
                .HasForeignKey(u => u.FacilityId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔗 Notification → Facility
            builder.Entity<Notification>()
                .HasOne(n => n.Facility)
                .WithMany(f => f.Notifications)
                .HasForeignKey(n => n.FacilityId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔗 FacilityReferral (UPHC)
            builder.Entity<FacilityReferral>()
                .HasOne(fr => fr.UPHC)
                .WithMany(f => f.UPHCReferrals)
                .HasForeignKey(fr => fr.UPHCId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔗 FacilityReferral (CHC)
            builder.Entity<FacilityReferral>()
                .HasOne(fr => fr.CHC)
                .WithMany(f => f.CHCReferrals)
                .HasForeignKey(fr => fr.CHCId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔒 Prevent duplicate referrals
            builder.Entity<FacilityReferral>()
                .HasIndex(fr => new { fr.UPHCId, fr.CHCId })
                .IsUnique();

            //// 🔒 Prevent self-referral
            //builder.Entity<FacilityReferral>()
            //    .HasCheckConstraint("CK_NoSelfReferral", "\"UPHCId\" <> \"CHCId\"");



            // 🔒 Prevent self-referral
            builder.Entity<FacilityReferral>().ToTable(t => t.HasCheckConstraint("CK_NoSelfReferral","\"UPHCId\" <> \"CHCId\""));

            // 🔥 CRITICAL: Prevent duplicate notifications
            builder.Entity<Notification>()
                .HasIndex(n => new { n.CaseRecordId, n.Type })
                .IsUnique();

            // 🌱 Seed Facilities
            builder.Entity<Facility>().HasData(
                new Facility { Id = 1, FacilityName = "UCHC Unit-4", Type = FacilityType.CHC, FacilityAddress = "Unit-4" },
                new Facility { Id = 2, FacilityName = "UCHC Dumduma", Type = FacilityType.CHC, FacilityAddress = "Dumduma" },
                new Facility { Id = 3, FacilityName = "UCHC Unit-8", Type = FacilityType.CHC, FacilityAddress = "Unit-8" },
                new Facility { Id = 4, FacilityName = "UCHC BMC Hospital", Type = FacilityType.CHC, FacilityAddress = "Old Town,Lingaraj nagar" },
                new Facility { Id = 5, FacilityName = "Unit-3 UPHC", Type = FacilityType.UPHC, FacilityAddress = "Kharvela Nagar" },
                new Facility { Id = 6, FacilityName = "Unit-4 UPHC", Type = FacilityType.UPHC, FacilityAddress = "Unit-4,Near AG Colony" },
                new Facility { Id = 7, FacilityName = "Unit-8 UPHC", Type = FacilityType.UPHC, FacilityAddress = "Unit-8" },
                new Facility { Id = 8, FacilityName = "Unit-9 UPHC", Type = FacilityType.UPHC, FacilityAddress = "Unit-9 Industrial Colony" },
                new Facility { Id = 9, FacilityName = "Saheed Nagar UPHC", Type = FacilityType.UPHC, FacilityAddress = "Saheed Nagar" },
                new Facility { Id = 10, FacilityName = "Satya Nagar UPHC", Type = FacilityType.UPHC, FacilityAddress = "Satya Nagar,Near Kali Mandir" },
                new Facility { Id = 11, FacilityName = "Baramunda UPHC", Type = FacilityType.UPHC, FacilityAddress = "Rental Colony, IRC Village" },
                new Facility { Id = 12, FacilityName = "IRC Village UPHC", Type = FacilityType.UPHC, FacilityAddress = "IRC Village, Nayapalli" },
                new Facility { Id = 13, FacilityName = "Rasulgarh UPHC", Type = FacilityType.UPHC, FacilityAddress = "GGP Colony, Rasulgarh" },
                new Facility { Id = 14, FacilityName = "Gadakan UPHC", Type = FacilityType.UPHC, FacilityAddress = "Gadeswar,Near RI Office Kalarahanga" },
                new Facility { Id = 15, FacilityName = "Pokhariput UPHC", Type = FacilityType.UPHC, FacilityAddress = "Pokhariput,Ward No-62" },
                new Facility { Id = 16, FacilityName = "Chandrasekharpur UPHC", Type = FacilityType.UPHC, FacilityAddress = "CS Pur HB Colony,Ward No-8" },
                new Facility { Id = 17, FacilityName = "Niladri Vihar UPHC", Type = FacilityType.UPHC, FacilityAddress = "Niladri Vihar, Sector-I" },
                new Facility { Id = 18, FacilityName = "Bjb Nagar UPHC", Type = FacilityType.UPHC, FacilityAddress = "BJB Nagar,Near BJB Nagar Hata" },
                new Facility { Id = 19, FacilityName = "Bhimatangi UPHC", Type = FacilityType.UPHC, FacilityAddress = "Bhimatangi Housing Board area" },
                new Facility { Id = 20, FacilityName = "Kapilaprasad UPHC", Type = FacilityType.UPHC, FacilityAddress = "Kapilaprasad Village area" },
                new Facility { Id = 21, FacilityName = "Badagada UPHC", Type = FacilityType.UPHC, FacilityAddress = "Badagada Village,Brit area" },
                new Facility { Id = 22, FacilityName = "Jharapada UPHC", Type = FacilityType.UPHC, FacilityAddress = "Jharapada Village area" },
                new Facility { Id = 23, FacilityName = "Bharatpur UPHC", Type = FacilityType.UPHC, FacilityAddress = "Bharatpur Slum,Mahalaxmi Vihar" },
                new Facility { Id = 24, FacilityName = "Patia UPHC", Type = FacilityType.UPHC, FacilityAddress = "Patia Village,Damana area" },
                new Facility { Id = 25, FacilityName = "VSS Nagar UPHC", Type = FacilityType.UPHC, FacilityAddress = "VSS Nagar Housing Board area" },
                new Facility { Id = 26, FacilityName = "Laxmisagar UPHC", Type = FacilityType.UPHC, FacilityAddress = "Laxmisagar Village area" },
                 new Facility { Id = 27, FacilityName = "VSS Nagar UHWC", Type = FacilityType.UHWC, FacilityAddress = "Ward 12, near Kalyan Mandap" },
    new Facility { Id = 28, FacilityName = "Niladri Vihar UHWC", Type = FacilityType.UHWC, FacilityAddress = "Ward 14, Sector 1" },
    new Facility { Id = 29, FacilityName = "Samantarapur UHWC", Type = FacilityType.UHWC, FacilityAddress = "Ward 59, near Samantarapur Square" },
    new Facility { Id = 30, FacilityName = "Aiginia UHWC", Type = FacilityType.UHWC, FacilityAddress = "Ward 49, Shreekhetra Vihar" },
    new Facility { Id = 31, FacilityName = "Patrapada UHWC", Type = FacilityType.UHWC, FacilityAddress = "Ward 65, Kalinga Vihar K-9" },
    new Facility { Id = 32, FacilityName = "Baramunda HB UHWC", Type = FacilityType.UHWC, FacilityAddress = "L170, Baramunda Housing Board Colony" },
    new Facility { Id = 33, FacilityName = "Chakaisiani UHWC", Type = FacilityType.UHWC, FacilityAddress = "Satya Vihar, Ward 5" },
    new Facility { Id = 34, FacilityName = "Acharya Vihar UHWC", Type = FacilityType.UHWC, FacilityAddress = "Acharya Vihar, Ward 27" },
    new Facility { Id = 35, FacilityName = "Kalarahanga UHWC", Type = FacilityType.UHWC, FacilityAddress = "Kalarahanga, near KIIT area" },
    new Facility { Id = 36, FacilityName = "Palasuni UHWC", Type = FacilityType.UHWC, FacilityAddress = "Sainik School Road, Ward 5" },
    new Facility { Id = 37, FacilityName = "Subudhipur UHWC", Type = FacilityType.UHWC, FacilityAddress = "Subudhipur, Aiginia area" },
    new Facility { Id = 38, FacilityName = "Badagada Sabarasahi UHWC", Type = FacilityType.UHWC, FacilityAddress = "Badagada Brit Colony" },
    new Facility { Id = 39, FacilityName = "Hanspal UHWC", Type = FacilityType.UHWC, FacilityAddress = "Hanspal, Balianta area" },
    new Facility { Id = 40, FacilityName = "Gadakana UHWC", Type = FacilityType.UHWC, FacilityAddress = "Gadakana area, Ward 9" },
    new Facility { Id = 41, FacilityName = "Jharapada UHWC", Type = FacilityType.UHWC, FacilityAddress = "Jharapada, near Jail area" },
    new Facility { Id = 42, FacilityName = "Sailashree Vihar UHWC", Type = FacilityType.UHWC, FacilityAddress = "Sailashree Vihar, Ward 7" },
    new Facility { Id = 43, FacilityName = "Unit-6 UHWC", Type = FacilityType.UHWC, FacilityAddress = "Unit-6 area, Ward 46" },
    new Facility { Id = 44, FacilityName = "Dumuduma UHWC", Type = FacilityType.UHWC, FacilityAddress = "Dumuduma Housing Board Phase-III" }






                );
        }
    }
}


































//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;
//using UPHC.SurveillanceDashboard.Models;

//namespace UPHC.SurveillanceDashboard.Data
//{



//    public class AppDbContext : IdentityDbContext<ApplicationUser>
//    {
//        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

//        public DbSet<CaseRecord> CaseRecords { get; set; }
//        public DbSet<Notification> Notifications { get; set; }
//        public DbSet<UPHC.SurveillanceDashboard.Models. Facility> Facilities { get; set; }

//        public DbSet<FacilityReferral> FacilityReferrals { get; set; }

//        protected override void OnModelCreating(ModelBuilder builder)
//        {
//            base.OnModelCreating(builder);

//            // 🔗 CaseRecord → facility (Many-to-One)
//            builder.Entity<CaseRecord>()
//                .HasOne(c => c.Facility)
//                .WithMany(u => u.CaseRecords)
//                .HasForeignKey(c => c.FacilityId)
//                .OnDelete(DeleteBehavior.Restrict);

//            // 🔗 Notification → CaseRecord (Many-to-One)
//            builder.Entity<Notification>()
//                .HasOne(n => n.CaseRecord)
//                .WithMany(c => c.Notifications)
//                .HasForeignKey(n => n.CaseRecordId)
//                .OnDelete(DeleteBehavior.Cascade);

//            //ApplicationUser -> Facility (Many-to-one)

//            builder.Entity<ApplicationUser>()
//    .HasOne(u => u.Facility)
//    .WithMany(f=>f.Users)
//    .HasForeignKey(u => u.FacilityId)
//    .OnDelete(DeleteBehavior.Restrict); 


//            //Notification -> Facility (Many-to-one)
//            builder.Entity<Notification>()
//    .HasOne(n => n.Facility)
//    .WithMany(n=>n.Notifications)
//    .HasForeignKey(n => n.FacilityId)
//    .OnDelete(DeleteBehavior.Restrict);


//            // Many referrals to one Facility as far as UPHC is concerned

//            builder.Entity<FacilityReferral>()
//    .HasOne(fr => fr.UPHC)
//    .WithMany(f => f.UPHCReferrals)
//    .HasForeignKey(fr => fr.UPHCId)
//    .OnDelete(DeleteBehavior.Restrict);

//            //Many referrals to one Facility as far as UCHC is concerned
//            builder.Entity<FacilityReferral>()
//    .HasOne(fr => fr.CHC)
//    .WithMany(f => f.CHCReferrals)
//    .HasForeignKey(fr => fr.CHCId)
//    .OnDelete(DeleteBehavior.Restrict);


//            // A composite unique key is created in Facility Referral table
//            builder.Entity<FacilityReferral>()
//    .HasIndex(fr => new { fr.UPHCId, fr.CHCId })
//    .IsUnique();
//            // Seed Facility
//            builder.Entity<UPHC.SurveillanceDashboard.Models.Facility>().HasData(



//                new UPHC.SurveillanceDashboard.Models.Facility { Id = 1, facilityName = "UCHC Unit-4", Type = facilityType.CHC, facilityAddress = "Unit-4" },
//                new UPHC.SurveillanceDashboard.Models.Facility { Id = 2, facilityName = "UCHC Dumduma", Type = facilityType.CHC, facilityAddress = "Dumduma" },
//                new UPHC.SurveillanceDashboard.Models.Facility { Id = 3, facilityName = "UCHC Unit-8", Type = facilityType.CHC, facilityAddress = "Unit-8" },
//                new UPHC.SurveillanceDashboard.Models.Facility { Id = 4, facilityName = "UCHC BMC Hospital", Type = facilityType.CHC, facilityAddress = "Old Town,Lingaraj nagar" },
//                new UPHC.SurveillanceDashboard.Models.Facility { Id =5 , facilityName = "Unit-3 UPHC",Type=facilityType.UPHC ,facilityAddress = "Kharvela Nagar" },
//                new UPHC.SurveillanceDashboard.Models.Facility { Id =6, facilityName = "Unit-4 UPHC", Type = facilityType.UPHC, facilityAddress = "Unit-4,Near AG Colony" },
//                new UPHC.SurveillanceDashboard.Models.Facility { Id = 7, facilityName = "Unit-8 UPHC", Type = facilityType.UPHC, facilityAddress = "Unit-8" },
//                new UPHC.SurveillanceDashboard.Models.Facility { Id = 8, facilityName = "Unit-9 UPHC",Type=facilityType.UPHC, facilityAddress = "Unit-9 Industrial Colony" },
//                new UPHC.SurveillanceDashboard.Models.Facility { Id = 9, facilityName = "Saheed Nagar UPHC", Type = facilityType.UPHC, facilityAddress = "Saheed Nagar" },
//                new UPHC.SurveillanceDashboard.Models.Facility { Id = 10, facilityName = "Satya Nagar UPHC",Type = facilityType.UPHC, facilityAddress = "Satya Nagar,Near Kali Mandir" },
//                new UPHC.SurveillanceDashboard.Models.Facility { Id = 11, facilityName = "Baramunda UPHC", Type = facilityType.UPHC, facilityAddress = "Rental Colony, IRC Village" },
//                new UPHC.SurveillanceDashboard.Models.Facility { Id = 12, facilityName = "IRC Village UPHC", Type = facilityType.UPHC, facilityAddress = "IRC Village, Nayapalli" },
//                new UPHC.SurveillanceDashboard.Models.Facility { Id = 13, facilityName = "Rasulgarh UPHC", Type = facilityType.UPHC, facilityAddress = "GGP Colony, Rasulgarh" },
//                new UPHC.SurveillanceDashboard.Models.Facility { Id = 14, facilityName = "Gadakan UPHC", Type = facilityType.UPHC, facilityAddress = "Gadeswar,Near RI Office Kalarahanga" },
//                new UPHC.SurveillanceDashboard.Models.Facility { Id = 15, facilityName = "Pokhariput UPHC", Type = facilityType.UPHC, facilityAddress = "Pokhariput,Ward No-62" },
//                new UPHC.SurveillanceDashboard.Models.Facility { Id = 16, facilityName = "Chandrasekharpur UPHC", Type = facilityType.UPHC, facilityAddress = "CS Pur HB Colony,Ward No-8" },
//                new UPHC.SurveillanceDashboard.Models.Facility { Id = 17, facilityName = "Niladri Vihar UPHC", Type = facilityType.UPHC, facilityAddress = "Niladri Vihar, Sector-I" },
//                new UPHC.SurveillanceDashboard.Models.Facility { Id = 18, facilityName = "Bjb Nagar UPHC", Type = facilityType.UPHC, facilityAddress = "BJB Nagar,Near BJB Nagar Hata" },
//                new UPHC.SurveillanceDashboard.Models.Facility { Id = 19, facilityName = "Bhimatangi UPHC", Type = facilityType.UPHC, facilityAddress = "Bhimatangi Housing Board area" },
//                new UPHC.SurveillanceDashboard.Models.Facility { Id = 20, facilityName = "Kapilaprasad UPHC", Type = facilityType.UPHC, facilityAddress = "Kapilaprasad Village area" },
//                new UPHC.SurveillanceDashboard.Models.Facility { Id = 21, facilityName = "Badagada UPHC", Type = facilityType.UPHC, facilityAddress = "Badagada Village,Brit area" },
//                new UPHC.SurveillanceDashboard.Models.Facility { Id = 22, facilityName = "Jharapada UPHC", Type = facilityType.UPHC, facilityAddress = "Jharapada Village area" },
//                new UPHC.SurveillanceDashboard.Models.Facility { Id = 23, facilityName = "Bharatpur UPHC", Type = facilityType.UPHC, facilityAddress = "Bharatpur Slum,Mahalaxmi Vihar" },
//                new UPHC.SurveillanceDashboard.Models.Facility { Id = 24, facilityName = "Patia UPHC", Type = facilityType.UPHC, facilityAddress = "Patia Village,Damana area" },
//                new UPHC.SurveillanceDashboard.Models.Facility { Id = 25, facilityName = "VSS Nagar UPHC", Type = facilityType.UPHC, facilityAddress = "VSS Nagar Housing Board area" },
//                new UPHC.SurveillanceDashboard.Models.Facility { Id = 26, facilityName = "Laxmisagar UPHC", Type = facilityType.UPHC, facilityAddress = "Laxmisagar Village area" }
//            );
//        }
//    }
//}
