using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjektLABDetailing.Migrations
{
    /// <inheritdoc />
    public partial class updatedatabasev2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Condition_Path",
                table: "Cars",
                newName: "Condition");

            migrationBuilder.AddColumn<string>(
                name: "Condition",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Condition",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "Condition",
                table: "Cars",
                newName: "Condition_Path");
        }
    }
}
