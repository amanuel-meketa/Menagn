using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace approvals.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_applicationType_applicationFlowDefinitions_WorkflowDefiniti~",
                table: "applicationType");

            migrationBuilder.DropForeignKey(
                name: "FK_applicationType_formDefinitions_FormDefinitionId",
                table: "applicationType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_applicationType",
                table: "applicationType");

            migrationBuilder.RenameTable(
                name: "applicationType",
                newName: "ApplicationTypes");

            migrationBuilder.RenameIndex(
                name: "IX_applicationType_WorkflowDefinitionId",
                table: "ApplicationTypes",
                newName: "IX_ApplicationTypes_WorkflowDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_applicationType_FormDefinitionId",
                table: "ApplicationTypes",
                newName: "IX_ApplicationTypes_FormDefinitionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationTypes",
                table: "ApplicationTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationTypes_applicationFlowDefinitions_WorkflowDefinit~",
                table: "ApplicationTypes",
                column: "WorkflowDefinitionId",
                principalTable: "applicationFlowDefinitions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationTypes_formDefinitions_FormDefinitionId",
                table: "ApplicationTypes",
                column: "FormDefinitionId",
                principalTable: "formDefinitions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationTypes_applicationFlowDefinitions_WorkflowDefinit~",
                table: "ApplicationTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationTypes_formDefinitions_FormDefinitionId",
                table: "ApplicationTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationTypes",
                table: "ApplicationTypes");

            migrationBuilder.RenameTable(
                name: "ApplicationTypes",
                newName: "applicationType");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationTypes_WorkflowDefinitionId",
                table: "applicationType",
                newName: "IX_applicationType_WorkflowDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationTypes_FormDefinitionId",
                table: "applicationType",
                newName: "IX_applicationType_FormDefinitionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_applicationType",
                table: "applicationType",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_applicationType_applicationFlowDefinitions_WorkflowDefiniti~",
                table: "applicationType",
                column: "WorkflowDefinitionId",
                principalTable: "applicationFlowDefinitions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_applicationType_formDefinitions_FormDefinitionId",
                table: "applicationType",
                column: "FormDefinitionId",
                principalTable: "formDefinitions",
                principalColumn: "Id");
        }
    }
}
