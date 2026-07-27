using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class MoveEquipmentToWorkLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "equipment_id",
                table: "work_logs",
                type: "uuid",
                nullable: true);

            // Существующие записи журнала получают технику, закреплённую за работником до этой миграции
            migrationBuilder.Sql(
                """
                UPDATE work_logs
                SET equipment_id = employees.equipment_id
                FROM employees
                WHERE work_logs.employee_id = employees.id;
                """);

            migrationBuilder.DropForeignKey(
                name: "fk_employees_equipments_equipment_id",
                table: "employees");

            migrationBuilder.DropIndex(
                name: "ix_employees_equipment_id",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "equipment_id",
                table: "employees");

            migrationBuilder.CreateIndex(
                name: "ix_work_logs_equipment_id",
                table: "work_logs",
                column: "equipment_id");

            migrationBuilder.AddForeignKey(
                name: "fk_work_logs_equipments_equipment_id",
                table: "work_logs",
                column: "equipment_id",
                principalTable: "equipments",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "equipment_id",
                table: "employees",
                type: "uuid",
                nullable: true);

            // Восстановление привязки по последней записи журнала с техникой (точное восстановление невозможно)
            migrationBuilder.Sql(
                """
                UPDATE employees
                SET equipment_id = last_logs.equipment_id
                FROM (
                    SELECT DISTINCT ON (employee_id) employee_id, equipment_id
                    FROM work_logs
                    WHERE equipment_id IS NOT NULL
                    ORDER BY employee_id, date DESC
                ) AS last_logs
                WHERE employees.id = last_logs.employee_id;
                """);

            migrationBuilder.DropForeignKey(
                name: "fk_work_logs_equipments_equipment_id",
                table: "work_logs");

            migrationBuilder.DropIndex(
                name: "ix_work_logs_equipment_id",
                table: "work_logs");

            migrationBuilder.DropColumn(
                name: "equipment_id",
                table: "work_logs");

            migrationBuilder.CreateIndex(
                name: "ix_employees_equipment_id",
                table: "employees",
                column: "equipment_id");

            migrationBuilder.AddForeignKey(
                name: "fk_employees_equipments_equipment_id",
                table: "employees",
                column: "equipment_id",
                principalTable: "equipments",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
