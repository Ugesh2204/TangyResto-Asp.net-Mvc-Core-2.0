using Microsoft.EntityFrameworkCore.Migrations;

namespace Tangy.Data.Migrations
{
    public partial class couponcorrectisvalid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "Coupons",
                newName: "IsActive");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Coupons",
                newName: "isActive");
        }
    }
}
