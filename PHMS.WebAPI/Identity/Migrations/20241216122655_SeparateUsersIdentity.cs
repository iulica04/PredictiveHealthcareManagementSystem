using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Identity.Migrations
{
    /// <inheritdoc />
    public partial class SeparateUsersIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MedicalCondition",
                columns: table => new
                {
                    MedicalConditionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PatientId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CurrentStatus = table.Column<string>(type: "TEXT", nullable: false),
                    IsGenetic = table.Column<bool>(type: "INTEGER", nullable: true),
                    Recommendation = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalCondition", x => x.MedicalConditionId);
                });

            migrationBuilder.CreateTable(
                name: "Prescription",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DateIssued = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescription", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Gender = table.Column<string>(type: "TEXT", maxLength: 6, nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false),
                    Rank = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    Specialization = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    Hospital = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Medication",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Ingredients = table.Column<string>(type: "TEXT", nullable: false),
                    AdverseEffects = table.Column<string>(type: "TEXT", nullable: false),
                    PrescriptionId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Medication_Prescription_PrescriptionId",
                        column: x => x.PrescriptionId,
                        principalTable: "Prescription",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Treatment",
                columns: table => new
                {
                    TreatmentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    PrescriptionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Duration = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Frequency = table.Column<string>(type: "TEXT", nullable: false),
                    MedicalConditionId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Treatment", x => x.TreatmentId);
                    table.ForeignKey(
                        name: "FK_Treatment_MedicalCondition_MedicalConditionId",
                        column: x => x.MedicalConditionId,
                        principalTable: "MedicalCondition",
                        principalColumn: "MedicalConditionId");
                    table.ForeignKey(
                        name: "FK_Treatment_Prescription_PrescriptionId",
                        column: x => x.PrescriptionId,
                        principalTable: "Prescription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientRecord",
                columns: table => new
                {
                    PatientRecordId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PatientId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MedicalConditionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TreatmentId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientRecord", x => x.PatientRecordId);
                    table.ForeignKey(
                        name: "FK_PatientRecord_MedicalCondition_MedicalConditionId",
                        column: x => x.MedicalConditionId,
                        principalTable: "MedicalCondition",
                        principalColumn: "MedicalConditionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientRecord_Treatment_TreatmentId",
                        column: x => x.TreatmentId,
                        principalTable: "Treatment",
                        principalColumn: "TreatmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientRecord_Users_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "BirthDate", "Email", "FirstName", "Gender", "LastName", "PasswordHash", "PhoneNumber", "Type" },
                values: new object[,]
                {
                    { new Guid("3839f91c-c2d7-4fca-987d-9ba155aafb28"), "Piata Unirii nr.3, Iasi", new DateTime(2004, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), "admin1@gmail.com", "Admin1", "Female", "User", "$2a$11$0HgjUGGoGnvGt0pwyhB8BOKLD83PuB27yU35NUPCz34gEM1A8x/jy", "0757732675", 0 },
                    { new Guid("9e470494-50fc-4361-9b8b-7c8a172fa474"), "Strada Libertatii nr.10, Iasi", new DateTime(2003, 7, 20, 0, 0, 0, 0, DateTimeKind.Utc), "admin2@gmail.com", "Admin2", "Male", "User", "$2a$11$0VJj.0p.cyW3KERp1WjWF.Jk5BGcF77z.CNpcavQIH6KNxbp0/LU.", "0751234567", 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medication_PrescriptionId",
                table: "Medication",
                column: "PrescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientRecord_MedicalConditionId",
                table: "PatientRecord",
                column: "MedicalConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientRecord_PatientId",
                table: "PatientRecord",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientRecord_TreatmentId",
                table: "PatientRecord",
                column: "TreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Treatment_MedicalConditionId",
                table: "Treatment",
                column: "MedicalConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_Treatment_PrescriptionId",
                table: "Treatment",
                column: "PrescriptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Medication");

            migrationBuilder.DropTable(
                name: "PatientRecord");

            migrationBuilder.DropTable(
                name: "Treatment");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "MedicalCondition");

            migrationBuilder.DropTable(
                name: "Prescription");
        }
    }
}
