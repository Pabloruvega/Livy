using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Livy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentTransactionCodeToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentTransactionCode",
                table: "Orders",
                type: "TEXT",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentTransactionCode",
                table: "Orders");
        }
    }
}
