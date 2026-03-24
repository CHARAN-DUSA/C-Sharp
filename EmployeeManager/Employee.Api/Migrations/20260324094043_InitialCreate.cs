using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Employee.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "announcementTbl",
                columns: table => new
                {
                    AnnouncementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_announcementTbl", x => x.AnnouncementId);
                });

            migrationBuilder.CreateTable(
                name: "departmentTbl",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_departmentTbl", x => x.DepartmentId);
                });

            migrationBuilder.CreateTable(
                name: "designationTbl",
                columns: table => new
                {
                    DesignationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    DesignationName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_designationTbl", x => x.DesignationId);
                });

            migrationBuilder.CreateTable(
                name: "employeeTbl",
                columns: table => new
                {
                    employeeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContactNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pincode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DesignationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AltContactNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DesignationId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employeeTbl", x => x.employeeId);
                });

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

            migrationBuilder.CreateTable(
                name: "salaryTbl",
                columns: table => new
                {
                    SalaryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    BasicSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HRA = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DA = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Bonus = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Deductions = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salaryTbl", x => x.SalaryId);
                    table.ForeignKey(
                        name: "FK_salaryTbl_employeeTbl_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "employeeTbl",
                        principalColumn: "employeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "taskTbl",
                columns: table => new
                {
                    TaskId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssignedToEmployeeId = table.Column<int>(type: "int", nullable: false),
                    AssignedByEmployeeId = table.Column<int>(type: "int", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletionNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HrMessage = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_taskTbl", x => x.TaskId);
                    table.ForeignKey(
                        name: "FK_taskTbl_employeeTbl_AssignedToEmployeeId",
                        column: x => x.AssignedToEmployeeId,
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

            migrationBuilder.CreateIndex(
                name: "IX_salaryTbl_EmployeeId",
                table: "salaryTbl",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_taskTbl_AssignedToEmployeeId",
                table: "taskTbl",
                column: "AssignedToEmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "announcementTbl");

            migrationBuilder.DropTable(
                name: "attendanceTbl");

            migrationBuilder.DropTable(
                name: "departmentTbl");

            migrationBuilder.DropTable(
                name: "designationTbl");

            migrationBuilder.DropTable(
                name: "leaveTbl");

            migrationBuilder.DropTable(
                name: "salaryTbl");

            migrationBuilder.DropTable(
                name: "taskTbl");

            migrationBuilder.DropTable(
                name: "employeeTbl");
        }
    }
}
