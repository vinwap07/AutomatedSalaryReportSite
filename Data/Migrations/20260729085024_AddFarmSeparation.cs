using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFarmSeparation : Migration
    {
        /// <summary>
        /// Ферма, к которой относятся все данные, существовавшие до разделения по фермам.
        /// Должна совпадать с идентификатором первой фермы в конфигурации приложения
        /// </summary>
        private static readonly Guid DefaultFarmId = new("11111111-1111-1111-1111-111111111111");

        private static readonly string[] FarmScopedTables =
        [
            "employees",
            "equipments",
            "reason_for_absences",
            "users",
            "work_logs",
            "work_types",
        ];

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "farms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_farms", x => x.id);
                });

            // Ферма для уже существующих данных; название будет обновлено из конфигурации при старте приложения
            migrationBuilder.InsertData(
                table: "farms",
                columns: ["id", "name"],
                values: [DefaultFarmId, "Ферма №1"]);

            // Значение по умолчанию переносит все существующие записи в первую ферму,
            // после чего оно больше не нужно — ферму всегда проставляет приложение
            foreach (var table in FarmScopedTables)
            {
                migrationBuilder.AddColumn<Guid>(
                    name: "farm_id",
                    table: table,
                    type: "uuid",
                    nullable: false,
                    defaultValue: DefaultFarmId);

                migrationBuilder.Sql($"ALTER TABLE {table} ALTER COLUMN farm_id DROP DEFAULT;");
            }

            migrationBuilder.CreateIndex(
                name: "ix_work_types_farm_id",
                table: "work_types",
                column: "farm_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_logs_farm_id",
                table: "work_logs",
                column: "farm_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_farm_id",
                table: "users",
                column: "farm_id");

            migrationBuilder.CreateIndex(
                name: "ix_reason_for_absences_farm_id",
                table: "reason_for_absences",
                column: "farm_id");

            migrationBuilder.CreateIndex(
                name: "ix_equipments_farm_id",
                table: "equipments",
                column: "farm_id");

            migrationBuilder.CreateIndex(
                name: "ix_employees_farm_id",
                table: "employees",
                column: "farm_id");

            migrationBuilder.AddForeignKey(
                name: "fk_employees_farms_farm_id",
                table: "employees",
                column: "farm_id",
                principalTable: "farms",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_equipments_farms_farm_id",
                table: "equipments",
                column: "farm_id",
                principalTable: "farms",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_reason_for_absences_farms_farm_id",
                table: "reason_for_absences",
                column: "farm_id",
                principalTable: "farms",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_users_farms_farm_id",
                table: "users",
                column: "farm_id",
                principalTable: "farms",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_logs_farms_farm_id",
                table: "work_logs",
                column: "farm_id",
                principalTable: "farms",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_types_farms_farm_id",
                table: "work_types",
                column: "farm_id",
                principalTable: "farms",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_employees_farms_farm_id",
                table: "employees");

            migrationBuilder.DropForeignKey(
                name: "fk_equipments_farms_farm_id",
                table: "equipments");

            migrationBuilder.DropForeignKey(
                name: "fk_reason_for_absences_farms_farm_id",
                table: "reason_for_absences");

            migrationBuilder.DropForeignKey(
                name: "fk_users_farms_farm_id",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "fk_work_logs_farms_farm_id",
                table: "work_logs");

            migrationBuilder.DropForeignKey(
                name: "fk_work_types_farms_farm_id",
                table: "work_types");

            migrationBuilder.DropTable(
                name: "farms");

            migrationBuilder.DropIndex(
                name: "ix_work_types_farm_id",
                table: "work_types");

            migrationBuilder.DropIndex(
                name: "ix_work_logs_farm_id",
                table: "work_logs");

            migrationBuilder.DropIndex(
                name: "ix_users_farm_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_reason_for_absences_farm_id",
                table: "reason_for_absences");

            migrationBuilder.DropIndex(
                name: "ix_equipments_farm_id",
                table: "equipments");

            migrationBuilder.DropIndex(
                name: "ix_employees_farm_id",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "farm_id",
                table: "work_types");

            migrationBuilder.DropColumn(
                name: "farm_id",
                table: "work_logs");

            migrationBuilder.DropColumn(
                name: "farm_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "farm_id",
                table: "reason_for_absences");

            migrationBuilder.DropColumn(
                name: "farm_id",
                table: "equipments");

            migrationBuilder.DropColumn(
                name: "farm_id",
                table: "employees");
        }
    }
}
