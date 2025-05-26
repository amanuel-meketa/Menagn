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
                name: "applicationFlowDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DefinitionJson = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_applicationFlowDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "formDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SchemaJson = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_formDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "applicationType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FormDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    WorkflowDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_applicationType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_applicationType_applicationFlowDefinitions_WorkflowDefiniti~",
                        column: x => x.WorkflowDefinitionId,
                        principalTable: "applicationFlowDefinitions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_applicationType_formDefinitions_FormDefinitionId",
                        column: x => x.FormDefinitionId,
                        principalTable: "formDefinitions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_applicationType_FormDefinitionId",
                table: "applicationType",
                column: "FormDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_applicationType_WorkflowDefinitionId",
                table: "applicationType",
                column: "WorkflowDefinitionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "applicationType");

            migrationBuilder.DropTable(
                name: "applicationFlowDefinitions");

            migrationBuilder.DropTable(
                name: "formDefinitions");
        }
    }
}
