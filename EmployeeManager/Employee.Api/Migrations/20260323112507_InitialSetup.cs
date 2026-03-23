using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Employee.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // All tables already exist in the database.
            // This migration just registers EF Core baseline.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "announcementTbl");
            migrationBuilder.DropTable(name: "departmentTbl");
            migrationBuilder.DropTable(name: "designationTbl");
            migrationBuilder.DropTable(name: "salaryTbl");
            migrationBuilder.DropTable(name: "taskTbl");
            migrationBuilder.DropTable(name: "employeeTbl");
        }
    }
}