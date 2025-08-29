using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace approvals.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApprovalTemplates",
                columns: table => new
                {
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalTemplates", x => x.TemplateId);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalInstances",
                columns: table => new
                {
                    InstanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentStageOrder = table.Column<int>(type: "integer", nullable: false),
                    OverallStatus = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalInstances", x => x.InstanceId);
                    table.ForeignKey(
                        name: "FK_ApprovalInstances_ApprovalTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "ApprovalTemplates",
                        principalColumn: "TemplateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StageDefinitions",
                columns: table => new
                {
                    StageDefId = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    StageName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    SequenceOrder = table.Column<int>(type: "integer", nullable: false),
                    AssignmentType = table.Column<string>(type: "text", nullable: false),
                    AssignmentKey = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StageDefinitions", x => x.StageDefId);
                    table.ForeignKey(
                        name: "FK_StageDefinitions_ApprovalTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "ApprovalTemplates",
                        principalColumn: "TemplateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StageInstances",
                columns: table => new
                {
                    StageInstanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApprovalInstanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    StageDefId = table.Column<Guid>(type: "uuid", nullable: false),
                    StageName = table.Column<string>(type: "text", nullable: false),
                    SequenceOrder = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Comments = table.Column<string>(type: "text", nullable: true),
                    ApprovedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StageInstances", x => x.StageInstanceId);
                    table.ForeignKey(
                        name: "FK_StageInstances_ApprovalInstances_ApprovalInstanceId",
                        column: x => x.ApprovalInstanceId,
                        principalTable: "ApprovalInstances",
                        principalColumn: "InstanceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StageInstances_StageDefinitions_StageDefId",
                        column: x => x.StageDefId,
                        principalTable: "StageDefinitions",
                        principalColumn: "StageDefId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalInstances_TemplateId",
                table: "ApprovalInstances",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_StageDefinitions_TemplateId",
                table: "StageDefinitions",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_StageInstances_ApprovalInstanceId",
                table: "StageInstances",
                column: "ApprovalInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_StageInstances_StageDefId",
                table: "StageInstances",
                column: "StageDefId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StageInstances");

            migrationBuilder.DropTable(
                name: "ApprovalInstances");

            migrationBuilder.DropTable(
                name: "StageDefinitions");

            migrationBuilder.DropTable(
                name: "ApprovalTemplates");
        }
    }
}
