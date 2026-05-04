using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UPHC.SurveillanceDashboard.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Facilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FacilityName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    FacilityAddress = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FacilityId = table.Column<int>(type: "integer", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FacilityReferrals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UPHCId = table.Column<int>(type: "integer", nullable: false),
                    CHCId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityReferrals", x => x.Id);
                    table.CheckConstraint("CK_NoSelfReferral", "\"UPHCId\" <> \"CHCId\"");
                    table.ForeignKey(
                        name: "FK_FacilityReferrals_Facilities_CHCId",
                        column: x => x.CHCId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacilityReferrals_Facilities_UPHCId",
                        column: x => x.UPHCId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CaseRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PatientName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    DiseaseName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Symptoms = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    OnsetDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AddressOfPatient = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    IsCommunicable = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateReported = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LabConfirmedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    FacilityId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseRecords_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CaseRecords_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DiseaseName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsChecked = table.Column<bool>(type: "boolean", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CaseRecordId = table.Column<int>(type: "integer", nullable: false),
                    FacilityId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_CaseRecords_CaseRecordId",
                        column: x => x.CaseRecordId,
                        principalTable: "CaseRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Facilities",
                columns: new[] { "Id", "FacilityAddress", "FacilityName", "Type" },
                values: new object[,]
                {
                    { 1, "Unit-4", "UCHC Unit-4", 2 },
                    { 2, "Dumduma", "UCHC Dumduma", 2 },
                    { 3, "Unit-8", "UCHC Unit-8", 2 },
                    { 4, "Old Town,Lingaraj nagar", "UCHC BMC Hospital", 2 },
                    { 5, "Kharvela Nagar", "Unit-3 UPHC", 1 },
                    { 6, "Unit-4,Near AG Colony", "Unit-4 UPHC", 1 },
                    { 7, "Unit-8", "Unit-8 UPHC", 1 },
                    { 8, "Unit-9 Industrial Colony", "Unit-9 UPHC", 1 },
                    { 9, "Saheed Nagar", "Saheed Nagar UPHC", 1 },
                    { 10, "Satya Nagar,Near Kali Mandir", "Satya Nagar UPHC", 1 },
                    { 11, "Rental Colony, IRC Village", "Baramunda UPHC", 1 },
                    { 12, "IRC Village, Nayapalli", "IRC Village UPHC", 1 },
                    { 13, "GGP Colony, Rasulgarh", "Rasulgarh UPHC", 1 },
                    { 14, "Gadeswar,Near RI Office Kalarahanga", "Gadakan UPHC", 1 },
                    { 15, "Pokhariput,Ward No-62", "Pokhariput UPHC", 1 },
                    { 16, "CS Pur HB Colony,Ward No-8", "Chandrasekharpur UPHC", 1 },
                    { 17, "Niladri Vihar, Sector-I", "Niladri Vihar UPHC", 1 },
                    { 18, "BJB Nagar,Near BJB Nagar Hata", "Bjb Nagar UPHC", 1 },
                    { 19, "Bhimatangi Housing Board area", "Bhimatangi UPHC", 1 },
                    { 20, "Kapilaprasad Village area", "Kapilaprasad UPHC", 1 },
                    { 21, "Badagada Village,Brit area", "Badagada UPHC", 1 },
                    { 22, "Jharapada Village area", "Jharapada UPHC", 1 },
                    { 23, "Bharatpur Slum,Mahalaxmi Vihar", "Bharatpur UPHC", 1 },
                    { 24, "Patia Village,Damana area", "Patia UPHC", 1 },
                    { 25, "VSS Nagar Housing Board area", "VSS Nagar UPHC", 1 },
                    { 26, "Laxmisagar Village area", "Laxmisagar UPHC", 1 },
                    { 27, "Ward 12, near Kalyan Mandap", "VSS Nagar UHWC", 4 },
                    { 28, "Ward 14, Sector 1", "Niladri Vihar UHWC", 4 },
                    { 29, "Ward 59, near Samantarapur Square", "Samantarapur UHWC", 4 },
                    { 30, "Ward 49, Shreekhetra Vihar", "Aiginia UHWC", 4 },
                    { 31, "Ward 65, Kalinga Vihar K-9", "Patrapada UHWC", 4 },
                    { 32, "L170, Baramunda Housing Board Colony", "Baramunda HB UHWC", 4 },
                    { 33, "Satya Vihar, Ward 5", "Chakaisiani UHWC", 4 },
                    { 34, "Acharya Vihar, Ward 27", "Acharya Vihar UHWC", 4 },
                    { 35, "Kalarahanga, near KIIT area", "Kalarahanga UHWC", 4 },
                    { 36, "Sainik School Road, Ward 5", "Palasuni UHWC", 4 },
                    { 37, "Subudhipur, Aiginia area", "Subudhipur UHWC", 4 },
                    { 38, "Badagada Brit Colony", "Badagada Sabarasahi UHWC", 4 },
                    { 39, "Hanspal, Balianta area", "Hanspal UHWC", 4 },
                    { 40, "Gadakana area, Ward 9", "Gadakana UHWC", 4 },
                    { 41, "Jharapada, near Jail area", "Jharapada UHWC", 4 },
                    { 42, "Sailashree Vihar, Ward 7", "Sailashree Vihar UHWC", 4 },
                    { 43, "Unit-6 area, Ward 46", "Unit-6 UHWC", 4 },
                    { 44, "Dumuduma Housing Board Phase-III", "Dumuduma UHWC", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_FacilityId",
                table: "AspNetUsers",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CaseRecords_FacilityId",
                table: "CaseRecords",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseRecords_UserId",
                table: "CaseRecords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityReferrals_CHCId",
                table: "FacilityReferrals",
                column: "CHCId");

            migrationBuilder.CreateIndex(
                name: "IX_FacilityReferrals_UPHCId_CHCId",
                table: "FacilityReferrals",
                columns: new[] { "UPHCId", "CHCId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CaseRecordId_Type",
                table: "Notifications",
                columns: new[] { "CaseRecordId", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_FacilityId",
                table: "Notifications",
                column: "FacilityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "FacilityReferrals");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "CaseRecords");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Facilities");
        }
    }
}
