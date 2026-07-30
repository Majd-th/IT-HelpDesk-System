using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITHelpDesk.API.Migrations
{
    /// <inheritdoc />
    public partial class CompleteTicketAssignmentWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovalStatus",
                table: "TicketAssignments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AssignmentType",
                table: "TicketAssignments",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TicketAssignments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "TicketAssignments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedDate",
                table: "TicketAssignments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UnassignedDate",
                table: "TicketAssignments",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "TicketAssignments");

            migrationBuilder.DropColumn(
                name: "AssignmentType",
                table: "TicketAssignments");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TicketAssignments");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "TicketAssignments");

            migrationBuilder.DropColumn(
                name: "ReviewedDate",
                table: "TicketAssignments");

            migrationBuilder.DropColumn(
                name: "UnassignedDate",
                table: "TicketAssignments");
        }
    }
}
