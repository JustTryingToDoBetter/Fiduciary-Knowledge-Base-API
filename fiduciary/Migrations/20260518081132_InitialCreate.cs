using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace fiduciary.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KnowledgeArticle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Summary = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Tags = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Source = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeArticle", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "KnowledgeArticle",
                columns: new[] { "Id", "Category", "Content", "Source", "Summary", "Tags", "Title", "UpdatedAtUtc" },
                values: new object[,]
                {
                    { 1, "Fiduciary", "A fiduciary must act with care, honesty, loyalty, and good faith when managing another person's assets or interests.", "Internal starter note", "A fiduciary duty is a responsibility to act in another person's best interests.", "fiduciary,duty,trust,compliance", "What is fiduciary duty?", null },
                    { 2, "Estate Administration", "Deceased estate administration involves identifying assets and liabilities, working with executors, preparing documentation, and distributing the estate according to legal requirements.", "Internal starter note", "The process of managing and distributing a deceased person's estate.", "estate,deceased,executor,assets", "What is deceased estate administration?", null },
                    { 3, "Trust Administration", "Trust administration includes managing trustees, beneficiaries, trust assets, documentation, reporting, and compliance obligations.", "Internal starter note", "The management of a trust according to its deed and applicable law.", "trust,trustees,beneficiaries,compliance", "What is trust administration?", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeArticle_Category",
                table: "KnowledgeArticle",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeArticle_Title",
                table: "KnowledgeArticle",
                column: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KnowledgeArticle");
        }
    }
}
