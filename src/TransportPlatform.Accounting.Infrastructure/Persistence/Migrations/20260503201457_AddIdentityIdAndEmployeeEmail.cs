using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransportPlatform.Accounting.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityIdAndEmployeeEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "employees",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "IdentityId",
                table: "employees",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IdentityId",
                table: "customers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_employees_Email",
                table: "employees",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employees_IdentityId",
                table: "employees",
                column: "IdentityId",
                unique: true,
                filter: "\"IdentityId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_customers_IdentityId",
                table: "customers",
                column: "IdentityId",
                unique: true,
                filter: "\"IdentityId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_employees_Email",
                table: "employees");

            migrationBuilder.DropIndex(
                name: "IX_employees_IdentityId",
                table: "employees");

            migrationBuilder.DropIndex(
                name: "IX_customers_IdentityId",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "IdentityId",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "IdentityId",
                table: "customers");
        }
    }
}
