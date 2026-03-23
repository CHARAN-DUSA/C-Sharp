using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Employee.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaveAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_taskTbl_employeeTbl_AssignedToEmployeeId",
                table: "taskTbl");

            migrationBuilder.CreateTable(
                name: "attendanceTbl",
                columns: table => new
                {
                    AttendanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckIn = table.Column<TimeSpan>(type: "time", nullable: true),
                    CheckOut = table.Column<TimeSpan>(type: "time", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkingHours = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendanceTbl", x => x.AttendanceId);
                    table.ForeignKey(
                        name: "FK_attendanceTbl_employeeTbl_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "employeeTbl",
                        principalColumn: "employeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "leaveTbl",
                columns: table => new
                {
                    LeaveId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    LeaveType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalDays = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedByEmployeeId = table.Column<int>(type: "int", nullable: true),
                    AppliedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leaveTbl", x => x.LeaveId);
                    table.ForeignKey(
                        name: "FK_leaveTbl_employeeTbl_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "employeeTbl",
                        principalColumn: "employeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_attendanceTbl_EmployeeId_Date",
                table: "attendanceTbl",
                columns: new[] { "EmployeeId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_leaveTbl_EmployeeId",
                table: "leaveTbl",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_taskTbl_employeeTbl_AssignedToEmployeeId",
                table: "taskTbl",
                column: "AssignedToEmployeeId",
                principalTable: "employeeTbl",
                principalColumn: "employeeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_taskTbl_employeeTbl_AssignedToEmployeeId",
                table: "taskTbl");

            migrationBuilder.DropTable(
                name: "attendanceTbl");

            migrationBuilder.DropTable(
                name: "leaveTbl");

            migrationBuilder.AddForeignKey(
                name: "FK_taskTbl_employeeTbl_AssignedToEmployeeId",
                table: "taskTbl",
                column: "AssignedToEmployeeId",
                principalTable: "employeeTbl",
                principalColumn: "employeeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
