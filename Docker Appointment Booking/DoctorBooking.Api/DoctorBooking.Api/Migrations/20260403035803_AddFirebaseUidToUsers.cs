using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctorBooking.API.Migrations
{
    /// <inheritdoc />
    public partial class AddFirebaseUidToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtpAttempts",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OtpExpiry",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OtpLockedUntil",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "OtpCode",
                table: "AspNetUsers",
                newName: "FirebaseUid");

            migrationBuilder.AlterColumn<string>(
                name: "AuthProvider",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FirebaseUid",
                table: "AspNetUsers",
                newName: "OtpCode");

            migrationBuilder.AlterColumn<string>(
                name: "AuthProvider",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "OtpAttempts",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "OtpExpiry",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OtpLockedUntil",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }
    }
}
