using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDo_Manager.Migrations
{
    /// <inheritdoc />
    public partial class AddIsEditingField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEditing",
                table: "Tasks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEditing",
                table: "Tasks");
        }
    }
}
