using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjektLABDetailing.Migrations
{
    /// <inheritdoc />
    public partial class updateServiceAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Condition_Path",
                table: "Cars",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Condition_Path",
                table: "Cars");
        }
    }
}
