using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebStore.Data.Migrations
{
    public partial class init3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cart");

            migrationBuilder.RenameColumn(
                name: "CartId",
                table: "Order",
                newName: "TotalPrice");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Order",
                newName: "NumOfProducts");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Order",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "Order",
                newName: "CartId");

            migrationBuilder.RenameColumn(
                name: "NumOfProducts",
                table: "Order",
                newName: "Amount");

            migrationBuilder.CreateTable(
                name: "Cart",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NumOrders = table.Column<int>(nullable: false),
                    TotalPrice = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cart", x => x.Id);
                });
        }
    }
}
