using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "PacientRecord",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PacientRecord", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    Role = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LastName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Gender = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Address = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "pacients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    PacientRecordId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pacients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pacients_PacientRecord_PacientRecordId",
                        column: x => x.PacientRecordId,
                        principalTable: "PacientRecord",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pacients_User_Id",
                        column: x => x.Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "consultations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    PacientId = table.Column<Guid>(type: "uuid", nullable: false),
                    PacientRecordId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_consultations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_consultations_PacientRecord_PacientRecordId",
                        column: x => x.PacientRecordId,
                        principalTable: "PacientRecord",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_consultations_pacients_PacientId",
                        column: x => x.PacientId,
                        principalTable: "pacients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "medicalConditions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    PacientId = table.Column<Guid>(type: "uuid", nullable: false),
                    PacientRecordId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medicalConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_medicalConditions_PacientRecord_PacientRecordId",
                        column: x => x.PacientRecordId,
                        principalTable: "PacientRecord",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_medicalConditions_pacients_PacientId",
                        column: x => x.PacientId,
                        principalTable: "pacients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trataments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    PacientId = table.Column<Guid>(type: "uuid", nullable: false),
                    PacientRecordId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trataments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trataments_PacientRecord_PacientRecordId",
                        column: x => x.PacientRecordId,
                        principalTable: "PacientRecord",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_trataments_pacients_PacientId",
                        column: x => x.PacientId,
                        principalTable: "pacients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_consultations_PacientId",
                table: "consultations",
                column: "PacientId");

            migrationBuilder.CreateIndex(
                name: "IX_consultations_PacientRecordId",
                table: "consultations",
                column: "PacientRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_medicalConditions_PacientId",
                table: "medicalConditions",
                column: "PacientId");

            migrationBuilder.CreateIndex(
                name: "IX_medicalConditions_PacientRecordId",
                table: "medicalConditions",
                column: "PacientRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_pacients_PacientRecordId",
                table: "pacients",
                column: "PacientRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_trataments_PacientId",
                table: "trataments",
                column: "PacientId");

            migrationBuilder.CreateIndex(
                name: "IX_trataments_PacientRecordId",
                table: "trataments",
                column: "PacientRecordId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "consultations");

            migrationBuilder.DropTable(
                name: "medicalConditions");

            migrationBuilder.DropTable(
                name: "trataments");

            migrationBuilder.DropTable(
                name: "pacients");

            migrationBuilder.DropTable(
                name: "PacientRecord");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
