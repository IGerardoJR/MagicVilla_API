using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MagicVilla_API.Migrations
{
    /// <inheritdoc />
    public partial class AlimentarTablaVilla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Villas",
                columns: new[] { "Id", "Amenidad", "Detalle", "FechaActualizacion", "FechaCreacion", "ImagenUrl", "MetrosCuadrados", "Nombre", "Ocupantes", "Tarifa" },
                values: new object[,]
                {
                    { 1, "", "Detalle de la villa...", new DateTime(2023, 11, 3, 1, 32, 55, 287, DateTimeKind.Local).AddTicks(2420), new DateTime(2023, 11, 3, 1, 32, 55, 287, DateTimeKind.Local).AddTicks(2379), "", 50.0, "Villa Real", 5, 200.0 },
                    { 2, "", "Detalle de la villa", new DateTime(2023, 11, 3, 1, 32, 55, 287, DateTimeKind.Local).AddTicks(2424), new DateTime(2023, 11, 3, 1, 32, 55, 287, DateTimeKind.Local).AddTicks(2423), "", 50.0, "Premium Vista a la piscina", 4, 300.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
