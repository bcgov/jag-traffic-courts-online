﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrafficCourts.Workflow.Service.Migrations.VerifyEmailAddressStateMigrations
{
    /// <inheritdoc />
    public partial class removeconcurrencytoken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xmin",
                table: "VerifyEmailAddressState");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "VerifyEmailAddressState",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);
        }
    }
}
