using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjektLABDetailing.Migrations
{
    /// <inheritdoc />
    public partial class fixedProductDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderProductDetail",
                columns: table => new
                {
                    OrderProductDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderProductId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderProductDetail", x => x.OrderProductDetailId);
                    table.ForeignKey(
                        name: "FK_OrderProductDetail_Order_OrderProductId",
                        column: x => x.OrderProductId,
                        principalTable: "Order",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderProductDetail_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderProductDetail_OrderProductId",
                table: "OrderProductDetail",
                column: "OrderProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProductDetail_ProductId",
                table: "OrderProductDetail",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderProductDetail");
        }
    }
}
