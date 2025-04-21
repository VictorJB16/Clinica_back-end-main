using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinica_back_end.Migrations
{
    /// <inheritdoc />
    public partial class Applicationuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                schema: "identity",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "Nombre",
                schema: "identity",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "Telefono",
                schema: "identity",
                table: "Pacientes");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                schema: "identity",
                table: "Pacientes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_ApplicationUserId",
                schema: "identity",
                table: "Pacientes",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Pacientes_AspNetUsers_ApplicationUserId",
                schema: "identity",
                table: "Pacientes",
                column: "ApplicationUserId",
                principalSchema: "identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pacientes_AspNetUsers_ApplicationUserId",
                schema: "identity",
                table: "Pacientes");

            migrationBuilder.DropIndex(
                name: "IX_Pacientes_ApplicationUserId",
                schema: "identity",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                schema: "identity",
                table: "Pacientes");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "identity",
                table: "Pacientes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                schema: "identity",
                table: "Pacientes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                schema: "identity",
                table: "Pacientes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
