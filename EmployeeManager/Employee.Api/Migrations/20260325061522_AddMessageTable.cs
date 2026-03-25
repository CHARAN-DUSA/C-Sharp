using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Employee.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "messageTbl",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderId = table.Column<int>(type: "int", nullable: false),
                    ReceiverId = table.Column<int>(type: "int", nullable: true),
                    MessageText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messageTbl", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_messageTbl_employeeTbl_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "employeeTbl",
                        principalColumn: "employeeId");
                    table.ForeignKey(
                        name: "FK_messageTbl_employeeTbl_SenderId",
                        column: x => x.SenderId,
                        principalTable: "employeeTbl",
                        principalColumn: "employeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_messageTbl_ReceiverId",
                table: "messageTbl",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_messageTbl_SenderId",
                table: "messageTbl",
                column: "SenderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "messageTbl");
        }
    }
}
