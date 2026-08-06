using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITHelpDesk.API.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkflowTrackingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLogs_Tickets_TicketId",
                table: "ActivityLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLogs_Users_UserId",
                table: "ActivityLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketComments_Users_UserId",
                table: "TicketComments");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "TicketComments");

            migrationBuilder.RenameColumn(
                name: "IsInternal",
                table: "TicketComments",
                newName: "IsPrivate");

            migrationBuilder.AddColumn<string>(
                name: "CommentText",
                table: "TicketComments",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsManagerNote",
                table: "TicketComments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ParentCommentId",
                table: "TicketComments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TicketCommentId",
                table: "TicketComments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "TicketComments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ActivityLogs",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewValue",
                table: "ActivityLogs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreviousValue",
                table: "ActivityLogs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TicketWorkLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    AgentId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MinutesWorked = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketWorkLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketWorkLogs_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketWorkLogs_Users_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketComments_ParentCommentId",
                table: "TicketComments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketComments_TicketCommentId",
                table: "TicketComments",
                column: "TicketCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketWorkLogs_AgentId",
                table: "TicketWorkLogs",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketWorkLogs_TicketId",
                table: "TicketWorkLogs",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLogs_Tickets_TicketId",
                table: "ActivityLogs",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLogs_Users_UserId",
                table: "ActivityLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketComments_TicketComments_ParentCommentId",
                table: "TicketComments",
                column: "ParentCommentId",
                principalTable: "TicketComments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketComments_TicketComments_TicketCommentId",
                table: "TicketComments",
                column: "TicketCommentId",
                principalTable: "TicketComments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketComments_Users_UserId",
                table: "TicketComments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLogs_Tickets_TicketId",
                table: "ActivityLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLogs_Users_UserId",
                table: "ActivityLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketComments_TicketComments_ParentCommentId",
                table: "TicketComments");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketComments_TicketComments_TicketCommentId",
                table: "TicketComments");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketComments_Users_UserId",
                table: "TicketComments");

            migrationBuilder.DropTable(
                name: "TicketWorkLogs");

            migrationBuilder.DropIndex(
                name: "IX_TicketComments_ParentCommentId",
                table: "TicketComments");

            migrationBuilder.DropIndex(
                name: "IX_TicketComments_TicketCommentId",
                table: "TicketComments");

            migrationBuilder.DropColumn(
                name: "CommentText",
                table: "TicketComments");

            migrationBuilder.DropColumn(
                name: "IsManagerNote",
                table: "TicketComments");

            migrationBuilder.DropColumn(
                name: "ParentCommentId",
                table: "TicketComments");

            migrationBuilder.DropColumn(
                name: "TicketCommentId",
                table: "TicketComments");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "TicketComments");

            migrationBuilder.DropColumn(
                name: "NewValue",
                table: "ActivityLogs");

            migrationBuilder.DropColumn(
                name: "PreviousValue",
                table: "ActivityLogs");

            migrationBuilder.RenameColumn(
                name: "IsPrivate",
                table: "TicketComments",
                newName: "IsInternal");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "TicketComments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ActivityLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLogs_Tickets_TicketId",
                table: "ActivityLogs",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLogs_Users_UserId",
                table: "ActivityLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketComments_Users_UserId",
                table: "TicketComments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
