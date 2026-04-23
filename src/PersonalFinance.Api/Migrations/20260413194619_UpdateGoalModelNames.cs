using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinance.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGoalModelNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeadLine",
                table: "Goals",
                newName: "Deadline");

            migrationBuilder.RenameColumn(
                name: "TargetValue",
                table: "Goals",
                newName: "TargetAmount");

            migrationBuilder.RenameColumn(
                name: "Progres",
                table: "Goals",
                newName: "Progress");

            migrationBuilder.RenameColumn(
                name: "FinancialGoal",
                table: "Goals",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "CreateAt",
                table: "Goals",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Deadline",
                table: "Goals",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Deadline",
                table: "Goals",
                newName: "DeadLine");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Goals",
                newName: "FinancialGoal");

            migrationBuilder.RenameColumn(
                name: "TargetAmount",
                table: "Goals",
                newName: "TargetValue");

            migrationBuilder.RenameColumn(
                name: "Progress",
                table: "Goals",
                newName: "Progres");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Goals",
                newName: "CreateAt");

            migrationBuilder.AlterColumn<int>(
                name: "DeadLine",
                table: "Goals",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");
        }
    }
}
