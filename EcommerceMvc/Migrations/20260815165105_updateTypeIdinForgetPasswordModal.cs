using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceMvc.Migrations
{
    /// <inheritdoc />
    public partial class updateTypeIdinForgetPasswordModal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUserOtps",
                table: "ApplicationUserOtps");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ApplicationUserOtps");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "ApplicationUserOtps",
                type: "nvarchar(450)",
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUserOtps",
                table: "ApplicationUserOtps",
                column: "Id");
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ApplicationUserOtps",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("SqlServer:Identity", "1, 1");
        }
    }
}